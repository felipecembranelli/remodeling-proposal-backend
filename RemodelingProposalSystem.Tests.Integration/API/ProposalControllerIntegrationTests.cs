using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
//using RemodelingProposalSystem.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RemodelingProposalSystem.Infrastructure.Data;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RemodelingProposalSystem.Core.Models;

//using Program = RemodelingProposalSystem.API;

namespace RemodelingProposalSystem.Tests.Integration.API
{
    public class ProposalControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ProposalControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing db context registration
                    var descriptor = services.SingleOrDefault(d => 
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add test database
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateProposal_ReturnsCreatedResponse()
        {
            // Arrange
            var proposal = new Proposal
            {
                PropertyType = "Residential",
                Region = "East",
                PropertySize = 1500,
                Budget = 75000,
                Status = "Draft",
                ProposalRawBody = "Test proposal content"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(proposal),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/proposals", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var createdProposal = JsonConvert.DeserializeObject<Proposal>(responseString);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdProposal);
            Assert.Equal(proposal.PropertyType, createdProposal.PropertyType);
            Assert.Equal(proposal.Budget, createdProposal.Budget);
        }

        [Fact]
        public async Task GetProposal_ReturnsProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                PropertyType = "Commercial",
                Region = "West",
                PropertySize = 3000,
                Budget = 150000,
                Status = "Draft"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(proposal),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/proposals", content);
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            var createdProposal = JsonConvert.DeserializeObject<Proposal>(createResponseString);

            // Act
            var response = await _client.GetAsync($"/api/proposals/{createdProposal.Id}");
            var responseString = await response.Content.ReadAsStringAsync();
            var retrievedProposal = JsonConvert.DeserializeObject<Proposal>(responseString);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(retrievedProposal);
            Assert.Equal(proposal.PropertyType, retrievedProposal.PropertyType);
            Assert.Equal(proposal.Budget, retrievedProposal.Budget);
        }

        [Fact]
        public async Task UpdateProposal_ReturnsUpdatedProposal()
        {
            // Arrange
            var proposal = new Proposal
            {
                PropertyType = "Industrial",
                Region = "North",
                PropertySize = 5000,
                Budget = 250000,
                Status = "Draft"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(proposal),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/proposals", content);
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            var createdProposal = JsonConvert.DeserializeObject<Proposal>(createResponseString);

            // Update the proposal
            createdProposal.Status = "In Review";
            var updateContent = new StringContent(
                JsonConvert.SerializeObject(createdProposal),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync($"/api/proposals/{createdProposal.Id}", updateContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var updatedProposal = JsonConvert.DeserializeObject<Proposal>(responseString);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(updatedProposal);
            Assert.Equal("In Review", updatedProposal.Status);
        }

        [Fact]
        public async Task DeleteProposal_ReturnsNoContent()
        {
            // Arrange
            var proposal = new Proposal
            {
                PropertyType = "Residential",
                Region = "South",
                PropertySize = 1000,
                Budget = 50000,
                Status = "Draft"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(proposal),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/proposals", content);
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            var createdProposal = JsonConvert.DeserializeObject<Proposal>(createResponseString);

            // Act
            var response = await _client.DeleteAsync($"/api/proposals/{createdProposal.Id}");
            var getResponse = await _client.GetAsync($"/api/proposals/{createdProposal.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
} 