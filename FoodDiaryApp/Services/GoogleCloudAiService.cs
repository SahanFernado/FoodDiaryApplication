using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Newtonsoft.Json;

namespace FoodDiaryApp.Services
{
    public class GoogleCloudAiService
    {
        private readonly LanguageServiceClient _languageServiceClient;
        private readonly HttpClient _httpClient;
        private readonly string _nutritionApiKey;
        private readonly string _nutritionAppId;

        public GoogleCloudAiService(string credentialPath, string nutritionApiKey, string nutritionAppId)
        {
            var credential = GoogleCredential.FromFile(credentialPath);
            _languageServiceClient = LanguageServiceClient.Create();
            _httpClient = new HttpClient();
            _nutritionApiKey = nutritionApiKey;
            _nutritionAppId = nutritionAppId;
        }

        public async Task<string> AnalyzeFoodCaloriesAsync(string text)
        {
            // Analyze text with Google Cloud Natural Language API
            var document = Document.FromPlainText(text);

            try
            {
                var response = await _languageServiceClient.AnalyzeEntitiesAsync(document);

                // Extract food items from the response
                var foodItems = response.Entities;
                if (foodItems.Count == 0)
                {
                    return "No food items detected.";
                }

                // Get calorie information for each food item
                string result = "";
                foreach (var entity in foodItems)
                {
                    var foodName = entity.Name;
                    var calories = await GetCaloriesForFoodItemAsync(foodName);
                    result += $"{foodName}: {calories} calories\n";
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task<string> GetCaloriesForFoodItemAsync(string foodName)
        {
            // Call the nutrition API (e.g., Edamam)
            var requestUrl = $"https://api.edamam.com/api/food-database/v2/parser?ingr={foodName}&app_id={_nutritionAppId}&app_key={_nutritionApiKey}";
            var response = await _httpClient.GetStringAsync(requestUrl);
            dynamic jsonResponse = JsonConvert.DeserializeObject(response);

            if (jsonResponse.hints.Count > 0)
            {
                return jsonResponse.hints[0].food.nutrients.ENERC_KCAL.ToString();
            }
            return "N/A";
        }
    }
}
