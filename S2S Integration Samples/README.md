# Universal Print Service-to-Service Integration Samples

The directory contains samples for Service-to-service integration with Universal Print. This sample is provided to demonstrate how developers can use the [Universal Print Microsoft Graph API](https://aka.ms/UPGraphDocs) to integrate applications with Universal Print.

A video walkthrough of this sample is [here](). The sample demonstrates how services can implement custom print functionality by hooking into and extending the print workflow. Build and run the sample to see how easy it is to update a print job's configuration to force all documents to print in black and white.

## Prerequisites
Before using this sample, you need to onboard to [`universal print`](https://aka.ms/UPDocs):

## Build Instructions
1. Open the solution in Visual Studio and hit 'Restore Nuget Packages'
2. Use Visual Studio to build the project.

## Additional Details
This code sample demonstrates
1. Printer Registration
2. Creating Task Definitions
3. Creating Subscription to the Task Definition
4. Creating Task Triggers
5. Getting a print Job and reading the job attributes
6. Updating the "Color" attribute to "GrayScale"
7. Completing the Task.

The samples are split two solutions(.sln): S2SIntegration_CreateGraphEntities.sln and S2SIntegration_ProcessPrintTask.sln

S2SIntegration_CreateGraphEntities is used for
- Printer Registration
- Creating Task Definitions
- Creating Subscription to the Task Definition
- Renewing Subscription to the Task Definition
- Creating Task Triggers for a printer.

S2SIntegration_ProcessPrintTask is used for
- Receive a webhook notification when a task is created.
- Getting a print Job and reading the job attributes
- Updating the "ColorMode" job configuration to "GrayScale"
- Completing the Task.