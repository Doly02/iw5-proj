using System.Net.Http;

namespace Forms.Web.BL
{
    public partial class ResponseApiClient
    {
        public ResponseApiClient(HttpClient httpClient, string baseUrl)
            : this(httpClient)
        {
            BaseUrl = baseUrl;
        }
    }
}