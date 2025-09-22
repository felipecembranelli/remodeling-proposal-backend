using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.LLM;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RemodelingProposalSystem.Tests.Services
{
    public class ProposalServiceTests
    {
        private readonly Mock<IProposalRepository> _mockRepository;
        private readonly Mock<ILLMService> _mockAiService;
        private readonly Mock<IPricingService> _mockPricingService;
        private readonly ProposalService _service;

        public ProposalServiceTests()
        {
            _mockRepository = new Mock<IProposalRepository>();
            _mockAiService = new Mock<ILLMService>();
            _mockPricingService = new Mock<IPricingService>();
            _service = new ProposalService(_mockRepository.Object, _mockAiService.Object, _mockPricingService.Object);
        }

        [Fact]
        public async Task GetAllRawProposalsAsync_ShouldReturnAllProposals()
        {
            // Arrange
            var expectedProposals = new List<Proposal>
            {
                new Proposal { Id = Guid.NewGuid(), PropertyType = "Residential" },
                new Proposal { Id = Guid.NewGuid(), PropertyType = "Commercial" }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedProposals);

            // Act
            var result = await _service.GetAllProposalsAsync();

            // Assert
            Assert.Equal(expectedProposals, result);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRawProposalByIdAsync_WhenProposalExists_ShouldReturnProposal()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedProposal = new Proposal { Id = id, PropertyType = "Residential" };
            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToString()))
                .ReturnsAsync(expectedProposal);

            // Act
            var result = await _service.GetProposalByIdAsync(id);

            // Assert
            Assert.Equal(expectedProposal, result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(id.ToString()), Times.Once);
        }

        [Fact]
        public async Task GetRawProposalByIdAsync_WhenProposalDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToString()))
                .ReturnsAsync((Proposal)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.GetProposalByIdAsync(id));
            _mockRepository.Verify(repo => repo.GetByIdAsync(id.ToString()), Times.Once);
        }

        [Fact]
        public async Task AddRawProposalAsync_ShouldAddAndReturnProposal()
        {
            // Arrange
            var proposal = new Proposal { PropertyType = "Residential" };
            _mockRepository.Setup(repo => repo.AddAsync(proposal))
                .ReturnsAsync(proposal);

            // Act
            var result = await _service.AddProposalAsync(proposal);

            // Assert
            Assert.Equal(proposal, result);
            _mockRepository.Verify(repo => repo.AddAsync(proposal), Times.Once);
        }

        [Fact]
        public async Task UpdateRawProposalAsync_WhenProposalExists_ShouldUpdateAndReturnProposal()
        {
            // Arrange
            var proposal = new Proposal { Id = new Guid(), PropertyType = "Residential" };
            _mockRepository.Setup(repo => repo.GetByIdAsync(proposal.Id.ToString()))
                .ReturnsAsync(proposal);
            _mockRepository.Setup(repo => repo.UpdateAsync(proposal))
                .ReturnsAsync(proposal);

            // Act
            var result = await _service.UpdateProposalAsync(proposal);

            // Assert
            Assert.Equal(proposal, result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(proposal.Id.ToString()), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(proposal), Times.Once);
        }

        [Fact]
        public async Task UpdateRawProposalAsync_WhenProposalDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var proposal = new Proposal { Id = new Guid(), PropertyType = "Residential" };
            _mockRepository.Setup(repo => repo.GetByIdAsync(proposal.Id.ToString()))
                .ReturnsAsync((Proposal)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.UpdateProposalAsync(proposal));
            _mockRepository.Verify(repo => repo.GetByIdAsync(proposal.Id.ToString()), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Proposal>()), Times.Never);
        }

        [Fact]
        public async Task DeleteRawProposalAsync_WhenProposalExists_ShouldDeleteProposal()
        {
            // Arrange
            var id = Guid.NewGuid();
            var proposal = new Proposal { Id = id };
            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToString()))
                .ReturnsAsync(proposal);

            // Act
            await _service.DeleteRawProposalAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id.ToString()), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(id.ToString()), Times.Once);
        }

        [Fact]
        public async Task DeleteRawProposalAsync_WhenProposalDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToString()))
                .ReturnsAsync((Proposal)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.DeleteRawProposalAsync(id));
            _mockRepository.Verify(repo => repo.GetByIdAsync(id.ToString()), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GenerateRawProposalAsync_ShouldGenerateAndSaveProposal()
        {
            // Arrange
            var propertyType = "Residential";
            var propertySize = 1000m;
            var region = "South";
            var budget = 5000m;
            var clientName = "Felipe Cembranelli";
            var clientEmail = "felipecembranelli@live.com";
            var clientPhone = "234-343444";
            var requestedServices = new List<string> { "Lawn Mowing", "Tree Trimming" };
            var siteAnalysis = "Property Characteristics: Large industrial property located in the Southern United States.";

            var generatedProposal = new Proposal
            {
                ClientName = "Felipe Cembranelli",
                ClientEmail = "felipecembranelli@live.com",
                ClientPhone = "234-343444",
                PropertyType = propertyType,
                PropertySize = propertySize,
                Region = region,
                Budget = budget,
                ProposalRawBody = "**Landscaping Proposal**\n\n**1. Property Details and Requirements**\n\nThe property in question is a commercial business park located in South California, spanning 8 square feet. Considering the region's climate and the property's size, the landscaping plan must be efficient, sustainable, and visually appealing. The client requests weekly maintenance, water usage optimization, and tree pruning services.\n\n**2. Services with Quantities and Costs**\n\n- Weekly Maintenance: This will include lawn mowing, weeding, leaf blowing, and general upkeep to maintain the aesthetic appeal of the property. Estimated cost: $200 per week for 52 weeks, totaling $10,400 per year. \n\n- Water Usage Optimization: This will involve creating a water-efficient irrigation system to minimize water wastage. Estimated cost: $1,500 for the initial setup.\n\n- Tree Pruning: Regular pruning of the trees in the business park to maintain their health and appearance. Estimated cost: $100 per tree. Assuming there are 10 trees on the property, the total cost would be $1,000 per year.\n\n**3. Material Requirements with Quantities and Costs**\n\nThe materials needed for this project include irrigation piping, mulch, fertilizers, and tree pruning equipment. \n\n- Irrigation Piping: Estimated cost: $500\n- Mulch: Required for maintaining the health of plants and minimizing weed growth. Estimated cost: $200\n- Fertilizers: Necessary for maintaining the health and vibrancy of the lawn. Estimated cost: $300\n- Tree Pruning Equipment: Estimated cost: $300\n\n**4. Labor Estimates**\n\nThe labor required for this project includes a crew for weekly maintenance, a specialist for the irrigation system, and a professional tree pruner. Estimated labor costs are as follows:\n\n- Weekly Maintenance Crew: Estimated cost: $400 per week for 52 weeks, totaling $20,800 per year.\n- Irrigation Specialist: Estimated cost: $1,000\n- Tree Pruner: Estimated cost: $500 per year\n\n**5. Total Cost Breakdown**\n\n- Weekly Maintenance: $10,400\n- Water Usage Optimization: $1,500\n- Tree Pruning: $1,000\n- Materials: $1,300\n- Labor: $22,300\n- Miscellaneous (10% contingency): $3,650\n\nTotal Estimate: $40,150\n\n(Note: This estimate exceeds the initial budget of $10,000. We may need to prioritize services, adjust frequency, or find other cost-saving measures to meet the budget.)\n\n**6. Timeline Estimate**\n\nThe initial setup of the irrigation system and first round of tree pruning would take approximately one month. After this, weekly maintenance and regular tree pruning would continue throughout the year.\n\nWe look forward to the prospect of working on this project. Our team is committed to providing top-notch services that enhance the aesthetic appeal and functionality of your business park.",
                RequestedServices = new List<string> { "Lawn Mowing", "Tree Trimming" }
            };

            _mockAiService.Setup(ai => ai.GenerateProposalAsync(clientName,
                propertyType, propertySize, region, budget, requestedServices, siteAnalysis))
                .ReturnsAsync(generatedProposal);
            _mockRepository.Setup(repo => repo.AddAsync(generatedProposal))
                .ReturnsAsync(generatedProposal);

            // Act
            var result = await _service.GenerateProposalAsync(
                propertyType, propertySize, region, budget, requestedServices, clientName, clientPhone, clientEmail, siteAnalysis);

            // Assert
            Assert.Equal(generatedProposal, result);
            _mockAiService.Verify(ai => ai.GenerateProposalAsync(clientName,
                propertyType, propertySize, region, budget, requestedServices, siteAnalysis), Times.Once);
            _mockRepository.Verify(repo => repo.AddAsync(generatedProposal), Times.Once);
        }
    }
} 