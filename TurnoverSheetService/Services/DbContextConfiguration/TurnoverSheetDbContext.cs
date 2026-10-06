using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace TurnoverSheetService.Services.DbContextConfiguration;

public partial class TurnoverSheetDbContext : DbContext
{
    public TurnoverSheetDbContext(DbContextOptions<TurnoverSheetDbContext> options)
        : base(options)
    {
        
    }

    public virtual DbSet<Charge> Charges { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Saldo> Saldos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Charge>(entity =>
        {
            entity.HasKey(e => e.ChargeUuid).HasName("charges_pkey");

            entity.HasIndex(e => new { e.ApartmentNumber, e.ChargeDate})
                .HasName("idx_charges")
                .HasMethod("btree");

            entity.ToTable("charges");

            entity.Property(e => e.ChargeUuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>()
                .HasColumnName("chargeUUID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ApartmentNumber).HasColumnName("apartmentNumber");
            entity.Property(e => e.ChargeDate).HasColumnName("chargeDate");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentUuid).HasName("payments_pkey");
            
            entity.HasIndex(e => new { e.ApartmentNumber, e.PaymentDate })
                .HasName("idx_payment")
                .HasMethod("btree");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentUuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>()
                .HasColumnName("paymentUUID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ApartmentNumber).HasColumnName("apartmentNumber");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
            entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");
        });

        modelBuilder.Entity<Saldo>(entity =>
        {
            entity.HasKey(e => e.SaldoUuid).HasName("saldoId_pk");
            
            entity.HasIndex(e => new { e.ApartmentNumber, e.PaymentDate})
                .HasName("idx_saldo")
                .HasMethod("btree");

            entity.ToTable("saldo");

            entity.Property(e => e.SaldoUuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>()
                .HasColumnName("saldoUUID");
            entity.Property(e => e.ApartmentNumber).HasColumnName("apartmentNumber");
            entity.Property(e => e.IncomingSaldo).HasColumnName("incomingSaldo");
            entity.Property(e => e.OutcomingSaldo).HasColumnName("outcomingSaldo");
            entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
        });
    }
}
