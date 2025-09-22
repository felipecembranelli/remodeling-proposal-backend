using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Interfaces.Services;
using RemodelingProposalSystem.Core.LLM;
using RemodelingProposalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemodelingProposalSystem.Core.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly ILLMService _aiService;
        private readonly IPricingService _pricingService;

        public ProposalService(
            IProposalRepository proposalRepository,
            ILLMService aiService,
            IPricingService pricingService)
        {
            _proposalRepository = proposalRepository;
            _aiService = aiService;
            _pricingService = pricingService;
        }

        public async Task<IEnumerable<Proposal>> GetAllProposalsAsync()
        {
            return await _proposalRepository.GetAllAsync();
        }

        public async Task<Proposal> GetProposalByIdAsync(Guid id)
        {
            var proposal = await _proposalRepository.GetByIdAsync(id.ToString());
            if (proposal == null)
                throw new KeyNotFoundException($"Proposal with ID {id} not found");

            return proposal;
        }

        public async Task<Proposal> AddProposalAsync(Proposal rawProposal)
        {
            return await _proposalRepository.AddAsync(rawProposal);
        }

        public async Task<Proposal> UpdateProposalAsync(Proposal rawProposal)
        {
            var existingProposal = await _proposalRepository.GetByIdAsync(rawProposal.Id.ToString());
            if (existingProposal == null)
                throw new KeyNotFoundException($"Proposal with ID {rawProposal.Id} not found");

            return await _proposalRepository.UpdateAsync(rawProposal);
        }

        public async Task DeleteRawProposalAsync(Guid id)
        {
            var proposal = await _proposalRepository.GetByIdAsync(id.ToString());
            if (proposal == null)
                throw new KeyNotFoundException($"Proposal with ID {id} not found");

            await _proposalRepository.DeleteAsync(id.ToString());
        }

        public async Task<Proposal> GenerateProposalAsync(
            string propertyType,
            decimal propertySize,
            string region,
            decimal budget,
            List<string> requestedServices,
            string clientName,
            string clientPhone,
            string clientEmail,
            string siteAnalysis)
        {
            // Generate initial proposal details using AI
            var proposal = await _aiService.GenerateProposalAsync(clientName,
                propertyType,
                propertySize,
                region,
                budget,
                requestedServices,
                siteAnalysis);

            // MOCK
            //await Task.Delay(5000);

            //var rawProposal = new RawProposal
            //{
            //    PropertyType = propertyType,
            //    PropertySize = propertySize,
            //    Region = region,
            //    Budget = budget,
            //    ProposalRawBody = "**Landscaping Proposal**\n\n**1. Property Details and Requirements**\n\nThe property in question is a commercial business park located in South California, spanning 8 square feet. Considering the region's climate and the property's size, the landscaping plan must be efficient, sustainable, and visually appealing. The client requests weekly maintenance, water usage optimization, and tree pruning services.\n\n**2. Services with Quantities and Costs**\n\n- Weekly Maintenance: This will include lawn mowing, weeding, leaf blowing, and general upkeep to maintain the aesthetic appeal of the property. Estimated cost: $200 per week for 52 weeks, totaling $10,400 per year. \n\n- Water Usage Optimization: This will involve creating a water-efficient irrigation system to minimize water wastage. Estimated cost: $1,500 for the initial setup.\n\n- Tree Pruning: Regular pruning of the trees in the business park to maintain their health and appearance. Estimated cost: $100 per tree. Assuming there are 10 trees on the property, the total cost would be $1,000 per year.\n\n**3. Material Requirements with Quantities and Costs**\n\nThe materials needed for this project include irrigation piping, mulch, fertilizers, and tree pruning equipment. \n\n- Irrigation Piping: Estimated cost: $500\n- Mulch: Required for maintaining the health of plants and minimizing weed growth. Estimated cost: $200\n- Fertilizers: Necessary for maintaining the health and vibrancy of the lawn. Estimated cost: $300\n- Tree Pruning Equipment: Estimated cost: $300\n\n**4. Labor Estimates**\n\nThe labor required for this project includes a crew for weekly maintenance, a specialist for the irrigation system, and a professional tree pruner. Estimated labor costs are as follows:\n\n- Weekly Maintenance Crew: Estimated cost: $400 per week for 52 weeks, totaling $20,800 per year.\n- Irrigation Specialist: Estimated cost: $1,000\n- Tree Pruner: Estimated cost: $500 per year\n\n**5. Total Cost Breakdown**\n\n- Weekly Maintenance: $10,400\n- Water Usage Optimization: $1,500\n- Tree Pruning: $1,000\n- Materials: $1,300\n- Labor: $22,300\n- Miscellaneous (10% contingency): $3,650\n\nTotal Estimate: $40,150\n\n(Note: This estimate exceeds the initial budget of $10,000. We may need to prioritize services, adjust frequency, or find other cost-saving measures to meet the budget.)\n\n**6. Timeline Estimate**\n\nThe initial setup of the irrigation system and first round of tree pruning would take approximately one month. After this, weekly maintenance and regular tree pruning would continue throughout the year.\n\nWe look forward to the prospect of working on this project. Our team is committed to providing top-notch services that enhance the aesthetic appeal and functionality of your business park.",
            //    RequestedServices = requestedServices
            //};

            // Add client information
            proposal.ClientName = clientName;
            proposal.ClientPhone = clientPhone;
            proposal.ClientEmail = clientEmail;

            // Save proposal to database
            return await _proposalRepository.AddAsync(proposal);
        }

    }
}
