using Microsoft.EntityFrameworkCore;

namespace IASServices.Models
{
    public partial class KontaktyContext : DbContext
    {
        public virtual DbSet<Kontakty> Kontakty { get; set; }

        //public KontaktyContext()
        //{ }

        public KontaktyContext(DbContextOptions<KontaktyContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kontakty>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Imie)
                    .HasColumnName("imie")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Jednostka)
                    .HasColumnName("jednostka")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Komorka)
                    .HasColumnName("komorka")
                    .HasColumnType("numeric");

                entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Nazwisko)
                    .HasColumnName("nazwisko")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Pokoj)
                    .HasColumnName("pokoj")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Stanowisko)
                    .HasColumnName("stanowisko")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Telefon)
                    .HasColumnName("telefon")
                    .HasColumnType("numeric");

                entity.Property(e => e.Wewnetrzny).HasColumnName("wewnetrzny");

                entity.Property(e => e.Wydzial)
                    .HasColumnName("wydzial")
                    .HasColumnType("varchar(255)");
            });
        }
    }
}