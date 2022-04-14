using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sys.Data.SqlRemote
{
    public class SqlHttpClient : ISqlMessageClient
    {
        private const string mediaType = "application/json";
        private readonly HttpClient httpClient;
        private readonly string requestUri;

        public SqlHttpClient(string requestUri)
            : this(new HttpClient(), requestUri)
        {
        }

        public SqlHttpClient(HttpClient client, string requestUri)
        {
            this.httpClient = client;
            this.requestUri = requestUri;
        }

        public async Task<SqlResultMessage> RequesteAsync(SqlRequestMessage request)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            string body = Json.Serialize(request);
            var content = new StringContent(body, Encoding.UTF8, mediaType);
            using (HttpResponseMessage response = await httpClient.PostAsync(requestUri, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    body = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(body))
                        return null;

                    var result = Json.Deserialize<SqlResultMessage>(body);
                    return result;
                }
                else
                {
                    string exception = await response.Content.ReadAsStringAsync();
                    return new SqlResultMessage
                    {
                        Error = $"status={response.StatusCode}, reason={response.ReasonPhrase}, {exception}"
                    };
                }
            }
        }
    }
}
