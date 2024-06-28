using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using FoodDiaryApp.ApiHelper;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Newtonsoft.Json;

public class AiNutritionService
{
    private readonly HttpClient _httpClient;
    private readonly string _nutritionixApiKey;
    private readonly string _nutritionixAppId;
    private readonly ITextAnalyticsClient _textAnalyticsClient;

    public AiNutritionService(string nutritionixApiKey, string nutritionixAppId, string textAnalyticsEndpoint, string textAnalyticsApiKey)
    {
        _httpClient = new HttpClient();
        _nutritionixApiKey = nutritionixApiKey;
        _nutritionixAppId = nutritionixAppId;
        _textAnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(textAnalyticsApiKey))
        {
            Endpoint = textAnalyticsEndpoint
        };
    }

    public async Task<string> GetCaloriesForFoodItemAsync(string foodDescription)
    {
        // Use Azure Text Analytics to extract key phrases (food items)
        var keyPhrasesResponse = await GetKeyPhrases(foodDescription);
        if (keyPhrasesResponse == null || keyPhrasesResponse.Count == 0)
        {
            return "Unable to extract food items from the description.";
        }

        // Use Nutritionix to get calorie information
        var nutritionixResponse = await GetNutritionixCaloriesAsync(string.Join(", ", keyPhrasesResponse));
        return nutritionixResponse;
    }

    private async Task<IList<string>> GetKeyPhrases(string text)
    {
        var input = new MultiLanguageBatchInput(new List<MultiLanguageInput>
        {
            new MultiLanguageInput("0", text, "en")
        });

        KeyPhraseBatchResult result = await _textAnalyticsClient.KeyPhrasesBatchAsync(input, false);

        var keyPhrases = new List<string>();
        foreach (var document in result.Documents)
        {
            keyPhrases.AddRange(document.KeyPhrases);
        }

        return keyPhrases;
    }

    private async Task<string> GetNutritionixCaloriesAsync(string foodNames)
    {
        try
        {
            var requestUrl = $"https://trackapi.nutritionix.com/v2/natural/nutrients";
            var requestBody = new
            {
                query = foodNames
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-app-id", _nutritionixAppId);
            _httpClient.DefaultRequestHeaders.Add("x-app-key", _nutritionixApiKey);

            var response = await _httpClient.PostAsync(requestUrl, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.ReasonPhrase}";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic parsedResponse = JsonConvert.DeserializeObject(jsonResponse);

            if (parsedResponse.foods.Count > 0)
            {
                string foodDescription = string.Empty;
                var totalCalories = 0.0;

                foreach (var foodItem in parsedResponse.foods)
                {
                    JsonDocument document = JsonDocument.Parse(foodItem.ToString());
                    JsonElement root = document.RootElement;

                    if (root.TryGetProperty("nf_calories", out JsonElement nfCalories))
                    {
                        string foodName = root.GetProperty("food_name").GetString();
                        double servingWeightGrams = root.GetProperty("serving_weight_grams").GetDouble();
                        double calories = nfCalories.GetDouble();

                        foodDescription += $"\n{foodName}: Approximately {calories} calories per {servingWeightGrams}g";
                    }
                    else
                    {
                        return $"{foodNames}: No calorie information found";
                    }
                }
                return foodDescription;
            }

            return $"{foodNames}: No calorie information found";
        }
        catch (Exception ex)
        {

            return ex.Message;
        }

    }
}