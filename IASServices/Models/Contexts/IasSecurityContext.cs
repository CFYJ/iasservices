using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IASServices.Models
{
    public partial class IasSecurityContext : DbContext
    {
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Roleuzytkownika> Roleuzytkownika { get; set; }

        public IasSecurityContext(DbContextOptions<IasSecurityContext> options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Rola)
                    .HasColumnName("rola")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Modul)
                    .HasColumnName("modul")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Opis)
                    .HasColumnName("opis")
                    .HasColumnType("varchar(300)");
            });

            modelBuilder.Entity<Roleuzytkownika>(entity =>
            {
                entity.ToTable("roleuzytkownika");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdRoli).HasColumnName("id_roli");

                entity.Property(e => e.IdUzytkownika).HasColumnName("id_uzytkownika");
            });
        }
    }
}