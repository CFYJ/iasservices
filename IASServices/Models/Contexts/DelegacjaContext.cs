using Microsoft.EntityFrameworkCore;
using IASServices.Models;

namespace SyncWebIAS.Model
{
    public partial class DelegacjaContext : DbContext
    {
        public virtual DbSet<Delegacja> Delegacja { get; set; }

        //public DelegacjaContext()
        //{ }
        public DelegacjaContext(DbContextOptions<DelegacjaContext> options): base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Delegacja>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cel)
                    .HasColumnName("cel")
                    .HasColumnType("varchar(2000)");

                entity.Property(e => e.CzasDo)
                    .HasColumnName("czas_do")
                    .HasColumnType("datetime");

                entity.Property(e => e.CzasOd)
                    .HasColumnName("czas_od")
                    .HasColumnType("datetime");

                entity.Property(e => e.DataWystawienia)
                    .HasColumnName("data_wystawienia")
                    .HasColumnType("datetime");

                entity.Property(e => e.Delegowany)
                    .HasColumnName("delegowany")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.IdDelegowanego).HasColumnName("id_delegowanego");

                entity.Property(e => e.IdWystawcy).HasColumnName("id_wystawcy");

                entity.Property(e => e.Miejscowosc)
                    .HasColumnName("miejscowosc")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Nr)
                    .HasColumnName("nr")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Srodek)
                    .HasColumnName("srodek")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Wydzial)
                    .HasColumnName("_wydzial")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Wystawil)
                    .HasColumnName("wystawil")
                    .HasColumnType("varchar(255)");
            });
        }

        public DbSet<IASServices.Models.Kontakty> Kontakty { get; set; }
    }
}