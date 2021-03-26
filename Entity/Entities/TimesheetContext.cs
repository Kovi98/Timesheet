using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

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

        public virtual DbSet<Finance> Finances { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<RewardSummary> RewardSummaries { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Timesheet> Timesheets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:Timesheet");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Czech_CI_AS");

            modelBuilder.Entity<Finance>(entity =>
            {
                entity.ToTable("Finance");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");

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
                entity.ToTable("Payment");

                entity.Property(e => e.CreateDateTime).HasColumnType("datetime");

                entity.Property(e => e.Payment1)
                    .HasColumnType("xml")
                    .HasColumnName("Payment");

                entity.Property(e => e.PaymentDateTime).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

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
                    .IsFixedLength(true);

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
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Job");

                entity.HasOne(d => d.PayedFrom)
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.PayedFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Finance");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.People)
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
                entity.ToTable("Section");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Timesheet>(entity =>
            {
                entity.ToTable("Timesheet");

                entity.Property(e => e.CreateDateTime).HasColumnType("datetime");

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
                    .WithMany(p => p.Timesheets)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timesheet_Job");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Timesheets)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Timesheet_Payment");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Timesheets)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timesheet_Person");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
