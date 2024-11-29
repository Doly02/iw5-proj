using System.Net.Http;

namespace Forms.Web.BL
{
    public partial class QuestionApiClient
    {
        public QuestionApiClient(HttpClient httpClient, string baseUrl)
            : this(httpClient)
        {
            BaseUrl = baseUrl;
        }
    }
}