using Microsoft.EntityFrameworkCore;
using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.DocManager.Entities
{
    public partial class DocumentContext : DbContext
    {
        public DocumentContext()
        {
        }

        public DocumentContext(DbContextOptions<DocumentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DocumentStorage> DocumentStorage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:Document");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentStorage>(entity =>
            {
                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValue<DateTime>(DateTime.Now);

                entity.Property(e => e.DocumentName).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
