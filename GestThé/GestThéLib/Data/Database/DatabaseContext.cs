using System;
using System.Collections.Generic;
using GestThéLib.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace GestThéLib.Data.Database;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TCountry> TCountries { get; set; }

    public virtual DbSet<TField> TFields { get; set; }

    public virtual DbSet<TList> TLists { get; set; }

    public virtual DbSet<TProvider> TProviders { get; set; }

    public virtual DbSet<TRegion> TRegions { get; set; }

    public virtual DbSet<TTea> TTeas { get; set; }

    public virtual DbSet<TType> TTypes { get; set; }

    public virtual DbSet<TVariety> TVarieties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=gestTea.db;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TCountry>(entity =>
        {
            entity.HasKey(e => e.IdCountry);

            entity.ToTable("t_country");

            entity.Property(e => e.IdCountry).HasColumnName("idCountry");
            entity.Property(e => e.CountryName).HasColumnName("countryName");
        });

        modelBuilder.Entity<TField>(entity =>
        {
            entity.HasKey(e => e.IdField);

            entity.ToTable("t_field");

            entity.Property(e => e.IdField).HasColumnName("idField");
            entity.Property(e => e.FieldName).HasColumnName("fieldName");
        });

        modelBuilder.Entity<TList>(entity =>
        {
            entity.HasKey(e => e.IdList);

            entity.ToTable("t_list");

            entity.Property(e => e.IdList).HasColumnName("idList");
            entity.Property(e => e.ListAddDate)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME")
                .HasColumnName("listAddDate");
            entity.Property(e => e.ListDescription).HasColumnName("listDescription");
            entity.Property(e => e.ListModificationDate)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME")
                .HasColumnName("listModificationDate");
            entity.Property(e => e.ListName).HasColumnName("listName");

            entity.HasMany(d => d.IdFields).WithMany(p => p.IdLists)
                .UsingEntity<Dictionary<string, object>>(
                    "TDefined",
                    r => r.HasOne<TField>().WithMany()
                        .HasForeignKey("IdField")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<TList>().WithMany()
                        .HasForeignKey("IdList")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdList", "IdField");
                        j.ToTable("t_defined");
                        j.IndexerProperty<long>("IdList").HasColumnName("idList");
                        j.IndexerProperty<long>("IdField").HasColumnName("idField");
                    });
        });

        modelBuilder.Entity<TProvider>(entity =>
        {
            entity.HasKey(e => e.IdProvider);

            entity.ToTable("t_provider");

            entity.Property(e => e.IdProvider).HasColumnName("idProvider");
            entity.Property(e => e.ProviderName).HasColumnName("providerName");
        });

        modelBuilder.Entity<TRegion>(entity =>
        {
            entity.HasKey(e => e.IdRegion);

            entity.ToTable("t_region");

            entity.Property(e => e.IdRegion).HasColumnName("idRegion");
            entity.Property(e => e.IdCountry).HasColumnName("idCountry");
            entity.Property(e => e.RegionName).HasColumnName("regionName");

            entity.HasOne(d => d.IdCountryNavigation).WithMany(p => p.TRegions)
                .HasForeignKey(d => d.IdCountry)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TTea>(entity =>
        {
            entity.HasKey(e => e.IdTea);

            entity.ToTable("t_tea");

            entity.Property(e => e.IdTea).HasColumnName("idTea");
            entity.Property(e => e.IdProvider).HasColumnName("idProvider");
            entity.Property(e => e.IdRegion).HasColumnName("idRegion");
            entity.Property(e => e.IdType).HasColumnName("idType");
            entity.Property(e => e.IdVariety).HasColumnName("idVariety");
            entity.Property(e => e.TeaAddDate)
                .HasColumnType("NUMERIC")
                .HasColumnName("teaAddDate");
            entity.Property(e => e.TeaDescription).HasColumnName("teaDescription");
            entity.Property(e => e.TeaIsArchived)
                .HasColumnType("NUMERIC")
                .HasColumnName("teaIsArchived");
            entity.Property(e => e.TeaModificationDate)
                .HasColumnType("NUMERIC")
                .HasColumnName("teaModificationDate");
            entity.Property(e => e.TeaName).HasColumnName("teaName");
            entity.Property(e => e.TeaPrice)
                .HasColumnType("NUMERIC(15,2)")
                .HasColumnName("teaPrice");
            entity.Property(e => e.TeaQuantity).HasColumnName("teaQuantity");
            entity.Property(e => e.TeaYear).HasColumnName("teaYear");

            entity.HasOne(d => d.IdProviderNavigation).WithMany(p => p.TTeas)
                .HasForeignKey(d => d.IdProvider)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdRegionNavigation).WithMany(p => p.TTeas)
                .HasForeignKey(d => d.IdRegion)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.TTeas)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdVarietyNavigation).WithMany(p => p.TTeas)
                .HasForeignKey(d => d.IdVariety)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.IdLists).WithMany(p => p.IdTeas)
                .UsingEntity<Dictionary<string, object>>(
                    "TContain",
                    r => r.HasOne<TList>().WithMany()
                        .HasForeignKey("IdList")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<TTea>().WithMany()
                        .HasForeignKey("IdTea")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("IdTea", "IdList");
                        j.ToTable("t_contains");
                        j.IndexerProperty<long>("IdTea").HasColumnName("idTea");
                        j.IndexerProperty<long>("IdList").HasColumnName("idList");
                    });
        });

        modelBuilder.Entity<TType>(entity =>
        {
            entity.HasKey(e => e.IdType);

            entity.ToTable("t_type");

            entity.Property(e => e.IdType).HasColumnName("idType");
            entity.Property(e => e.TypeName).HasColumnName("typeName");
        });

        modelBuilder.Entity<TVariety>(entity =>
        {
            entity.HasKey(e => e.IdVariety);

            entity.ToTable("t_variety");

            entity.Property(e => e.IdVariety).HasColumnName("idVariety");
            entity.Property(e => e.VarietyName).HasColumnName("varietyName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
