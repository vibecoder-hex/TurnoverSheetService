using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models.DatabaseEntites;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace TurnoverSheetService.Services.DbContextConfiguration;

public partial class TurnoverSheetDbContext : DbContext
{
    public TurnoverSheetDbContext(DbContextOptions<TurnoverSheetDbContext> options)
        : base(options)
    {
        
    }
    
    public virtual DbSet<Apartment> Apartments { get; set; }
    public virtual DbSet<Charge> Charges { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Saldo> Saldos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.ApartmentGuid).HasName("apartment_pkey");
            
            entity.HasIndex(e => e.Number).IsUnique();
            
            entity.ToTable("apartments");
            
            entity.Property(e => e.Number)
                .HasColumnName("apartmentUuid");
            entity.Property(e => e.Number)
                .HasColumnName("number");
            entity.Property(e => e.Address)
                .HasMaxLength(120)
                .HasColumnName("address");
            
        });
        modelBuilder.Entity<Charge>(entity =>
        {
            entity.HasKey(e => e.ChargeUuid).HasName("charges_pkey");

            entity.ToTable("charges");

            entity.Property(e => e.ChargeUuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>()
                .HasColumnName("chargeUUID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
            
            entity.HasOne(d => d.Saldo)
                .WithMany(p => p.Charges)
                .HasForeignKey(d => d.SaldoUuid)
                .HasConstraintName("charges_saldo_fk");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentUuid).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentUuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>()
                .HasColumnName("paymentUUID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
            
            entity.HasOne(d => d.Saldo)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.SaldoUuid)
                .HasConstraintName("payments_appartment_fk");
        });

        modelBuilder.Entity<Saldo>(entity =>
        {
            entity.HasKey(e => e.SaldoUuid).HasName("saldoId_pk");
            
            entity.HasIndex(e => new {e.ApartmentUuid, e.CalculatingDate, e.Description })
                .HasName("idx_saldo_unq")
                .IsUnique();

            entity.ToTable("saldo");

            entity.Property(e => e.SaldoUuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>()
                .HasColumnName("saldoUUID");
            entity.Property(e => e.CurrentSaldoValue).HasColumnName("сurrentSaldoValue");
            entity.Property(e => e.CalculatingDate).HasColumnName("calculatingDate");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
            
            entity.HasOne(d => d.Apartment)
                .WithMany(p => p.Saldos)
                .HasForeignKey(d => d.ApartmentUuid)
                .HasConstraintName("saldo_apartment_fk");
        });
    }
}
