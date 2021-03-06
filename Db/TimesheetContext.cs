using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Timesheet.Common;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Db
{
    public partial class TimesheetContext : DbContext
    {
        private ILoggerFactory _loggerFactory;
        public TimesheetContext()
        {
        }

        public TimesheetContext(DbContextOptions<TimesheetContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public virtual DbSet<Finance> Finance { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Section> Section { get; set; }
        public virtual DbSet<Common.Timesheet> Timesheet { get; set; }
        public virtual DbSet<RewardSummary> RewardSummary { get; set; }
        public virtual DbSet<DocumentStorage> DocumentStorage { get; set; }
        public virtual DbSet<PaymentItem> PaymentItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:Timesheet");
            }

#if DEBUG
            optionsBuilder.UseLoggerFactory(_loggerFactory)  //tie-up DbContext with LoggerFactory object
            .EnableSensitiveDataLogging();
#endif
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
                    .HasColumnName("PaymentXML");
                //.HasColumnType("xml");

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

                entity.HasOne(d => d.PaidFrom)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.PaidFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Finance");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_Section");
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

            modelBuilder.Entity<Common.Timesheet>(entity =>
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

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Timesheet)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timesheet_Job");

                entity.HasOne(d => d.PaymentItem)
                    .WithMany(p => p.Timesheet)
                    .HasForeignKey(d => d.PaymentItemId)
                    .HasConstraintName("FK_Timesheet_PaymentItem");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Timesheet)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timesheet_Person");
            });
            modelBuilder.Entity<RewardSummary>(entity =>
            {
                entity.ToView("RewardSummary");
                entity.HasOne(r => r.Person)
                    .WithMany()
                    .HasForeignKey(r => r.PersonId);
            });
            modelBuilder.Entity<DocumentStorage>(entity =>
            {
                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.DocumentName).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });
            modelBuilder.Entity<PaymentItem>(entity =>
            {
                entity.Property(e => e.Hours).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.Reward).HasColumnType("decimal(19, 2)");

                entity.Property(e => e.Tax).HasColumnType("decimal(19, 2)");

                entity.Property(e => e.RewardToPay).HasColumnType("decimal(19, 2)");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PaymentItem)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_PaymentItem_Payment");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PaymentItem)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentItem_Person");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

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
