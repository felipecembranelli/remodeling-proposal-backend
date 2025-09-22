using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RemodelingProposalSystem.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        return await _context.Services
            .Include(s => s.RequiredMaterials)
            .ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(string id)
    {
        return await _context.Services
            .Include(s => s.RequiredMaterials)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Service?> GetByNameAsync(string name)
    {
        return await _context.Services
            .Include(s => s.RequiredMaterials)
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<Service> AddAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task UpdateAsync(Service service)
    {
        var existingService = await _context.Services
            .Include(s => s.RequiredMaterials)
            .FirstOrDefaultAsync(s => s.Id == service.Id);

        if (existingService == null)
        {
            throw new KeyNotFoundException($"Service with ID {service.Id} not found.");
        }

        // Update basic properties
        existingService.Name = service.Name;
        existingService.Description = service.Description;
        existingService.BasePrice = service.BasePrice;
        existingService.UnitPrice = service.UnitPrice;
        existingService.Quantity = service.Quantity;
        existingService.TotalCost = service.TotalCost;
        existingService.UnitOfMeasure = service.UnitOfMeasure;
        existingService.EstimatedDuration = service.EstimatedDuration;
        existingService.PropertyTypes = service.PropertyTypes;
        existingService.Regions = service.Regions;

        // Update required materials
        _context.Materials.RemoveRange(existingService.RequiredMaterials);
        existingService.RequiredMaterials = service.RequiredMaterials;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var service = await _context.Services
            .Include(s => s.RequiredMaterials)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null)
        {
            throw new KeyNotFoundException($"Service with ID {id} not found.");
        }

        _context.Materials.RemoveRange(service.RequiredMaterials);
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }

    public Task<IEnumerable<Service>> GetRequestedServicesDetailsAsync(List<string> requestedServices)
    {

        var services = new List<Service>();

        foreach (var service in requestedServices)
        {
            services.Add(new Service() { Name = service, TotalCost = 1000 });
        }

        return Task.FromResult<IEnumerable<Service>>(services);

    }
} 