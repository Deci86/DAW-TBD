using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Data;

public partial class SmallChangeDbContext : DbContext
{
    public SmallChangeDbContext()
    {
    }

    public SmallChangeDbContext(DbContextOptions<SmallChangeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Oferta> Ofertas { get; set; }

    public virtual DbSet<Transaccion> Transacciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PassHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.PromedioCalificacionComprador)
                .HasColumnType("decimal(3, 2)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CalificacionVendedor)
                .HasColumnType("decimal(3, 2)")
                .HasDefaultValue(0m);

            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Oferta>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.id)
                .HasColumnName("id");

            entity.Property(e => e.cliente_id)
                .HasColumnName("cliente_id");

            entity.Property(e => e.moneda_a_enviar)
                .IsRequired()
                .HasColumnName("moneda_a_enviar")
                .HasMaxLength(3);

            entity.Property(e => e.moneda_a_recibir)
                .IsRequired()
                .HasColumnName("moneda_a_recibir")
                .HasMaxLength(3);

            entity.Property(e => e.tipo_cambio)
                .HasColumnName("tipo_cambio")
                .HasColumnType("decimal(10, 4)");

            entity.Property(e => e.estado)
                .HasColumnName("estado")
                .HasDefaultValue(true);

            entity.Property(e => e.fecha_creacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(e => e.cliente_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Oferta_Cliente");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.id)
                .HasColumnName("id");

            entity.Property(e => e.oferta_id)
                .HasColumnName("oferta_id");

            entity.Property(e => e.cliente_comprador_id)
                .HasColumnName("cliente_comprador_id");

            entity.Property(e => e.fecha_transaccion)
                .HasColumnName("fecha_transaccion")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.estado)
                .IsRequired()
                .HasColumnName("estado")
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("pendiente");

            entity.HasOne<Oferta>()
                .WithMany()
                .HasForeignKey(e => e.oferta_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaccion_Oferta");

            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(e => e.cliente_comprador_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaccion_ClienteComprador");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
