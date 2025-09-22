using LandscapingProposalSystem.Core.Models;

namespace LandscapingProposalSystem.Core.Interfaces;

public interface IAIService
{
    Task<RawProposal> GenerateProposalAsync(string propertyType, decimal propertySize, string region, decimal budget);
    Task<string> GenerateDescriptionAsync(string propertyType, decimal propertySize, string region, decimal budget);
    Task<RawProposal> GenerateRawProposalDetailsAsync(
       string propertyType,
       decimal propertySize,
       string region,
       decimal budget,
       List<string> requestedServices);

    Task<RawProposal> GenerateProposalDetailsAsync(
       string propertyType,
       decimal propertySize,
       string region,
       decimal budget,
       List<string> requestedServices);
} 