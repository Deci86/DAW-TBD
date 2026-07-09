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

    public virtual DbSet<AuditoriaTransaccion> AuditoriasTransacciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.email)
                .IsUnique();

            entity.Property(e => e.pass_hash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.promedio_calificacion_comprador)
                .HasColumnType("decimal(3, 2)")
                .HasDefaultValue(0m);

            entity.Property(e => e.calificacion_vendedor)
                .HasColumnType("decimal(3, 2)")
                .HasDefaultValue(0m);

            entity.Property(e => e.fecha_registro)
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

            entity.HasOne(e => e.Cliente)
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

            entity.HasOne(e => e.Oferta)
                .WithMany()
                .HasForeignKey(e => e.oferta_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaccion_Oferta");

            entity.HasOne(e => e.ClienteComprador)
                .WithMany()
                .HasForeignKey(e => e.cliente_comprador_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaccion_ClienteComprador");
        });

        modelBuilder.Entity<AuditoriaTransaccion>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.id)
                .HasColumnName("id");

            entity.Property(e => e.transaccion_id)
                .HasColumnName("transaccion_id");

            entity.Property(e => e.usuario_id)
                .HasColumnName("usuario_id");

            entity.Property(e => e.accion)
                .IsRequired()
                .HasColumnName("accion")
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.estado_anterior)
                .HasColumnName("estado_anterior")
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.estado_nuevo)
                .IsRequired()
                .HasColumnName("estado_nuevo")
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.fecha_accion)
                .HasColumnName("fecha_accion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Transaccion)
                .WithMany()
                .HasForeignKey(e => e.transaccion_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_AuditoriaTransaccion_Transaccion");

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.usuario_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_AuditoriaTransaccion_Clientes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
