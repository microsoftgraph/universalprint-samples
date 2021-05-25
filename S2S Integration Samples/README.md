# Universal Print Service-to-Service Integration Samples

The directory contains samples for service-to-service integration with Universal Print. This sample is provided to demonstrate how developers can use the [Microsoft Graph API](https://aka.ms/UPGraphDocs) to integrate applications with Universal Print.

A video walkthrough of this sample is [here](https://aka.ms/UPDevResources). The code sample demonstrates how easily the cloud applications can register printers and create workflows to manage the jobs on a print queue as it transits Universal Print in Azure.

Build and run this sample to intercept all jobs for a newly registered Universal Print printer, get notified when a new jobs arrives and then execute a simple task (force document to print in grayscale only).

## Prerequisites
- Before using this sample, you need to onboard to [`Universal Print`](https://aka.ms/UPDocs).
- App has scopes provisioned as per the requirements of each API it calls.
- App is already provisioned in the Azure AD tenant under which printer and print jobs need to be accessed.

## Build Instructions
1. Open the solution in VS 2019 (recommended) or VS 2017 and hit 'Restore Nuget Packages'.
2. Use Visual Studio to build the project.

## Details
This code sample demonstrates
1. Registering a [Printer](https://docs.microsoft.com/en-us/graph/api/resources/printer?view=graph-rest-1.0).
2. Creating a [Task Definition](https://docs.microsoft.com/en-us/graph/api/resources/printtaskdefinition?view=graph-rest-1.0).
3. Subscribing to a [JobStarted event notification](https://docs.microsoft.com/en-us/graph/universal-print-webhook-notifications#create-subscription-printtask-triggered-jobstarted-event).
4. Creating a [Task Trigger](https://docs.microsoft.com/en-us/graph/api/resources/printtasktrigger?view=graph-rest-1.0).
5. Receiving a [webhook notification](https://docs.microsoft.com/en-us/graph/universal-print-webhook-notifications) when the [Task](https://docs.microsoft.com/en-us/graph/api/resources/printtask?view=graph-rest-1.0) is created.
6. Getting a [Print Job](https://docs.microsoft.com/en-us/graph/api/resources/printjob?view=graph-rest-1.0) for the task and reading the [job attributes](https://docs.microsoft.com/en-us/graph/api/resources/printjobconfiguration?view=graph-rest-1.0).
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
- [Getting a Print Job](https://docs.microsoft.com/en-us/graph/api/printjob-get?view=graph-rest-1.0&tabs=http) for the task and reading the job attributes.
- [Updating the job's](https://docs.microsoft.com/en-us/graph/api/printjob-update?view=graph-rest-1.0&tabs=http) "Color" attribute to "GrayScale".
- [Completing the Task](https://docs.microsoft.com/en-us/graph/api/printtaskdefinition-update-task?view=graph-rest-1.0&tabs=http).

## Demo Videos
- [Demo: Register and update Printer](https://aka.ms/UP-demo-registerprinter)
- [Demo: Create `Task Trigger` on a printer to hold print jobs and convert them from color to grayscale](https://aka.ms/UP-demo-PrintTask)

## Resources
Code sample to generate a [Certificate Signing Request](https://docs.microsoft.com/en-us/universal-print/hardware/universal-print-oem-certificate-signing-request) with the BouncyCastle C# library.
[Application scopes](https://docs.microsoft.com/en-us/graph/permissions-reference#universal-print-permissions)