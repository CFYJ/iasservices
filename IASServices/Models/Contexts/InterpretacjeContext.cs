using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IASServices.Models
{
    public partial class InterpretacjeContext : DbContext
    {
        public virtual DbSet<InterFiles> InterFiles { get; set; }

        public InterpretacjeContext(DbContextOptions<InterpretacjeContext> options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //optionsBuilder.UseSqlServer(@"Data Source=WKS-2891-175\SQLEXPRESS;Initial Catalog=IAS;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}