namespace FoodDiaryApp.ApiHelper
{
    public class ApiKeyServiceClientCredentials : Microsoft.Rest.ServiceClientCredentials
    {
        private readonly string _subscriptionKey;

        public ApiKeyServiceClientCredentials(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}
