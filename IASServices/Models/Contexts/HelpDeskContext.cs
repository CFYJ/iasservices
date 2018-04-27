using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IASServices.Models
{
    public partial class HelpDeskContext : DbContext
    {
        public virtual DbSet<HelpDeskInfo> HelpDeskInfo { get; set; }

        public HelpDeskContext(DbContextOptions<HelpDeskContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HelpDeskInfo>(entity =>
            {
                entity.Property(e => e.Data)
                    .HasComputedColumnSql("getdate()")
                    .ValueGeneratedOnAddOrUpdate();
            });
        }
    }
}