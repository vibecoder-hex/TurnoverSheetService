using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TurnoverSheetService;

public partial class TurnoverSheetDbContext : DbContext
{
    public TurnoverSheetDbContext(DbContextOptions<TurnoverSheetDbContext> options)
        : base(options)
    {
        
    }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Charge>(entity =>
        {
            entity.HasKey(e => e.ChargeUuid).HasName("charges_pkey");

            entity.ToTable("charges");

            entity.HasIndex(e => new { e.ApartmentNumber, e.ChargeDate }, "charges_unq").IsUnique();

            entity.Property(e => e.ChargeUuid)
                .ValueGeneratedNever()
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

            entity.ToTable("payments");

            entity.HasIndex(e => new { e.PaymentDate, e.ApartmentNumber }, "payments_unq").IsUnique();

            entity.Property(e => e.PaymentUuid)
                .ValueGeneratedNever()
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

            entity.ToTable("saldo");

            entity.HasIndex(e => new { e.ApartmentNumber, e.PaymentDate }, "saldo_unq").IsUnique();

            entity.Property(e => e.SaldoUuid)
                .ValueGeneratedNever()
                .HasColumnName("saldoUUID");
            entity.Property(e => e.ApartmentNumber).HasColumnName("apartmentNumber");
            entity.Property(e => e.IncomingSaldo).HasColumnName("incomingSaldo");
            entity.Property(e => e.OutcomingSaldo).HasColumnName("outcomingSaldo");
            entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");
        });
    }
}
