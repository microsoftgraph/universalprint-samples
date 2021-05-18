// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_ProcessPrintTask.Graph
{
    using Microsoft.Graph;
    using Microsoft.Graph.Auth;
    using Microsoft.Identity.Client;

    public static class GraphHelper
    {
        public static GraphServiceClient CreateGraphServiceClient(string tenantId, IHttpProvider httpProvider)
        {
            string scope = CommonConstants.GraphScope;
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
             .Create(CommonConstants.ClientId)
             .WithTenantId(tenantId)
             .WithClientSecret(CommonConstants.ClientSecret)
             .Build();

            var clientCredentialAuthProvider = new ClientCredentialProvider(confidentialClientApplication, scope);
            var graphServiceClient = new GraphServiceClient(clientCredentialAuthProvider, httpProvider);
            return graphServiceClient;
        }
    }
}
