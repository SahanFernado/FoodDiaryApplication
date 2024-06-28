using OpenAI_API;

namespace FoodDiaryApp.Services
{
    public class CalorieEstimatorService
    {
        private readonly OpenAIAPI _api;

        public CalorieEstimatorService(string apiKey)
        {
            _api = new OpenAIAPI(apiKey);
        }

        public async Task<string> EstimateCalories(string mealDescription)
        {
            
            //get openai chat response


            var result = await _api.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest
            {
                Prompt = $"Estimate the approximate calories in the following meal: {mealDescription}",
                
                MaxTokens = 50
            });

            return result.Completions[0].Text.Trim();
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                var test = await _api.Models.GetModelsAsync();

                var result = await _api.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest
                {
                    Prompt = "Say hello",
                    MaxTokens = 500
                });

                return result != null && result.Completions.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
