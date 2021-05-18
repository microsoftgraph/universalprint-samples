// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_CreateGraphEntities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Graph.Auth;
    using Microsoft.Identity.Client;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Org.BouncyCastle.Crypto;

    public class GraphSdkClient
    {
        private GraphServiceClient graphAppClient;
        private GraphServiceClient graphAdminClient;

        public GraphSdkClient()
        {
            /*
             * Refer https://docs.microsoft.com/en-us/graph/sdks/create-client?tabs=CS for more information on Microsoft Graph client.
             */

            var scopes = new List<string>() { CommonConstants.GraphResource + "/.default" };

            // creating client for admin
            {
                var publicClientApplicationBuilder = PublicClientApplicationBuilder
                    .Create(CommonConstants.ClientId)
                    .WithRedirectUri("http://localhost")
                    .Build();

                var authProvider = new InteractiveAuthenticationProvider(publicClientApplicationBuilder, scopes);

                // GraphServiceClient, by design, will create one HttpClient per client.
                // To avoid this, we have to provide our own HttpProvider, that uses a static HttpClient.
                this.graphAdminClient = new GraphServiceClient(authProvider, new HttpProvider());

                // Make a request to /print/services so the we can cache the UserToken in the HttpProvider class.
                this.GetPrintServices(graphAdminClient, scopes).Wait();
            }

            // creating client for app permission
            {
                string scope = CommonConstants.GraphResource + "/.default";
                var confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(CommonConstants.ClientId)
                    .WithTenantId(CommonConstants.TenantId)
                    .WithClientSecret(CommonConstants.ClientSecret)
                    .Build();

                var clientCredentialAuthProvider = new ClientCredentialProvider(confidentialClientApplication, scope);
                this.graphAppClient = new GraphServiceClient(clientCredentialAuthProvider, new HttpProvider());
            }
        }

        #region Printer APIs
        public async Task UpdatePrinterAsync(string printerId, byte[] content)
        {
            /*
             * STEP 1: Create graph request object for "Printer"
             */
            var request = this.graphAppClient.Print.Printers[printerId]
                    .Request();
            
            /*
             * STEP 2: Write the byte content in the request message.
             * If we use graph client, we won't be able to write content to the request message.
             * So we will get the underlying httpClient, write the content in the request message and make a http patch operation
             */
            var requestMessage = request.GetHttpRequestMessage();
            requestMessage.Content = new ByteArrayContent(content);
            requestMessage.Content.Headers.Add("Content-Type", CommonConstants.MediaTypes.ApplicationIpp);

            /*
             * STEP 3: Make http "Patch" request.
             */
            requestMessage.Method = HttpMethod.Patch;
            await this.graphAppClient.HttpProvider.SendAsync(requestMessage);
        }

        public async Task<Printer> GetPrinterAsync(string printerId)
        {
            /*
             * STEP 1: Create graph request object for "Printer"
             */
            var getPrinterRequest = this.graphAdminClient.Print.Printers[printerId]
                .Request();

            /*
             * STEP 2: Make the "GET" request
             */
            var printer = await getPrinterRequest.GetAsync();

            return printer;
        }

        public async Task<HttpResponseMessage> RegisterPrinterAsync(string printerName, string physicalDeviceId = null)
        {
            /*
             * STEP 1: Make "CreatePrinter" request parameters.
             */

            // 1. Certificate Signing Request (CSR)
            //      When Universal Print get a printer registration request, it registers the device with Azure AD.
            //      To do that, Universal Print uses Certificate Signing Request, or simply called CSR.
            //      The CSR should be part of the "Create" request. 
            //      Refer to code snippet here.
            //      https://docs.microsoft.com/en-us/universal-print/hardware/universal-print-oem-certificate-signing-request

            AsymmetricCipherKeyPair keyPair = Crypto.GenerateKeyPair();
            var certRequest = Crypto.GenerateCertificateSigningRequest(keyPair);
            var certificateSigningRequest = new PrintCertificateSigningRequestObject
            {
                Content = certRequest,
                TransportKey = Crypto.ConvertKeyToString(keyPair.Public, removePemHeaderFooter: true)
            };

            // 2. Printer Properties (Basics)
            var displayName = printerName;
            var manufacturer = "Test Printer Manufacturer";
            var model = "Test Printer Model";
            var hasPhysicalDevice = !string.IsNullOrEmpty(physicalDeviceId);
            /* ########################## END ####################### */

            /*
             * STEP 2: Send the "Create Printer" request.
             */

            // The response of the "Create Printer" request contains the operation ID in the response header.
            // We need operation ID to check the status of the registration.
            // If we use graph sdk to send the "Create Printer" request, we won't be able to read the reponse header.
            // Hence, for creating a printer, let's use HttpClient

            var initiateRegistrationRequest = this.graphAdminClient.Print.Printers
                .Create(displayName, manufacturer, model, certificateSigningRequest, physicalDeviceId, hasPhysicalDevice)
                .Request();
        
            var initiateRegistrationRequestContent = initiateRegistrationRequest.RequestBody;
            var initiateRegistrationRequestMessage = initiateRegistrationRequest.GetHttpRequestMessage();
            initiateRegistrationRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(initiateRegistrationRequestContent), Encoding.UTF8, "application/json");
            initiateRegistrationRequestMessage.Method = HttpMethod.Post;
            var initiateRegistrationResponse = await this.graphAdminClient.HttpProvider.SendAsync(initiateRegistrationRequestMessage);
            /* ########################## END ####################### */

            return initiateRegistrationResponse;
        }

        public async Task<Printer> GetRegisteredPrinterAsync(HttpResponseMessage initiateRegistrationResponse)
        {
            /*
             * STEP 1: Get OperationId from the response header.
             */

            // The format of operationLocation is https://graph.microsoft.com/v1.0/print/operations/67348dfe-b1bb-4e1a-9a17-ca0da73cef64
            initiateRegistrationResponse.Headers.TryGetValues("Operation-Location", out IEnumerable<string> operationLocation);
            string operationUri = operationLocation.First();
            string operationId = operationUri.Substring(operationUri.LastIndexOf('/') + 1);

            /*
             *  STEP 2: Poll the operation status
             */
            int triesLeft = 15;
            while (triesLeft-- > 0)
            {
                var printOperation = await this.graphAdminClient.Print.Operations[operationId]
                    .Request().GetAsync();
                var printerCreateOperation = printOperation as PrinterCreateOperation;

                if (printerCreateOperation.Status.State == PrintOperationProcessingState.Running)
                {
                    // If the operation is still "Running", retry after recommended "Retry-After" seconds
                    printerCreateOperation.AdditionalData.TryGetValue("responseHeaders", out object responseHeaders);
                    var retryAfter = ((JObject)responseHeaders)["Retry-After"];

                    var pollingInterval = int.Parse(retryAfter.First.ToString());

                    await Task.Delay(pollingInterval * 1000);
                    continue;
                }
                else
                {
                    if (printerCreateOperation.Status.State == PrintOperationProcessingState.Succeeded)
                    {
                        /*
                         *Store the printer certificate securely, it will be required to generate printer device token, if IPP-Infra APIs are being used.
                         * Documentation to generate printer device token:
                         * https://docs.microsoft.com/en-us/universal-print/hardware/universal-print-oem-printer-registration#3-getting-printer-token 
                         */
                        var cert = printerCreateOperation.Certificate;
                        return printerCreateOperation.Printer;
                    }
                    else
                    {
                        // Error occured while registering printer. PrinterCreateOperation.Status.Description provides the error details. Handle the error appropriately.
                        return null;
                    }
                }
            }
            return null;
        }
        #endregion

        #region TaskDefinition APIs
        public async Task<PrintTaskDefinition> CreateTaskDefinitionAsync(string taskDefinitionName)
        {
            /*
             * STEP 1: Create PrintTaskDefintion object with given task definition name.
             */
            var taskDefinition = new PrintTaskDefinition()
            {
                DisplayName = taskDefinitionName
            };

            /*
             * STEP 2: Create Task Definition graph request Object.
             */
            var createTaskDefinitionRequest = this.graphAppClient.Print.TaskDefinitions
                .Request();

            /*
             * STEP 3: Use Add method on request object to create the task definition
             */
            return await createTaskDefinitionRequest.AddAsync(taskDefinition);
        }

        public async Task<Subscription> CreateSubscriptionAsync(string taskDefinitionId)
        {
            /* Create request body, to create subscription */
            var subscription = new Subscription
            {
                ApplicationId = CommonConstants.ClientId,

                /*
                 * The accepted values are "created", "updated", "deleted".
                 * In this case, "created" is of our interest.
                 * Refer to https://docs.microsoft.com/en-us/graph/api/resources/subscription?view=graph-rest-1.0#properties for more information.
                 */
                ChangeType = "created",

                /*
                 * Any value can be passed as ClientState, the incoming notification will contain the same client state value .
                 * Upon receiving notification, applications can check that the notification came from Universal Print by comparing the value of the clientState property to the clientState property received with each change notification.
                 */
                ClientState = CommonConstants.ClientState,

                /* ExpirationDateTime can be any time in future that is LESS THAN 4230 minutes (~3 days) ahead of current time */
                ExpirationDateTime = DateTime.UtcNow.AddMinutes(CommonConstants.SubscriptionLifetimeInMinutes),

                NotificationUrl = CommonConstants.NotificationUrl,

                Resource = $"/print/taskdefinitions/{taskDefinitionId}/tasks"
            };

            /* Create subscription using graph Client, 
             * We get the createdSubscription as response */
            return await this.graphAppClient.Subscriptions.Request().AddAsync(subscription);
        }

        public async Task<Subscription> RenewSubscriptionAsync(string taskDefinitionId, string subscriptionId)
        {
            /* Create request body to renew subscription, with new expiration time */
            var subscriptionToUpdate = new Subscription
            {
                Id = subscriptionId,
                ExpirationDateTime = DateTime.UtcNow.AddMinutes(CommonConstants.SubscriptionLifetimeInMinutes)
            };

            /* renew subscription by extending expiration time */
            return await this.graphAppClient.Subscriptions[subscriptionId].Request().UpdateAsync(subscriptionToUpdate);
        }

        #endregion

        #region TaskTrigger APIs
        public async Task<PrintTaskTrigger> CreateTaskTriggerAsync(string taskDefinitionId, string printerId)
        {
            /*
             * STEP 1: Create a PrintTaskTrigger object for JobStarted Event and bind it to the the task definition.
             */
            var taskTrigger = new PrintTaskTrigger
            {
                Event = PrintEvent.JobStarted,
                AdditionalData = new Dictionary<string, object>
                {
                    { "definition@odata.bind", "https://graph.microsoft.com/v1.0/print/taskDefinitions/" + taskDefinitionId }
                }
            };

            /*
             * STEP 2: Create Task Trigger Graph Request Object
             * This creates a hook in the printJob lifecycle when it's submitted to the printerId
             */
            var createTaskTriggerRequest = this.graphAdminClient.Print.Printers[printerId].TaskTriggers
                .Request();

            /*
             * STEP 3: Use Add method on request object to create the task trigger
             */
            return await createTaskTriggerRequest.AddAsync(taskTrigger);
        }

        #endregion

        private async Task GetPrintServices(GraphServiceClient graphServiceClient, List<string> scopes)
        {
            var requestWithScope = graphServiceClient.Print.Services
                .Request()
                .WithScopes(scopes.ToArray());

            var services = await requestWithScope.GetAsync();
        }
    }
}
