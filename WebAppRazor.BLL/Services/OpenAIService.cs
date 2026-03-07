using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebAppRazor.BLL.Services
{
    public class OpenAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string _model;
        private readonly ILogger<OpenAIService> _logger;

        private const int MaxRetries = 3;
        private static readonly TimeSpan[] RetryDelays = new[]
        {
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        };

        public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"];
            _model = configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
            _logger = logger;
        }

        public async Task<AIMenuResult> GenerateMenuWithAIAsync(double targetCalories, bool isPremium, string? goal = null, string? activityLevel = null, string? allergies = null, string? favoriteFoods = null)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("OpenAI API key not configured. Falling back to default menu generation.");
                return new AIMenuResult { Success = false, ErrorMessage = "API key not configured" };
            }

            try
            {
                var prompt = BuildPrompt(targetCalories, isPremium, goal, activityLevel, allergies, favoriteFoods);
                var responseText = await CallOpenAIWithRetryAsync(prompt);

                if (string.IsNullOrEmpty(responseText))
                {
                    return new AIMenuResult { Success = false, ErrorMessage = "Empty response from OpenAI" };
                }

                var mealItems = ParseAIResponse(responseText, isPremium);

                if (mealItems.Count == 0)
                {
                    return new AIMenuResult { Success = false, ErrorMessage = "Could not parse OpenAI response" };
                }

                return new AIMenuResult { Success = true, MealItems = mealItems };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                return new AIMenuResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        private static string BuildPrompt(double targetCalories, bool isPremium, string? goal, string? activityLevel, string? allergies, string? favoriteFoods)
        {
            var goalText = goal switch
            {
                "LoseWeight" => "giảm cân",
                "GainWeight" => "tăng cân",
                _ => "duy trì cân nặng"
            };

            var activityText = activityLevel switch
            {
                "Sedentary" => "ít vận động",
                "LightlyActive" => "vận động nhẹ",
                "ModeratelyActive" => "vận động vừa phải",
                "VeryActive" => "vận động nhiều",
                "ExtraActive" => "vận động rất nhiều",
                _ => ""
            };

            var allergyText = !string.IsNullOrWhiteSpace(allergies) ? $"Tránh các thực phẩm gây dị ứng: {allergies}." : "";
            var favoriteText = !string.IsNullOrWhiteSpace(favoriteFoods) ? $"Ưu tiên các món ăn yêu thích hoặc nguyên liệu liên quan đến: {favoriteFoods}." : "";

            var premiumExtra = isPremium
                ? "Bao gồm chi tiết nguyên liệu và hướng dẫn nấu ăn từng bước cho mỗi món."
                : "Chỉ cần tên món, mô tả ngắn và thông tin dinh dưỡng.";

            return $@"Bạn là chuyên gia dinh dưỡng. Hãy tạo thực đơn 1 ngày cho người Việt Nam với mục tiêu {goalText}, mức vận động {activityText}.
{allergyText}
{favoriteText}
Tổng calories mục tiêu: {targetCalories:F0} kcal/ngày.
Phân bổ: Bữa sáng ~25%, Bữa trưa ~35%, Bữa tối ~30%, Bữa phụ ~10%.
{premiumExtra}

Trả về JSON theo format sau (KHÔNG có markdown, CHỈ JSON thuần):
{{
  ""meals"": [
    {{
      ""mealType"": ""Breakfast"",
      ""name"": ""Tên món"",
      ""description"": ""Mô tả ngắn"",
      ""calories"": 500,
      ""protein"": 20.0,
      ""carbs"": 60.0,
      ""fat"": 15.0,
      ""ingredients"": ""Nguyên liệu chi tiết"",
      ""cookingInstructions"": ""Hướng dẫn nấu""
    }},
    {{
      ""mealType"": ""Lunch"",
      ""name"": ""Tên món"",
      ""description"": ""Mô tả ngắn"",
      ""calories"": 700,
      ""protein"": 30.0,
      ""carbs"": 80.0,
      ""fat"": 20.0,
      ""ingredients"": ""Nguyên liệu chi tiết"",
      ""cookingInstructions"": ""Hướng dẫn nấu""
    }},
    {{
      ""mealType"": ""Dinner"",
      ""name"": ""Tên món"",
      ""description"": ""Mô tả ngắn"",
      ""calories"": 600,
      ""protein"": 25.0,
      ""carbs"": 70.0,
      ""fat"": 18.0,
      ""ingredients"": ""Nguyên liệu chi tiết"",
      ""cookingInstructions"": ""Hướng dẫn nấu""
    }},
    {{
      ""mealType"": ""Snack"",
      ""name"": ""Tên món"",
      ""description"": ""Mô tả ngắn"",
      ""calories"": 200,
      ""protein"": 8.0,
      ""carbs"": 25.0,
      ""fat"": 5.0,
      ""ingredients"": ""Nguyên liệu chi tiết"",
      ""cookingInstructions"": ""Hướng dẫn nấu""
    }}
  ]
}}

Lưu ý: 
- Các món ăn phải là món Việt Nam truyền thống hoặc phổ biến.
- Calories và dinh dưỡng phải hợp lý và cộng lại gần bằng {targetCalories:F0} kcal.
- Protein tính bằng gram (4 kcal/g), Carbs tính bằng gram (4 kcal/g), Fat tính bằng gram (9 kcal/g).";
        }

        private async Task<string?> CallOpenAIWithRetryAsync(string prompt)
        {
            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    return await CallOpenAIApiAsync(prompt);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    if (attempt == MaxRetries)
                    {
                        _logger.LogWarning("OpenAI API rate limit exceeded after {MaxRetries} retries. Falling back to default menu.", MaxRetries);
                        return null;
                    }

                    var delay = RetryDelays[attempt];
                    _logger.LogWarning("OpenAI API returned 429 (Too Many Requests). Retrying in {Delay}s (attempt {Attempt}/{MaxRetries})...",
                        delay.TotalSeconds, attempt + 1, MaxRetries);
                    await Task.Delay(delay);
                }
            }

            return null;
        }

        private async Task<string?> CallOpenAIApiAsync(string prompt)
        {
            var url = "https://api.openai.com/v1/chat/completions";

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "Bạn là chuyên gia dinh dưỡng. Luôn trả lời bằng JSON thuần, không markdown." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 2048,
                response_format = new { type = "json_object" }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add Authorization header
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync(url, content);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new HttpRequestException("Rate limit exceeded", null, HttpStatusCode.TooManyRequests);
            }

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var choices = doc.RootElement.GetProperty("choices");
            if (choices.GetArrayLength() == 0) return null;

            var messageContent = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent;
        }

        private static List<AIMealItem> ParseAIResponse(string responseText, bool isPremium)
        {
            var items = new List<AIMealItem>();

            try
            {
                // Clean up response - remove markdown code blocks if present
                var cleanJson = responseText.Trim();
                if (cleanJson.StartsWith("```"))
                {
                    var startIdx = cleanJson.IndexOf('[');
                    var endIdx = cleanJson.LastIndexOf(']');
                    if (startIdx >= 0 && endIdx > startIdx)
                    {
                        cleanJson = cleanJson.Substring(startIdx, endIdx - startIdx + 1);
                    }
                }

                // OpenAI with response_format json_object may wrap in an object
                // Try to extract array if wrapped in { "meals": [...] } or similar
                if (cleanJson.StartsWith("{"))
                {
                    using var wrappedDoc = JsonDocument.Parse(cleanJson);
                    var root = wrappedDoc.RootElement;

                    // Try common wrapper keys
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            cleanJson = prop.Value.GetRawText();
                            break;
                        }
                    }
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var parsed = JsonSerializer.Deserialize<List<AIMealItem>>(cleanJson, options);

                if (parsed != null)
                {
                    foreach (var item in parsed)
                    {
                        if (!isPremium)
                        {
                            item.Ingredients = string.Empty;
                            item.CookingInstructions = string.Empty;
                        }
                        items.Add(item);
                    }
                }
            }
            catch
            {
                // If JSON parsing fails, return empty list (will fall back to default generation)
            }

            return items;
        }
    }
}
