// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graph;
using System.Threading.Tasks;

namespace S2SIntegration_CreateGraphEntities
{
    public class TaskDefinitionHelper
    {
        private GraphSdkClient graphClient;

        public TaskDefinitionHelper(GraphSdkClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public async Task TaskDefinitionDemoAsync()
        {
            /*
             * Operation 1: Creating Task Definition
             */
            var printTaskDefinition = await CreateTaskDefinitionAsync("Change Job Configuration To GrayScale");

            /*
             *  Operation 2: Create subscription for the above Task Definition
             */
            var taskDefinitionSubscription = await CreateSubscriptionAsync(printTaskDefinition.Id);

            /*
             * When the above subscription is created, we create with expireTime 4230 minutes (~3 days) ahead.
             * Use the below code to renew the subscription.
             */
            // var renewTaskDefinitionSubscription = await RenewSubscriptionAsync(printTaskDefinition.Id, taskDefinitionSubscription.Id);

            /*
             * Operation 3: Creating Task Trigger for printer Id 879146bd-984c-4989-bded-691a209442fc
             */
            var printTaskTrigger = await CreateTaskTriggerAsync(printTaskDefinition.Id, "879146bd-984c-4989-bded-691a209442fc");
        }

        public async Task<PrintTaskDefinition> CreateTaskDefinitionAsync(string taskDefinitionName)
        {
            return await this.graphClient.CreateTaskDefinitionAsync(taskDefinitionName);
        }

        public async Task<PrintTaskTrigger> CreateTaskTriggerAsync(string taskDefinitionId, string printerId)
        {
            return await this.graphClient.CreateTaskTriggerAsync(taskDefinitionId, printerId);
        }

        public async Task<Subscription> CreateSubscriptionAsync(string taskDefinitionId)
        {
            return await this.graphClient.CreateSubscriptionAsync(taskDefinitionId);
        }

        public async Task<Subscription> RenewSubscriptionAsync(string taskDefinitionId, string subscriptionId)
        {
            return await this.graphClient.RenewSubscriptionAsync(taskDefinitionId, subscriptionId);
        }
    }
}