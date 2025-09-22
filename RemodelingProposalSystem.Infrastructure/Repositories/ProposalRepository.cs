using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RemodelingProposalSystem.Infrastructure.Repositories;

public class ProposalRepository : IProposalRepository
{
    private readonly ApplicationDbContext _context;

    public ProposalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Proposal>> GetAllAsync()
    {
        return await _context.Proposals
            .ToListAsync();
    }

    public async Task<Proposal?> GetByIdAsync(string id)
    {
        return await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id.ToString() == id);
    }

    public async Task<Proposal> AddAsync(Proposal proposal)
    {
        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();
        return proposal;
    }

    public async Task<Proposal> UpdateAsync(Proposal proposal)
    {
        var existingProposal = await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id == proposal.Id);

        if (existingProposal == null)
        {
            throw new KeyNotFoundException($"Proposal with ID {proposal.Id} not found.");
        }

        // Update basic properties
        existingProposal.PropertyType = proposal.PropertyType;
        existingProposal.Region = proposal.Region;
        existingProposal.PropertySize = proposal.PropertySize;
        existingProposal.ProposalRawBody = proposal.ProposalRawBody;
        existingProposal.Status = proposal.Status;
        existingProposal.Budget = proposal.Budget;

        await _context.SaveChangesAsync();
        return existingProposal;
    }

    public async Task DeleteAsync(string id)
    {
        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id.ToString() == id);

        if (proposal == null)
        {
            throw new KeyNotFoundException($"Proposal with ID {id} not found.");
        }

        _context.Proposals.Remove(proposal);
        await _context.SaveChangesAsync();
    }
} 