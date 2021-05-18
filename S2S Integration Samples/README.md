# Universal Print Service-to-Service Integration Samples

The directory contains samples for Service-to-service integration with Universal Print. This sample is provided to demonstrate how developers can use the `Microsoft Graph Universal Print API` to integrate their applications with Universal Print.

The video explanation for this sample can be found [here](). The following code is explained by taking a usecase where applications can create hooks to a print flow and change job configuration to print the document in grayscale when a user prints a job.

## Prerequisites
Before using this sample, you need to onboard to [`universal print`](https://docs.microsoft.com/en-us/universal-print/):

## Build Instructions
1. Open the solution in Visual Studio and hit 'Restore Nuget Package'
2. Use Visual Studio to build the project.

## Additional Details
This code sample contains
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