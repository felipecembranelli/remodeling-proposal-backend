using System;
using System.Linq;
using System.Threading.Tasks;
using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.LLM;
using RemodelingProposalSystem.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using OllamaSharp;
using OllamaSharp.Models;
using System.Text.Json;
using Newtonsoft.Json;
using RemodelingProposalSystem.Infrastructure.LLM.ModelsHelper;

namespace RemodelingProposalSystem.Infrastructure.LLM
{
    public class LlamaService : BaseLLMService
    {
        private readonly OllamaApiClient _ollamaClient;
        private readonly ILogger<LlamaService> _logger;
        private readonly string _modelName;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMaterialRepository _materialRepository;

        public LlamaService(
            IConfiguration configuration,
            ILogger<LlamaService> logger,
            IServiceRepository serviceRepository,
            IMaterialRepository materialRepository)
            : base(logger, "llama2", "Llama")
        {
            _logger = logger;
            _modelName = configuration["Llama:ModelName"] ?? "llama2";
            
            var ollamaUrl = configuration["Llama:OllamaUrl"] ?? "http://localhost:11434";
            _ollamaClient = new OllamaApiClient(ollamaUrl);

            _materialRepository = materialRepository;
            _serviceRepository = serviceRepository;
        }

        //public override async Task<string> GenerateProposalAsync(string prompt)
        //{
        //    return await GenerateProposalAsync(prompt, _defaultModel);
        //}

        public override async Task<string> GetLLMResponseAsync(string prompt, string model)
        {
            try
            {
                LogInfo($"Generating proposal using model: {model}");
                var formattedPrompt = FormatPrompt(prompt);

                var request = new GenerateRequest
                {
                    Model = model,
                    Prompt = formattedPrompt,
                    Stream = false,
                    Options = new RequestOptions
                    {
                        Temperature = 0.7f,
                        NumPredict = 2048,
                        TopK = 40,
                        TopP = 0.9f,
                        RepeatPenalty = 1.1f,
                        Seed = -1,
                        NumCtx = 4096,
                        NumThread = 4
                    }
                };

                var fullResponse = string.Empty;

                await foreach (var stream in _ollamaClient.GenerateAsync(request))
                {
                    fullResponse += stream;
                }
                
                LogInfo("Proposal generated successfully");
                return fullResponse;
            }
            catch (Exception ex)
            {
                LogError(ex, "GenerateProposalAsync");
                throw;
            }
        }

        public override async Task<Proposal> GenerateProposalAsync(string clientName, string propertyType, 
            decimal propertySize, 
            string region, 
            decimal budget, 
            List<string> requestedServices, 
            string siteAnalysis)
        {
            try
            {
                LogInfo($"Generating proposal using model: {_modelName}");

                // Get materials and services data
                var materials = await _materialRepository.GetMaterialsByServicesAsync(requestedServices);
                var servicesDetails = await _serviceRepository.GetRequestedServicesDetailsAsync(requestedServices);
                var totalCost = CalculateTotalCost(materials, servicesDetails);
                var companyStandards = GetRemodelingCompanyStandards();
                var similiarProjectsImages = GetSimilarProjectsImages(propertyType, propertySize);

                // Generate the proposal
                var prompt = GenerateProposalPrompt(clientName, 
                    propertyType, 
                    propertySize, 
                    region, 
                    budget, 
                    materials, 
                    servicesDetails, 
                    totalCost, 
                    siteAnalysis, 
                    companyStandards,
                    similiarProjectsImages);

                var fullResponse = await GenerateProposalWithOllama(prompt);

                // Validate the proposal structure
                var validationResult = ValidateProposalStructure(fullResponse);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Proposal validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                }

                LogInfo("Proposal generated successfully");

                return new Proposal
                {
                    PropertyType = propertyType,
                    PropertySize = propertySize,
                    Region = region,
                    Budget = budget,
                    ProposalRawBody = fullResponse,
                    Status = "Draft"
                };
            }
            catch (Exception ex)
            {
                LogError(ex, "GenerateProposalAsync");
                throw;
            }
        }

        private async Task<string> GenerateProposalWithOllama(string prompt)
        {
            var client = new HttpClient();
            var url = $"{_ollamaClient.Uri}api/generate";
            string jsonContent = System.Text.Json.JsonSerializer.Serialize(new { model = _modelName, prompt = prompt, stream = false });

            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to generate proposal: {response.StatusCode}");
            }

            var tempResponse = await response.Content.ReadAsStringAsync();
            var proposalContent = System.Text.Json.JsonSerializer.Deserialize<OllamaResponse>(tempResponse);
            
            return proposalContent.Response;
        }

        private class ProposalValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        private ProposalValidationResult ValidateProposalStructure(string proposal)
        {
            var result = new ProposalValidationResult { IsValid = true };
            var requiredSections = new[]
            {
                "## 1. EXECUTIVE SUMMARY",
                "## 2. SITE ANALYSIS",
                "## 3. DESIGN CONCEPT",
                "## 4. SCOPE OF WORK",
                "## 5. MATERIALS AND EQUIPMENT",
                "## 6. COST BREAKDOWN",
                "## 7. TIMELINE",
                "## 8. MAINTENANCE PLAN",
                "## 9. TERMS AND CONDITIONS"
            };

            // Check for required sections
            foreach (var section in requiredSections)
            {
                if (!proposal.Contains(section))
                {
                    result.IsValid = false;
                    result.Errors.Add($"Missing section: {section}");
                }
            }

            // Check for cost table
            if (!proposal.Contains("| Cost Category | Amount |"))
            {
                result.IsValid = false;
                result.Errors.Add("Missing cost breakdown table");
            }

            // Check for proper markdown formatting
            if (!proposal.Contains("**Total Cost**"))
            {
                result.IsValid = false;
                result.Errors.Add("Missing bold formatting for total cost");
            }

            // Check for bullet points
            if (!proposal.Contains("-"))
            {
                result.IsValid = false;
                result.Errors.Add("Missing bullet points in sections");
            }

            return result;
        }

        public override string GetModelName()
        {
            return _modelName;
        }

        public override bool IsAvailable()
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 