using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RemodelingProposalSystem.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly ApplicationDbContext _context;

    public MaterialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Material>> GetAllAsync()
    {
        return await _context.Materials.ToListAsync();
    }

    public async Task<Material?> GetByIdAsync(string id)
    {
        return await _context.Materials.FindAsync(id);
    }

    public async Task<Material?> GetByNameAsync(string name)
    {
        return await _context.Materials
            .FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());
    }

    public async Task<Material> AddAsync(Material material)
    {
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();
        return material;
    }

    public async Task<Material> UpdateAsync(Material material)
    {
        _context.Entry(material).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return material;
    }

    public async Task DeleteAsync(string id)
    {
        var material = await _context.Materials.FindAsync(id);
        if (material != null)
        {
            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<Material>> GetMaterialsByServicesAsync(List<string> requestedServices)
    {
        // get materials needed
        var materials = new List<Material>
                {
                    new Material() { Id="1", Name="Shrubs", UnitPrice=1000, Quantity=100},
                    new Material() { Id="2", Name="Turf grass", UnitPrice=1000, Quantity=100},
                    new Material() { Id="3", Name="Rubber mulch", UnitPrice=1000, Quantity = 100},
                    new Material() { Id="4", Name="Sand ", UnitPrice=1000, Quantity = 100},
                };

        return Task.FromResult<IEnumerable<Material>>(materials);
    }
} 