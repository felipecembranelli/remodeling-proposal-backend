using System;
using System.Threading.Tasks;
using RemodelingProposalSystem.Core.LLM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.Interfaces;
using OpenAI;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Core.Interfaces.Repositories;
using OpenAI.Managers;


namespace RemodelingProposalSystem.Infrastructure.LLM
{
    public class OpenAIService : BaseLLMService
    {
        private readonly IOpenAIService _openAIService;
        private readonly string _apiKey;
        private readonly string _modelName;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMaterialRepository _materialRepository;

        public OpenAIService(
            IConfiguration configuration,
            ILogger<OpenAIService> logger,
            IServiceRepository serviceRepository,
            IMaterialRepository materialRepository)
            : base(logger, "gpt-4", "OpenAI")
        {
            _apiKey = configuration["OpenAI:ApiKey"] 
            ?? throw new ArgumentNullException("OpenAI:ApiKey configuration is missing");

            _openAIService = new OpenAI.Managers.OpenAIService(new OpenAiOptions() { ApiKey = _apiKey });

            _modelName = configuration["OpenAI:ModelName"] ?? "gpt-4";

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

                var result = await _openAIService.ChatCompletion.CreateCompletion(new  ChatCompletionCreateRequest()
                  {
                      Model = model,
                      Messages = new[]
                      {
                          new ChatMessage("System", "You are a professional landscaping proposal writer."),
                          new ChatMessage("User", formattedPrompt)
                      },
                      MaxTokens = 2000
                  });

                  var response = result.Choices[0].Message.Content;
                  LogInfo("Proposal generated successfully");

                return string.Empty;
            }
            catch (Exception ex)
            {
                LogError(ex, "GenerateProposalAsync");
                throw;
            }
        }

        public override async Task<Proposal> GenerateProposalAsync(string clientName, string propertyType, decimal propertySize, 
            string region, decimal budget, List<string> requestedServices, string siteAnalysis)
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

                var fullResponse = await GenerateProposalWithOpenAI(prompt);

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

        private async Task<string> GenerateProposalWithOpenAI(string prompt)
        {
            var response = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a professional landscaping estimator with expertise in creating detailed proposals."),
                ChatMessage.FromUser(prompt)
            },
                Model = _modelName,
                //Temperature = 0.7f,
                //MaxTokens = 2000
            });

            if (!response.Successful)
                throw new Exception($"Failed to generate raw proposal details: {response.Error?.Message}");

            var rawProposalContent = response.Choices[0].Message.Content;

            if (rawProposalContent == null)
                throw new Exception("Failed to deserialize proposal details");

            return rawProposalContent;
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
            return _defaultModel;
        }

        public override bool IsAvailable()
        {
            return !string.IsNullOrEmpty(_apiKey);
        }
    }
} 