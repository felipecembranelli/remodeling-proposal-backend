using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Interfaces.Services;
using RemodelingProposalSystem.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace RemodelingProposalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProposalsController : ControllerBase
{
    private readonly IProposalService _proposalService;

    public ProposalsController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    // See thread - Need for integration tests with WebApplicationFactory 
    // https://stackoverflow.com/questions/69991983/deps-file-missing-for-dotnet-6-integration-tests

    [HttpPost]
    public async Task<ActionResult<Proposal>> CreateProposal([FromBody] CreateProposalRequest request)
    {
        try
        {
            var proposal = await _proposalService.GenerateProposalAsync(
                request.PropertyType,
                request.PropertySize,
                request.Region,
                request.Budget,
                request.RequestedServices,
                request.ClientName,
                request.ClientPhone,
                request.ClientEmail,
                request.SiteAnalysis);

            return CreatedAtAction(nameof(GetProposal), new { id = proposal.Id }, proposal);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Proposal>> GetProposal(Guid id)
    {
        try
        {
            var proposal = await _proposalService.GetProposalByIdAsync(id);
            return Ok(proposal);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Proposal>>> GetProposals()
    {
        try
        {
            var proposals = await _proposalService.GetAllProposalsAsync();
            return Ok(proposals);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Proposal>> UpdateProposal(Guid id, [FromBody] Proposal proposal)
    {
        if (id != proposal.Id)
            return BadRequest();

        try
        {
            var updatedProposal = await _proposalService.UpdateProposalAsync(proposal);
            return Ok(updatedProposal);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProposal(Guid id)
    {
        try
        {
            await _proposalService.DeleteRawProposalAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class CreateProposalRequest
{
    public string PropertyType { get; set; } = string.Empty;
    public decimal PropertySize { get; set; }
    public string Region { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public List<string> RequestedServices { get; set; } = new();
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string SiteAnalysis { get; set; } = string.Empty;
    
} 