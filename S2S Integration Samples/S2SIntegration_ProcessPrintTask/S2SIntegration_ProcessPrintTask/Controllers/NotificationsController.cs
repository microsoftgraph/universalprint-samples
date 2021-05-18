// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_ProcessPrintTask.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using S2SIntegration_ProcessPrintTask.Graph;
    using S2SIntegration_ProcessPrintTask.Graph.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IHttpProvider httpProvider;

        public NotificationsController(IHttpProvider httpProvider)
        {
            this.httpProvider = httpProvider ?? throw new ArgumentNullException(nameof(httpProvider));
        }

        [HttpPost]
        public async Task<ActionResult<string>> ReceiveNotificationsAsync([FromQuery]string validationToken = null)
        {
            /* Handle validation
             * 
             * During create subscription operation, Graph Webhook Service validates the notification url provided, 
             * by pinging with a validationToken.
             * The client needs to return a 200 OK response with same validation token.*/
            if (!string.IsNullOrEmpty(validationToken))
            {
                Console.WriteLine($"Received Token: '{validationToken}'");
                return Ok(validationToken);
            }
            
            /* 
             * handle notifications
             */
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string content = await reader.ReadToEndAsync();

                var notifications = JsonConvert.DeserializeObject<Notifications>(content);

                var notification = notifications?.Items.FirstOrDefault();
                if (notification != null)
                {
                    /* Create a graphServiceClient Instance */
                    var graphServiceClient = GraphHelper.CreateGraphServiceClient(notification.TenantId, this.httpProvider);

                    /*
                     * Validate client state, ensure that you are getting the clientState value which was provided during creation of subscription
                     * If client state does not match, reject the request,
                     * as it is possible that the change notification has not originated from Microsoft Graph and may have been sent by a rogue actor.
                     */
                    if (!notification.ClientState.Equals(CommonConstants.ClientState))
                    {
                        return BadRequest("Invalid client state");
                    }

                    /*
                     * The following regex finds guid in a string.
                     */
                    string regexp = @"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}";
                    
                    /*
                     * The string, notification.resource, will be print/taskDefinitions/{taskDefinitionId}/tasks/{taskId}
                     * For example: print/taskDefinitions/8119abe3-b5f5-457c-8a32-d83d94816cca/tasks/b9be9576-099e-48ee-a06b-1c1bf9245607
                     * 
                     * In plain english, what the above string says is,
                     * the current notification is received because a print task of taskDefintion type "8119abe3-b5f5-457c-8a32-d83d94816cca" was created. And the id of the task is "b9be9576-099e-48ee-a06b-1c1bf9245607".
                     * And the notification was sent because we created a subscription to notify us at this endpoint when a task is created for task definition "8119abe3-b5f5-457c-8a32-d83d94816cca".
                     * If we didn't create that subscription, we wouldn't have got this notification.
                     * 
                     * Let's parse the value and get taskDefinitionId and taskId
                     */
                    var matches = Regex.Matches(notification.Resource, regexp);
                    var taskDefinitionId = matches[0].Value;
                    var taskId = matches[1].Value;

                    /*
                     * The string, notification.ResourceData.ParentUrl, will be https://graph.microsoft.com/v1.0/print/printers/{printerId}/jobs/{jobId}
                     * For example: https://graph.microsoft.com/v1.0/print/printers/879146bd-984c-4989-bded-691a209442fc/jobs/1
                     * 
                     * In other words, the parentUrl says what made this current notification go off, in this case, it's because a job with {jobId} was submitted to printer {printerId}
                     * This happened because we created a task trigger on printer Id "8119abe3-b5f5-457c-8a32-d83d94816cca" that goes off and creates a task when a job is submitted to the printer.
                     * 
                     * Let's parse the value and get printerId and jobId
                     */

                    var printerId = Regex.Match(notification.ResourceData.ParentUrl, regexp).Value;
                    string jobId = notification.ResourceData.ParentUrl.Substring(notification.ResourceData.ParentUrl.LastIndexOf('/') + 1);

                    // Read printJob
                    var printJob = this.GetPrintJobAsync(graphServiceClient, printerId, jobId).Result;

                    // Update the printJob job configuration to print the document in GrayScale and validate the colorMode.
                    var updatedPrintJob = this.PatchPrinterAttributeToGrayscaleAsync(graphServiceClient, printerId, jobId).Result;

                    // Completed the PrintTask, so that UniversalPrint can resume the PrintJob.
                    this.CompletePrintTaskAsync(graphServiceClient, taskDefinitionId, taskId).Wait();
                }
            }

            return Ok();
        }

        public async Task<PrintJob> PatchPrinterAttributeToGrayscaleAsync(GraphServiceClient graphServiceClient, string printerId, string jobId)
        {
            /*
             * STEP 1: Create a PrintJob with ColorMode Set to GrayScale in JobConfiguration
             */
            var updateJobRequest = new PrintJob
            {
                Configuration = new PrintJobConfiguration
                {
                    ColorMode = PrintColorMode.Grayscale
                }
            };

            /*
             * STEP 2: Create a Graph Request Object for PrintJob
             */
            var updatedPrintJobRequest = graphServiceClient.Print.Printers[printerId].Jobs[jobId]
                .Request();

            /*
             * Update the jobConfiguration using Update Operation.
             */
            return await updatedPrintJobRequest.UpdateAsync(updateJobRequest);
        }

        public async Task CompletePrintTaskAsync(GraphServiceClient graphServiceClient, string taskDefinitionId, string taskId)
        {
            /*
             * STEP 1: Create PrintTask with status completed
             */
            var completeTaskRequest = new PrintTask
            {
                Status = new PrintTaskStatus
                {
                    State = PrintTaskProcessingState.Completed
                }
            };

            /*
             * STEP 2: Create a Graph Request Object for PrintTask
             */
            var updatedTaskRequest = graphServiceClient.Print.TaskDefinitions[taskDefinitionId].Tasks[taskId]
                .Request();

            /*
             * STEP 3: Update the Print Task using Update operation.
             */
            await updatedTaskRequest.UpdateAsync(completeTaskRequest);
        }

        public async Task<PrintJob> GetPrintJobAsync(GraphServiceClient graphServiceClient, string printerId, string jobId)
        {
            /*
             * STEP 1: Create "PrintJob" graph request object
             */
            var getJobRequest = graphServiceClient.Print.Printers[printerId].Jobs[jobId]
                .Request();

            /*
             * STEP 2: Make "GET" request
             */
            var printJob = await getJobRequest.GetAsync();

            return printJob;
        }
    }
}
 