using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using RemodelingProposalSystem.Core.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.LLM
{
    public abstract class BaseLLMService : ILLMService
    {
        protected readonly ILogger _logger;
        protected readonly string _defaultModel;
        protected readonly string _serviceName;

        protected BaseLLMService(ILogger logger, string defaultModel, string serviceName)
        {
            _logger = logger;
            _defaultModel = defaultModel;
            _serviceName = serviceName;
        }

        public abstract Task<string> GetLLMResponseAsync(string prompt, string model);
        public abstract Task<Proposal> GenerateProposalAsync(string clientName,
            string propertyType,
            decimal propertySize,
            string region,
            decimal budget,
            List<string> requestedServices,
            string siteAnalysis);
        public abstract string GetModelName();
        public abstract bool IsAvailable();

        protected virtual string FormatPrompt(string prompt)
        {
            return $@"You are a professional remodeling contractor and proposal writer. 
Please create a detailed remodeling proposal based on the following information:

{prompt}

Please format the proposal in a professional manner with clear sections for:
1. Client name
2. Project Overview
3. Current Property Assessment and Site Analysis
4. Design Concept and Proposed Solutions
5. Detailed Scope of Work
6. Materials and Finishes
7. Labor and Installation Services
8. Timeline and Project Phases
9. Cost Breakdown and Pricing
10. Required Permits and Approvals
11. Terms and Conditions
12. Similar Projects Portfolio (Show similar project images using IMG tags provided in the prompt)

Notes: 
- If the total cost exceeds the budget, please provide alternative options or suggestions to stay within budget.
- Include any necessary building permits and inspections
- Specify material grades and quality levels
- Use only simple HTML formatting (title, bold, bullet lists). DO NOT add any other tags besides standard HTML tags.
";
        }

        protected virtual void LogError(Exception ex, string operation)
        {
            _logger.LogError(ex, $"[{_serviceName}] Error during {operation}: {ex.Message}");
        }

        protected virtual void LogInfo(string message)
        {
            _logger.LogInformation($"[{_serviceName}] {message}");
        }

        protected decimal CalculateTotalCost(IEnumerable<Material> materials, IEnumerable<Service> services)
        {
            return materials.Sum(m => m.TotalCost) + services.Sum(s => s.TotalCost);
        }

        protected string GenerateProposalPrompt(
            string clientName,
            string propertyType,
            decimal propertySize,
            string region,
            decimal budget,
            IEnumerable<Material> materials,
            IEnumerable<Service> services,
            decimal totalCost,
            string siteAnalysis,
            string companyStandards,
            List<string> similarProjectsImages)
        {
            var prompt = $@"Generate a detailed remodeling proposal for a {propertySize} square foot {propertyType} property in the {region} region.

Client Information:
{clientName}

Current Property Assessment and Site Analysis:
{siteAnalysis}

Company Standards:
{companyStandards}

Budget: ${budget:N2}
Requested Services: {string.Join(", ", services.Select(s => s.Name))}

Materials Required:
{string.Join("\n", materials.Select(m => $"- {m.Name} ({m.Grade}): ${m.TotalCost:N2}"))}

Services Breakdown:
{string.Join("\n", services.Select(s => $"- {s.Name}: ${s.TotalCost:N2}"))}

Total Cost: ${totalCost:N2}

Similar Projects Portfolio:
{string.Join("\n", similarProjectsImages.Select(img => $"<img src=\"{img}\" alt=\"Similar Project\" style=\"max-width: 300px; margin: 10px;\">"))}

";

            return prompt;
        }

        protected string GetRemodelingCompanyStandards()
        {
            return @"Our company standards for remodeling projects include:
1. Licensed and insured contractors
2. High-quality materials and finishes
3. Compliance with local building codes and regulations
4. Detailed project timelines and milestones
5. Regular client communication and updates
6. Clean work environment and job site management
7. Warranty on all workmanship
8. Eco-friendly and sustainable options when available
9. Professional design consultation
10. Post-completion walkthrough and client satisfaction";
        }

        public List<string> GetSimilarProjectsImages(string propertyType, decimal propertySize)
        {
            return propertyType.ToLower() switch
            {
                "residential" => new List<string>
                {
                    "/img/residential-kitchen1.jpg",
                    "/img/residential-bathroom1.jpg",
                    "/img/residential-living-room1.jpg"
                },
                "commercial" => new List<string>
                {
                    "/img/commercial-office1.jpg",
                    "/img/commercial-retail1.jpg",
                    "/img/commercial-restaurant1.jpg"
                },
                "industrial" => new List<string>
                {
                    "/img/industrial-warehouse1.jpg",
                    "/img/industrial-facility1.jpg",
                    "/img/industrial-office1.jpg"
                },
                _ => new List<string>
                {
                    "/img/remodeling-project1.jpg",
                    "/img/remodeling-project2.jpg",
                    "/img/remodeling-project3.jpg"
                }
            };
        }
    }

    
} 