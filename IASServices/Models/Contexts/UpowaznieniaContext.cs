using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IASServices.Models
{
    public partial class UpowaznieniaContext : DbContext
    {
        public virtual DbSet<Upowaznienia> Upowaznienia { get; set; }
        public virtual DbSet<UpowaznieniaPliki> UpowaznieniaPliki { get; set; }

        public UpowaznieniaContext(DbContextOptions<UpowaznieniaContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Upowaznienia>(entity =>
            //{
            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.Nazwa)
            //        .HasColumnName("nazwa")
            //        .HasColumnType("varchar(250");

            //    entity.Property(e => e.Nazwa_skrocona)
            //       .HasColumnName("nazwa_skrocona")
            //       .HasColumnType("varchar(50)");

            //    entity.Property(e => e.Wniosek_nadania_upr)
            //       .HasColumnName("wniosek_nadania_upr")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Nadajacy_upr)
            //       .HasColumnName("nadajacy_upr")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Prowadzacy_rejestr_uzyt)
            //       .HasColumnName("prowadzacy_rejestr_uzyt")
            //       .HasColumnType("varchar(250)");

            //    entity.Property(e => e.Wniosek_odebrania_upr)
            //       .HasColumnName("wniosek_odebrania_upr")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Odbierajacy_upr)
            //       .HasColumnName("odbierajacy_upr")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Opiekun)
            //       .HasColumnName("opiekun")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Adres_email)
            //       .HasColumnName("adres_email")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Decyzja)
            //       .HasColumnName("decyzja")
            //       .HasColumnType("varchar(150)");

            //    entity.Property(e => e.Uwagi)
            //       .HasColumnName("uwagi")
            //       .HasColumnType("text");
            //});
        }
    }
}