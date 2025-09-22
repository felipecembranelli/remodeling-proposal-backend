using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using RemodelingProposalSystem.Infrastructure.Data;
using RemodelingProposalSystem.Infrastructure.Repositories;
//using RemodelingProposalSystem.Core.Domain.Entities;
using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Tests.Integration.Repositories
{
    public class ProposalRepositoryIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProposalRepository _repository;
        private readonly string _dbName;

        public ProposalRepositoryIntegrationTests()
        {
            _dbName = $"TestDb_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ProposalRepository(_context);
        }

        [Fact]
        public async Task Repository_CanAddAndRetrieveProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                PropertyType = "Commercial",
                Region = "South",
                PropertySize = 2000,
                Budget = 100000,
                Status = "Draft",
                ProposalRawBody = "Test commercial proposal"
            };

            // Act
            var addedProposal = await _repository.AddAsync(proposal);
            var retrievedProposal = await _repository.GetByIdAsync(proposal.Id.ToString());

            // Assert
            Assert.NotNull(retrievedProposal);
            Assert.Equal(proposal.PropertyType, retrievedProposal.PropertyType);
            Assert.Equal(proposal.Budget, retrievedProposal.Budget);
        }

        [Fact]
        public async Task Repository_CanUpdateProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                PropertyType = "Commercial",
                Region = "South",
                PropertySize = 2000,
                Budget = 100000,
                Status = "Draft"
            };

            await _repository.AddAsync(proposal);

            // Act
            proposal.Status = "In Review";
            var updatedProposal = await _repository.UpdateAsync(proposal);
            var retrievedProposal = await _repository.GetByIdAsync(proposal.Id.ToString());

            // Assert
            Assert.Equal("In Review", retrievedProposal.Status);
        }

        [Fact]
        public async Task Repository_CanDeleteProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                PropertyType = "Commercial",
                Region = "South"
            };

            await _repository.AddAsync(proposal);

            // Act
            await _repository.DeleteAsync(proposal.Id.ToString());
            var retrievedProposal = await _repository.GetByIdAsync(proposal.Id.ToString());

            // Assert
            Assert.Null(retrievedProposal);
        }

        [Fact]
        public async Task Repository_CanGetAllProposals()
        {
            // Arrange
            var proposals = new[]
            {
                new Proposal { Id = Guid.NewGuid(), PropertyType = "Commercial" },
                new Proposal { Id = Guid.NewGuid(), PropertyType = "Residential" },
                new Proposal { Id = Guid.NewGuid(), PropertyType = "Industrial" }
            };

            foreach (var proposal in proposals)
            {
                await _repository.AddAsync(proposal);
            }

            // Act
            var allProposals = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(3, allProposals.Count());
            Assert.Contains(allProposals, p => p.PropertyType == "Commercial");
            Assert.Contains(allProposals, p => p.PropertyType == "Residential");
            Assert.Contains(allProposals, p => p.PropertyType == "Industrial");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 