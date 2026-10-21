
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HealthTrackPro.Web.Areas.Client.Models;

namespace HealthTrackPro.Web.Areas.Client.Services
{
    public class ClientAiCoachService
    {
        private const string ApiEndpoint =
            "https://api.x.ai/v1/responses";

        private const string DefaultModel =
            "grok-4.7";

        private const string SystemInstructions =
            "You are HealthTrack AI Coach, a friendly fitness, " +
            "nutrition and general wellness assistant. " +
            "Give clear, practical and realistic advice. " +
            "Focus on safe workouts, balanced nutrition, " +
            "recovery, hydration and sustainable fitness habits. " +
            "Do not invent personal health records, measurements " +
            "or trainer assignments. " +
            "Do not diagnose diseases, prescribe medications " +
            "or provide dangerous exercise or diet instructions. " +
            "For injuries, concerning symptoms, medical conditions " +
            "or eating disorders, recommend appropriate " +
            "professional support. " +
            "Be encouraging without making unrealistic promises. " +
            "Keep answers concise and easy for beginners to follow.";

        private static readonly HttpClient Client =
            new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(120)
            };

        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(
                    Environment.GetEnvironmentVariable(
                        "XAI_API_KEY"));
            }
        }

        public async Task<string> GetReplyAsync(
            IList<AiChatMessageViewModel> history)
        {
            string apiKey =
                Environment.GetEnvironmentVariable(
                    "XAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Live AI is not configured. " +
                    "Please contact the application administrator.");
            }

            apiKey = apiKey.Trim();

            if (history == null || history.Count == 0)
            {
                throw new ArgumentException(
                    "Chat history cannot be empty.");
            }

            string model =
                Environment.GetEnvironmentVariable(
                    "XAI_MODEL");

            if (string.IsNullOrWhiteSpace(model))
            {
                model = DefaultModel;
            }

            var messages = new List<object>();

            messages.Add(new
            {
                role = "system",
                content = SystemInstructions
            });

            var recentMessages = history
                .Where(m =>
                    m != null &&
                    (m.Role == "user" ||
                     m.Role == "assistant") &&
                    !string.IsNullOrWhiteSpace(m.Content))
                .TakeLastCompatible(10);

            foreach (var message in recentMessages)
            {
                messages.Add(new
                {
                    role = message.Role,
                    content = message.Content
                });
            }

            var requestBody = new
            {
                model = model.Trim(),
                input = messages,
                store = false,
                reasoning = new
                {
                    effort = "low"
                },
                max_output_tokens = 700
            };

            string json =
                JsonConvert.SerializeObject(requestBody);

            using (var request = new HttpRequestMessage(
                HttpMethod.Post,
                ApiEndpoint))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        apiKey);

                request.Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                using (var response =
                    await Client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode ==
                            HttpStatusCode.Unauthorized ||
                            response.StatusCode ==
                            HttpStatusCode.Forbidden)
                        {
                            throw new InvalidOperationException(
                                "AI authentication failed. " +
                                "Please check the server API configuration.");
                        }

                        if ((int)response.StatusCode == 429)
                        {
                            throw new InvalidOperationException(
                                "The AI service is currently busy " +
                                "or has reached its usage limit. " +
                                "Please try again later.");
                        }

                        throw new InvalidOperationException(
                            "The AI service could not process " +
                            "your request. Please try again later.");
                    }

                    string responseJson =
                        await response.Content.ReadAsStringAsync();

                    var data = JObject.Parse(responseJson);

                    var output = data["output"] as JArray;

                    if (output == null)
                    {
                        throw new InvalidOperationException(
                            "The AI service returned an unexpected response.");
                    }

                    var answerParts = new List<string>();

                    foreach (var outputItem in output)
                    {
                        if ((string)outputItem["type"] != "message")
                        {
                            continue;
                        }

                        var contentItems =
                            outputItem["content"] as JArray;

                        if (contentItems == null)
                        {
                            continue;
                        }

                        foreach (var contentItem in contentItems)
                        {
                            if ((string)contentItem["type"] ==
                                "output_text")
                            {
                                string text =
                                    (string)contentItem["text"];

                                if (!string.IsNullOrWhiteSpace(text))
                                {
                                    answerParts.Add(text.Trim());
                                }
                            }
                        }
                    }

                    string answer =
                        string.Join(
                            Environment.NewLine,
                            answerParts);

                    if (string.IsNullOrWhiteSpace(answer))
                    {
                        throw new InvalidOperationException(
                            "The AI service returned no readable answer. " +
                            "Please try again.");
                    }

                    return answer;
                }
            }
        }
    }

    internal static class AiCoachListExtensions
    {
        public static List<T> TakeLastCompatible<T>(
            this IEnumerable<T> source,
            int count)
        {
            return source
                .Reverse()
                .Take(count)
                .Reverse()
                .ToList();
        }
    }
}
