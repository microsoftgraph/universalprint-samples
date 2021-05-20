// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_ProcessPrintTask
{
    using Microsoft.Graph;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpProvider : IHttpProvider, ISerializer
    {
        public HttpProvider()
        {

        }

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = null,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        private static HttpClient httpClient;

        public ISerializer Serializer => this;

        public TimeSpan OverallTimeout { get; set; }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            var response = await SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
            return response;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }

            var response = await httpClient.SendAsync(httpRequestMessage, completionOption, cancellationToken);
            return response;
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }
        }

        /// <inheritdoc />
        public T DeserializeObject<T>(Stream stream)
        {
            var jsonSerializer = new JsonSerializer()
            {
                ContractResolver = null,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            using (StreamReader sr = new StreamReader(stream))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize<T>(jtr);
            }
        }

        /// <inheritdoc />
        public T DeserializeObject<T>(string inputString)
        {
            return JsonConvert.DeserializeObject<T>(inputString, this.jsonSerializerSettings);
        }

        /// <inheritdoc />
        public string SerializeObject(object serializeableObject)
        {
            return JsonConvert.SerializeObject(serializeableObject, this.jsonSerializerSettings);
        }
    }
}
