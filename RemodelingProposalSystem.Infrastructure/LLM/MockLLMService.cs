using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RemodelingProposalSystem.Core.LLM;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Infrastructure.LLM.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RemodelingProposalSystem.Infrastructure.LLM
{
    public class MockLLMService : BaseLLMService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint;
        private readonly string _apiKey;
        private readonly ILogger<GemmaService> _logger;

        public MockLLMService(
            IConfiguration configuration,
            ILogger<GemmaService> logger,
            HttpClient httpClient)
            : base(logger, "mockLLM", "mockLLM")
        {
            _httpClient = httpClient;
            _apiEndpoint = "";
            _apiKey = "" ?? throw new ArgumentNullException("configuration is missing");
            _logger = logger;
        }

        public override async Task<string> GetLLMResponseAsync(string prompt, string model)
        {
            try
            {
                LogInfo($"Generating proposal using model: {model}");
                var formattedPrompt = FormatPrompt(prompt);

                var request = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = formattedPrompt }
                            }
                        }
                    }
                };

                var endpointUrl = $"{_apiEndpoint}/{model}:generateContent?key={_apiKey}";

                //var response = await _httpClient.PostAsync(
                //    endpointUrl,
                //    new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
                //);

                string responseContent = "{\"candidates\":[{\"content\":{\"parts\":[{\"text\":\"## Landscaping Proposal: Industrial Property Enhancement\\n\\n**To:** [Client Name/Company Name]\\n**From:** [Your Landscaping Company Name]\\n**Date:** October 26, 2023\\nSincerely,\\n\\n[Your Name]\\n\\n[Your Title]\\n\\n\"}],\"role\":\"model\"},\"finishReason\":\"STOP\",\"avgLogprobs\":-0.35303128420651614}],\"usageMetadata\":{\"promptTokenCount\":353,\"candidatesTokenCount\":2912,\"totalTokenCount\":3265,\"promptTokensDetails\":[{\"modality\":\"TEXT\",\"tokenCount\":353}],\"candidatesTokensDetails\":[{\"modality\":\"TEXT\",\"tokenCount\":2912}]},\"modelVersion\":\"gemini-2.0-flash\"}";

                //response.EnsureSuccessStatusCode();
                //var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Raw response content: {Content}", responseContent);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var result = JsonSerializer.Deserialize<GeminiResponse>(responseContent, options);
                _logger.LogDebug("Deserialized result: {@Result}", result);

                if (result?.Candidates == null || result.Candidates.Length == 0)
                {
                    _logger.LogError("Failed to deserialize response. Candidates is null or empty");
                    throw new Exception("Failed to deserialize Gemini API response");
                }

                LogInfo("Proposal generated successfully");
                var text = result?.Candidates[0]?.Content?.Parts[0]?.Text
                    ?? throw new Exception("No response content received from Gemini API");
                
                // Clean the text by removing unwanted characters from anywhere in the content
                text = text.Replace("```html", "").Replace("```", "");
                text = text.Trim();
                
                return text;
            }
            catch (Exception ex)
            {
                LogError(ex, "GenerateProposalAsync");
                throw;
            }
        }

        public override async Task<Proposal> GenerateProposalAsync(
            string clientName,
            string propertyType,
            decimal propertySize,
            string region,
            decimal budget,
            List<string> requestedServices,
            string siteAnalysis)
        {
            try
            {
                LogInfo($"Generating proposal for {propertySize} sq ft {propertyType} property in {region}");
                
                // Get materials and services from the database or service layer
                var materials = await GetMaterialsAsync(propertyType, propertySize);
                var services = await GetServicesAsync(requestedServices);
                var totalCost = CalculateTotalCost(materials, services);

                // Get company landscaping standards
                var companyStandards = GetRemodelingCompanyStandards();

                // Get similar projects images
                var similarProjectsImages = GetSimilarProjectsImages(propertyType, propertySize);

                var prompt = GenerateProposalPrompt(
                    clientName,
                    propertyType,
                    propertySize,
                    region,
                    budget,
                    materials,
                    services,
                    totalCost,
                    siteAnalysis,
                    companyStandards,
                    similarProjectsImages
                );

                var proposalContent = await GetLLMResponseAsync(prompt, _defaultModel);
                return new Proposal 
                { 
                    Id = Guid.NewGuid(),
                    PropertyType = propertyType,
                    PropertySize = propertySize,
                    Region = region,
                    Budget = budget,
                    RequestedServices = requestedServices,
                    ProposalRawBody = proposalContent,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Draft"
                };
            }
            catch (Exception ex)
            {
                LogError(ex, "GenerateProposalAsync");
                throw;
            }
        }

        public override string GetModelName()
        {
            return _defaultModel;
        }

        public override bool IsAvailable()
        {
            return !string.IsNullOrEmpty(_apiKey);
        }

        private class GeminiResponse
        {
            public Candidate[] Candidates { get; set; }
        }

        private class Candidate
        {
            public Content Content { get; set; }
        }

        private class Content
        {
            public Part[] Parts { get; set; }
        }

        private class Part
        {
            public string Text { get; set; }
        }

        // These methods should be implemented to get actual data from your database or service layer
        private async Task<IEnumerable<Material>> GetMaterialsAsync(string propertyType, decimal propertySize)
        {
            // TODO: Implement actual material retrieval logic
            return new List<Material>();
        }

        private async Task<IEnumerable<Service>> GetServicesAsync(List<string> requestedServices)
        {
            // TODO: Implement actual service retrieval logic
            return new List<Service>();
        }
    }
} 