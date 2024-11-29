using System.Net.Http;

namespace Forms.Web.BL
{
    public partial class FormApiClient
    {
        public FormApiClient(HttpClient httpClient, string baseUrl)
            : this(httpClient)
        {
            BaseUrl = baseUrl;
        }
    }
}