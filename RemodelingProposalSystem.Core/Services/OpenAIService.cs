using System.Text.Json;
using LandscapingProposalSystem.Core.Interfaces;
using LandscapingProposalSystem.Core.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.Interfaces;
using LandscapingProposalSystem.Core.Interfaces.Repositories;

namespace LandscapingProposalSystem.Core.Services;

public class OpenAIService : IAIService
{
    private readonly IOpenAIService _openAIService;
    private readonly IConfiguration _configuration;
    private readonly IPricingService _pricingService;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMaterialRepository _materialRepository;

    public OpenAIService(
        IOpenAIService openAIService,
        IConfiguration configuration,
        IPricingService pricingService,
        IServiceRepository serviceRepository,
        IMaterialRepository materialRepository)
    {
        _openAIService = openAIService;
        _configuration = configuration;
        _pricingService = pricingService;
        _serviceRepository = serviceRepository;
        _materialRepository = materialRepository;
    }

    public async Task<RawProposal> GenerateProposalAsync(string propertyType, decimal propertySize, string region, decimal budget)
    {
        var materials = await _materialRepository.GetAllAsync();
        var materialList = string.Join("\n", materials.Select(m => $"- {m.Name}: {m.Description} (Base Price: ${m.BasePrice:F2} per {m.UnitOfMeasure})"));

        var prompt = $@"Generate a detailed landscaping proposal for a {propertySize} square foot {propertyType} property in {region} with a budget of ${budget:F2}.

Available Materials:
{materialList}

Please provide a detailed proposal including:
1. A comprehensive description of the landscaping plan
2. A list of recommended services with quantities and costs
3. A list of required materials with quantities and costs
4. Total cost breakdown

Format the response as a JSON object with the following structure:
{{
    ""description"": ""Detailed description of the landscaping plan..."",
    ""services"": [
        {{
            ""name"": ""Service name"",
            ""description"": ""Service description"",
            ""unitPrice"": 100.00,
            ""quantity"": 2.0,
            ""totalCost"": 200.00,
            ""unitOfMeasure"": ""unit"",
            ""estimatedDuration"": 4.0
        }}
    ],
    ""materials"": [
        {{
            ""name"": ""Material name"",
            ""description"": ""Material description"",
            ""unitPrice"": 50.00,
            ""quantity"": 10.0,
            ""totalCost"": 500.00,
            ""unitOfMeasure"": ""unit""
        }}
    ],
    ""totalCost"": 700.00
}}";

        var chatCompletion = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = "gpt-4",
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a professional landscaping expert. Generate detailed and accurate proposals based on the provided information."),
                ChatMessage.FromUser(prompt)
            },
            Temperature = 0.7f,
            MaxTokens = 2000
        });

        if (!chatCompletion.Successful)
        {
            throw new Exception($"Failed to generate proposal: {chatCompletion.Error?.Message}");
        }

        var response = chatCompletion.Choices.First().Message.Content;
        var proposal = JsonSerializer.Deserialize<RawProposal>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (proposal == null)
        {
            throw new Exception("Failed to deserialize proposal response");
        }

        proposal.Id = Guid.NewGuid();
        proposal.PropertyType = propertyType;
        proposal.PropertySize = propertySize;
        proposal.Region = region;
        proposal.Budget = budget;
        proposal.Status = "Draft";
        proposal.CreatedAt = DateTime.UtcNow;
        proposal.ValidUntil = DateTime.UtcNow.AddDays(30);

        return proposal;
    }

    public async Task<string> GenerateDescriptionAsync(string propertyType, decimal propertySize, string region, decimal budget)
    {
        var materials = await _materialRepository.GetAllAsync();
        var materialList = string.Join("\n", materials.Select(m => $"- {m.Name}: {m.Description} (Base Price: ${m.BasePrice:F2} per {m.UnitOfMeasure})"));

        var prompt = $@"Generate a detailed description for a landscaping proposal for a {propertySize} square foot {propertyType} property in {region} with a budget of ${budget:F2}.

Available Materials:
{materialList}

Please provide a comprehensive description that includes:
1. Overall design concept and vision
2. Key features and focal points
3. Plant selection and placement
4. Hardscaping elements
5. Maintenance considerations
6. Expected timeline for implementation

Format the response as a well-structured text with clear sections and paragraphs.";

        var chatCompletion = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = "gpt-4",
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a professional landscaping expert. Generate detailed and engaging descriptions that highlight the value and beauty of the proposed design."),
                ChatMessage.FromUser(prompt)
            },
            Temperature = 0.7f,
            MaxTokens = 1000
        });

        if (!chatCompletion.Successful)
        {
            throw new Exception($"Failed to generate description: {chatCompletion.Error?.Message}");
        }

        return chatCompletion.Choices.First().Message.Content;
    }

    public async Task<RawProposal> GenerateRawProposalDetailsAsync(
       string propertyType,
       decimal propertySize,
       string region,
       decimal budget,
       List<string> requestedServices)
    {
        var materials = await _materialRepository.GetAllAsync();
        var materialList = string.Join("\n", materials.Select(m => $"- {m.Name}: {m.Description} (Base Price: ${m.BasePrice:F2} per {m.UnitOfMeasure})"));

        var prompt = $@"Generate a detailed landscaping proposal for:
Property Type: {propertyType}
Property Size: {propertySize} sq ft
Region: {region}
Budget: ${budget}
Requested Services: {string.Join(", ", requestedServices)}

Available Materials:
{materialList}

Please provide a detailed proposal that includes:
1. Property details and requirements
2. List of services with quantities and costs
3. Material requirements with quantities and costs
4. Labor estimates
5. Total cost breakdown
6. Timeline estimate

The proposal should be detailed and professional, highlighting the value proposition and benefits of the proposed landscaping work. Use the provided material prices as a reference for cost calculations.";

        var response = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a professional landscaping estimator with expertise in creating detailed proposals."),
                ChatMessage.FromUser(prompt)
            },
            Model = "gpt-4",
            Temperature = 0.7f,
            MaxTokens = 2000
        });

        if (!response.Successful)
            throw new Exception($"Failed to generate raw proposal details: {response.Error?.Message}");

        var rawProposalContent = response.Choices[0].Message.Content;

        if (rawProposalContent == null)
            throw new Exception("Failed to deserialize proposal details");

        var rawProposal = new RawProposal
        {
            PropertyType = propertyType,
            PropertySize = propertySize,
            Region = region,
            Budget = budget,
            ProposalRawBody = rawProposalContent,
            Status = "Draft"
        };

        return rawProposal;
    }

    public async Task<string> GenerateProposalDescriptionAsync(RawProposal proposal)
    {
        var prompt = GenerateDescriptionPrompt(proposal);
        var response = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a professional landscaping writer who creates compelling proposal descriptions."),
                ChatMessage.FromUser(prompt)
            },
            Model = "gpt-4",
            Temperature = 0.7f,
            MaxTokens = 1000
        });

        if (!response.Successful)
            throw new Exception($"Failed to generate proposal description: {response.Error?.Message}");

        return response.Choices[0].Message.Content;
    }

    public async Task<List<Service>> SuggestServicesAsync(
        string propertyType,
        decimal propertySize,
        string region,
        decimal budget)
    {
        var prompt = GenerateServicesPrompt(propertyType, propertySize, region, budget);
        var response = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a professional landscaping consultant who suggests appropriate services based on property details."),
                ChatMessage.FromUser(prompt)
            },
            Model = "gpt-4",
            Temperature = 0.7f,
            MaxTokens = 1000
        });

        if (!response.Successful)
            throw new Exception($"Failed to suggest services: {response.Error?.Message}");

        var servicesJson = response.Choices[0].Message.Content;
        return JsonSerializer.Deserialize<List<Service>>(servicesJson) ?? new List<Service>();
    }

    private string GenerateProposalPrompt(
        string propertyType,
        decimal propertySize,
        string region,
        decimal budget,
        List<string> requestedServices,
        List<Service> availableServices,
        Dictionary<string, decimal> serviceCosts)
    {
        var availableServicesList = string.Join("\n", availableServices.Select(s => 
            $"- {s.Name}: Base price ${s.BasePrice}, Description: {s.Description}"));

        return $@"Generate a detailed landscaping proposal for:
Property Type: {propertyType}
Property Size: {propertySize} sq ft
Region: {region}
Budget: ${budget}
Requested Services: {string.Join(", ", requestedServices)}

Available Services and Base Prices:
{availableServicesList}

Please provide a JSON response with the following structure:
- Property details
- List of services with quantities (only numerical values) and costs (only numerical values)
- Material requirements
- Labor estimates (only numerical value)
- Total cost breakdown (only numerical value)
- Timeline estimate

Note: Use the provided base prices as a reference, but adjust quantities based on property size and region.";
    }

    private string GenerateRawProposalPrompt(
        string propertyType,
        decimal propertySize,
        string region,
        decimal budget,
        List<string> requestedServices)
    {
        return $@"Generate a detailed landscaping proposal for:
Property Type: {propertyType}
Property Size: {propertySize} sq ft
Region: {region}
Budget: ${budget}
Requested Services: {string.Join(", ", requestedServices)}

Please provide a response with the following topics:
- Property details
- List of services with quantities and costs
- Material requirements
- Labor estimates
- Total cost breakdown
- Timeline estimate

The proposal description should highlight the value proposition and benefits of the proposed landscaping work.";
    }

    private string GenerateDescriptionPrompt(RawProposal proposal)
    {
        return $@"Create a compelling description for a landscaping proposal with the following details:
Property Type: {proposal.PropertyType}
Property Size: {proposal.PropertySize} sq ft
Total Cost: ${proposal.TotalCost}
Services: {string.Join(", ", proposal.Services.Select(s => s.Name))}

The description should highlight the value proposition and benefits of the proposed landscaping work.";
    }

    private string GenerateServicesPrompt(
        string propertyType,
        decimal propertySize,
        string region,
        decimal budget)
    {
        return $@"Suggest appropriate landscaping services for:
Property Type: {propertyType}
Property Size: {propertySize} sq ft
Region: {region}
Budget: ${budget}

Please provide a JSON response with a list of services, including:
- Service name and description
- Estimated quantity
- Base price
- Required materials
- Estimated duration";
    }

    public Task<RawProposal> GenerateProposalDetailsAsync(string propertyType, decimal propertySize, string region, decimal budget, List<string> requestedServices)
    {
        throw new NotImplementedException();
    }
} 