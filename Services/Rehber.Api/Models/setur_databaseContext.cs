﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace Rehber.Api.Models
{
    public partial class setur_databaseContext : DbContext
    {
        public setur_databaseContext()
        {
        }

        public setur_databaseContext(DbContextOptions<setur_databaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Iletisimbilgileri> Iletisimbilgileris { get; set; }
        public virtual DbSet<Kisiler> Kisilers { get; set; }
        public virtual DbSet<Raporlar> Raporlars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()                 
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
                
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("SeturConnection"));
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Iletisimbilgileri>(entity =>
            {
                entity.HasKey(e => e.IletisimbilgiId)
                    .HasName("iletisimbilgileri_pkey");

                entity.ToTable("iletisimbilgileri");

                entity.Property(e => e.IletisimbilgiId)
                    .HasColumnName("iletisimbilgi_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.BilgiTipi).HasColumnName("bilgi_tipi");

                entity.Property(e => e.Icerik)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("icerik");

                entity.Property(e => e.KisiId).HasColumnName("kisi_id");

                entity.HasOne(d => d.Kisi)
                    .WithMany(p => p.Iletisimbilgileris)
                    .HasForeignKey(d => d.KisiId)
                    .HasConstraintName("iletisimbilgileri_kisi_id_fkey");
            });

            modelBuilder.Entity<Kisiler>(entity =>
            {
                entity.HasKey(e => e.KisiId)
                    .HasName("kisiler_pkey");

                entity.ToTable("kisiler");

                entity.Property(e => e.KisiId)
                    .HasColumnName("kisi_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Ad)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("ad");

                entity.Property(e => e.Firma)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("firma");

                entity.Property(e => e.Soyad)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("soyad");
            });

            modelBuilder.Entity<Raporlar>(entity =>
            {
                entity.HasKey(e => e.RaporId)
                    .HasName("raporlar_pkey");

                entity.ToTable("raporlar");

                entity.Property(e => e.RaporId)
                    .HasColumnName("rapor_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Durum).HasColumnName("durum");

                entity.Property(e => e.Icerik)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("icerik");

                entity.Property(e => e.Tarih)
                    .HasColumnName("tarih")
                    .HasDefaultValueSql("now()");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
