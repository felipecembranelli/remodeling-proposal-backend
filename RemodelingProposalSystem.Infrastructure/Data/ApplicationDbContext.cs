using RemodelingProposalSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using static OpenAI.ObjectModels.SharedModels.IOpenAiModels;
using System;

namespace RemodelingProposalSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<RegionalPricing> RegionalPricing { get; set; }
    public DbSet<PropertyTypePricing> PropertyTypePricing { get; set; }
    public DbSet<SeasonalPricing> SeasonalPricing { get; set; }
    public DbSet<LaborPricing> LaborPricing { get; set; }
    public DbSet<MaterialBasePricing> MaterialBasePricing { get; set; }
    public DbSet<ServiceBasePricing> ServiceBasePricing { get; set; }
    public DbSet<PriceHistory> PriceHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PropertyType).IsRequired();
            entity.Property(e => e.Region).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            //entity.Property(e => e.Description).IsRequired();
            //entity.Property(e => e.TotalCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.UnitOfMeasure).IsRequired();
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.UnitOfMeasure).IsRequired();
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PropertyType).IsRequired();
            entity.Property(e => e.PropertySize).IsRequired();
            entity.Property(e => e.Region).IsRequired();
            entity.Property(e => e.ProposalRawBody).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Budget).IsRequired();
            entity.Property(e => e.ClientEmail).IsRequired();
            entity.Property(e => e.ClientPhone).IsRequired();
            entity.Property(e => e.ClientName).IsRequired();
            //entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.RequestedServices).IsRequired();
            //entity.Property(e => e.ValidUntil).IsRequired();
        });

        modelBuilder.Entity<RegionalPricing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Region).IsRequired();
            entity.Property(e => e.Multiplier).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PropertyTypePricing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PropertyType).IsRequired();
            entity.Property(e => e.Multiplier).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SeasonalPricing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Season).IsRequired();
            entity.Property(e => e.Multiplier).HasPrecision(18, 2);
        });

        modelBuilder.Entity<LaborPricing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceType).IsRequired();
            entity.Property(e => e.BaseRate).HasPrecision(18, 2);
        });

        modelBuilder.Entity<MaterialBasePricing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaterialId).IsRequired();
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ServiceBasePricing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceId).IsRequired();
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemId).IsRequired();
            entity.Property(e => e.ItemType).IsRequired();
            entity.Property(e => e.OldPrice).HasPrecision(18, 2);
            entity.Property(e => e.NewPrice).HasPrecision(18, 2);
        });

        // Configure relationships
        //modelBuilder.Entity<RawProposal>()
        //    .HasMany(p => p.Services)
        //    .WithOne()
        //    .HasForeignKey("ProposalId")
        //    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Service>()
            .HasMany(s => s.RequiredMaterials)
            .WithOne()
            .HasForeignKey("ServiceId")
            .OnDelete(DeleteBehavior.Cascade);
    }
} 