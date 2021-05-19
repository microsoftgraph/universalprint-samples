# Universal Print Service-to-Service Integration Samples

The directory contains samples for Service-to-service integration with Universal Print. This sample is provided to demonstrate how developers can use the [Universal Print Microsoft Graph API](https://aka.ms/UPGraphDocs) to integrate applications with Universal Print.

A video walkthrough of this sample is [here](). The sample demonstrates how services can implement custom print functionality by hooking into and extending the print workflow. Build and run the sample to see how easy it is to update a print job's configuration to force all documents to print in black and white to save costs.

## Prerequisites
Before using this sample, you need to onboard to [`Universal Print`](https://aka.ms/UPDocs):

## Build Instructions
1. Open the solution in Visual Studio and hit 'Restore Nuget Packages'
2. Use Visual Studio to build the project.

## Additional Details
This code sample demonstrates
1. Registering [Printers](https://docs.microsoft.com/en-us/graph/api/resources/printer?view=graph-rest-1.0)
2. Creating [Task Definition](https://docs.microsoft.com/en-us/graph/api/resources/printtaskdefinition?view=graph-rest-1.0)
3. Subscribing to [JobStarted event notifications](https://docs.microsoft.com/en-us/graph/universal-print-webhook-notifications#create-subscription-printtask-triggered-jobstarted-event)
4. Creating [Task Triggers](https://docs.microsoft.com/en-us/graph/api/resources/printtasktrigger?view=graph-rest-1.0)
5. Getting a [print Job](https://docs.microsoft.com/en-us/graph/api/resources/printjob?view=graph-rest-1.0) and reading the job attributes
6. Updating the "Color" attribute to "GrayScale"
7. Completing the [Task](https://docs.microsoft.com/en-us/graph/api/resources/printtask?view=graph-rest-1.0).

The samples are split into two solutions(.sln): S2SIntegration_CreateGraphEntities.sln and S2SIntegration_ProcessPrintTask.sln

S2SIntegration_CreateGraphEntities is used for
- Registering Printers
- Creating Task Definition
- Subscribing to JobStarted event notifications
- Renewing subscriptions to JobStarted event notifications
- Creating Task Triggers for a printer.

S2SIntegration_ProcessPrintTask is used for
- Receive a webhook notification when a task is created.
- Getting a print Job and reading the job attributes
- Updating the "ColorMode" job configuration to "GrayScale"
- Completing the Task.