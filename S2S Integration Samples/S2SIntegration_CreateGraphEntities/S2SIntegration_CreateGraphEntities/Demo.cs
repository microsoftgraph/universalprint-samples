// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_CreateGraphEntities
{
    public class Demo
    {
        static void Main(string[] args)
        {
            var graphClient = new GraphSdkClient();

            /*
             * Module 1:
             * 1. Create a printer with Universal Print
             * 2. Update the printer to add attributes 'required' by Mopria.
             */
            PrinterRegistrationHelper printerRegistrationHelper = new PrinterRegistrationHelper(graphClient);
            printerRegistrationHelper.PrinterRegistrationDemoAsync().Wait();

            /*
             * Module 2: Adds hook for applications to integrate with print job lifecycle.
             * In this module, we create
             * 1. Task Definition,
             * 2. Subscription on Task Definition so that Universal Print can notify us when a printer creates creates a task of task definition type,
             * 3. Task Trigger.
             */
            TaskDefinitionHelper taskDefinitionHelper = new TaskDefinitionHelper(graphClient);
            taskDefinitionHelper.TaskDefinitionDemoAsync().Wait();
        }
    }
}
