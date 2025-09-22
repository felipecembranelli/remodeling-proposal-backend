using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using RemodelingProposalSystem.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using RemodelingProposalSystem.Core.Models;
//using RemodelingProposalSystem.Core.Domain.Entities;

namespace RemodelingProposalSystem.Tests.Integration.Data
{
    public class DatabaseIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly string _dbName;

        public DatabaseIntegrationTests()
        {
            _dbName = $"TestDb_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Database_CanSaveAndRetrieveProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                PropertyType = "Residential",
                Region = "North",
                PropertySize = 1000,
                Budget = 50000,
                Status = "Draft",
                ProposalRawBody = "Test proposal content"
            };

            // Act
            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            var retrievedProposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == proposal.Id);

            // Assert
            Assert.NotNull(retrievedProposal);
            Assert.Equal(proposal.PropertyType, retrievedProposal.PropertyType);
            Assert.Equal(proposal.Budget, retrievedProposal.Budget);
        }

        [Fact]
        public async Task Database_CanUpdateProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                PropertyType = "Residential",
                Region = "North",
                PropertySize = 1000,
                Budget = 50000,
                Status = "Draft"
            };

            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            // Act
            proposal.Status = "Submitted";
            _context.Proposals.Update(proposal);
            await _context.SaveChangesAsync();

            var updatedProposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == proposal.Id);

            // Assert
            Assert.Equal("Submitted", updatedProposal.Status);
        }

        [Fact]
        public async Task Database_CanDeleteProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                PropertyType = "Residential",
                Region = "North"
            };

            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            // Act
            _context.Proposals.Remove(proposal);
            await _context.SaveChangesAsync();

            var deletedProposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == proposal.Id);

            // Assert
            Assert.Null(deletedProposal);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 