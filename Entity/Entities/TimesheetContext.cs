using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class TimesheetContext : DbContext
    {
        public TimesheetContext()
        {
        }

        public TimesheetContext(DbContextOptions<TimesheetContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Finance> Finance { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<RewardSummary> RewardSummary { get; set; }
        public virtual DbSet<Section> Section { get; set; }
        public virtual DbSet<Timesheet> Timesheet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:Timesheet");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Finance>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.HourReward).HasColumnType("decimal(19, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.PaymentDateTime).HasColumnType("datetime");

                entity.Property(e => e.PaymentXml)
                    .HasColumnName("PaymentXML")
                    .HasColumnType("xml");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.BankAccount).HasMaxLength(50);

                entity.Property(e => e.BankCode).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.DateBirth).HasColumnType("date");

                entity.Property(e => e.HourReward).HasColumnType("money");

                entity.Property(e => e.HouseNumber).HasMaxLength(10);

                entity.Property(e => e.IdentityDocument).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(5)
                    .IsFixedLength();

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.Street).HasMaxLength(50);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Job");

                entity.HasOne(d => d.PayedFrom)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.PayedFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Finance");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Section");
            });

            modelBuilder.Entity<RewardSummary>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RewardSummary");

                entity.Property(e => e.Hours).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.PaymentDateTime).HasColumnType("datetime");

                entity.Property(e => e.Reward).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.Tax).HasColumnType("decimal(38, 2)");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Timesheet>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.DateTimeFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTimeTo).HasColumnType("datetime");

                entity.Property(e => e.Hours).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Reward).HasColumnType("decimal(19, 2)");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Tax).HasColumnType("decimal(19, 2)");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Timesheet)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timesheet_Job");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Timesheet)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Timesheet_Payment");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Timesheet)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timesheet_Person");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
