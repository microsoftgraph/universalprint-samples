# Universal Print Service-to-Service Integration Samples

The directory contains samples for Service-to-service integration with Universal Print. This sample is provided to demonstrate how developers can use the [Universal Print Microsoft Graph API](https://aka.ms/UPGraphDocs) to integrate applications with Universal Print.

A video walkthrough of this sample is [here](https://aka.ms/UPDevResources). The sample demonstrates how services can implement custom print functionality by hooking into and extending the print workflow. Build and run the sample to see how easy it is to update a print job's configuration to force all documents to print in grayscale to save costs.

## Prerequisites
Before using this sample, you need to onboard to [`Universal Print`](https://aka.ms/UPDocs):

## Build Instructions
1. Open the solution in Visual Studio and hit 'Restore Nuget Packages'.
2. Use Visual Studio to build the project.

## Details
This code sample demonstrates
1. Registering a [Printer](https://docs.microsoft.com/en-us/graph/api/resources/printer?view=graph-rest-1.0).
2. Creating a [Task Definition](https://docs.microsoft.com/en-us/graph/api/resources/printtaskdefinition?view=graph-rest-1.0).
3. Subscribing to a [JobStarted event notification](https://docs.microsoft.com/en-us/graph/universal-print-webhook-notifications#create-subscription-printtask-triggered-jobstarted-event).
4. Creating a [Task Trigger](https://docs.microsoft.com/en-us/graph/api/resources/printtasktrigger?view=graph-rest-1.0).
5. Receiving a webhook notification when the [Task](https://docs.microsoft.com/en-us/graph/api/resources/printtask?view=graph-rest-1.0) is created.
6. Getting a [Print Job](https://docs.microsoft.com/en-us/graph/api/resources/printjob?view=graph-rest-1.0) for the task and reading the job attributes.
7. Updating the job's "Color" attribute to "GrayScale".
8. Completing the Task.

The samples are split into two solutions(.sln): S2SIntegration_CreateGraphEntities.sln and S2SIntegration_ProcessPrintTask.sln

S2SIntegration_CreateGraphEntities is used for
- Registering a Printer.
- Creating a Task Definition.
- Subscribing to a JobStarted event notifications.
- Renewing the subscription to the JobStarted event notifications.
- Creating a Task Trigger.

S2SIntegration_ProcessPrintTask is used for
- Receiving a webhook notification when the Task is created.
- Getting a Print Job for the task and reading the job attributes.
- Updating the job's "Color" attribute to "GrayScale".
- Completing the Task.

## Demos
- Demo: [Create Printer]()
- Demo: [Hold print job and implement task trigger]()

## Resources
Code sample to generate a [Certificate Signing Request](https://docs.microsoft.com/en-us/universal-print/hardware/universal-print-oem-certificate-signing-request) with the BouncyCastle C# library.
Documentation to generate a [printer device token](https://docs.microsoft.com/en-us/universal-print/hardware/universal-print-oem-printer-registration#3-getting-printer-token)