using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IASServices.Models
{
    public partial class RejestrBWIPContext : DbContext
    {
        public virtual DbSet<Pliki> Pliki { get; set; }
        public virtual DbSet<Sprawa> Sprawa { get; set; }
        public virtual DbSet<Zdarzenia> Zdarzenia { get; set; }

        public RejestrBWIPContext(DbContextOptions<RejestrBWIPContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}