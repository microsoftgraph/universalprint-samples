// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_CreateGraphEntities
{
    public class CommonConstants
    {
        public const string TenantId = "138cb46e-4adc-4018-8263-d5b18cdd054e";

        //  Configure your own client id and client secret
        public const string ClientId = "";
        public const string ClientSecret = "";

        public const string PrinterNameFormat = "BUILD2021_{0}";

        public const string ClientState = "9864d2d6-4fa1-40ab-84d9-23093addb340";
        public const int SubscriptionLifetimeInMinutes = 4230; // ~3 days. This is the maximum time that a subscription can persist before it gets deleted.

        // When a task is created for a task definition, the notification is received at the below endpoint.
        // For example, if the value is https://23f084572c67.ngrok.io/api/notification, we will get notification to the endpoint when a task is created for a task definition.
        public const string NotificationUrl = "";

        public const string GraphResource = "https://graph.microsoft.com";
        public const string PrinterAttributeBinLocation = "../../../DemoFiles/GraphPatchPrinterRequestBody.bin";

        public static class MediaTypes
        {
            public const string ApplicationJson = "application/json";
            public const string ApplicationIpp = "application/ipp";
        }
    }
}
