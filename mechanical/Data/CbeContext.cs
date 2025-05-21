﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;

//using mechanical.Migrations;
namespace mechanical.Data
{
    public class CbeContext : DbContext
    {
        //production capacity estimation
        public DbSet<PCECase> PCECases { get; set; }
        public DbSet<ProductionCapacity> ProductionCapacities { get; set; }
        public DbSet<PCEEvaluation> PCEEvaluations { get; set; }
        public DbSet<ProductionLine> ProductionLines { get; set; }
        public DbSet<ProductionLineInput> ProductionLineInputs { get; set; }
        public DbSet<Justification> Justifications { get; set; }
        public DbSet<PCECaseAssignment> PCECaseAssignments { get; set; }
        public DbSet<PCECaseTimeLine> PCECaseTimeLines { get; set; }
        public DbSet<PCECaseSchedule> PCECaseSchedules { get; set; }
        public DbSet<PCECaseTerminate> PCECaseTerminates { get; set; }
        public DbSet<ProductionReestimation> ProductionReestimations { get; set; }
        public DbSet<ReturnedProduction> ReturnedProductions { get; set; }
        public DbSet<PCECaseComment> PCECaseComments { get; set; }
        public DbSet<DateTimeRange> DateTimeRanges { get; set; }

        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseAssignment> CaseAssignments { get; set; }
        public DbSet<CollateralReestimation> CollateralReestimations { get; set; }
        public DbSet<CaseTimeLine> CaseTimeLines { get; set; }
        public DbSet<CaseComment> CaseComments { get; set; }
        public DbSet<CaseSchedule> CaseSchedules { get; set; }
        public DbSet<CaseTerminate> CaseTerminates { get; set; }
        public DbSet<ConsecutiveNumber> ConsecutiveNumbers { get; set; }
        public DbSet<Collateral> Collaterals { get; set; }
        public DbSet<ConstMngAgrMachinery> ConstMngAgrMachineries { get; set; }
        public DbSet<IndBldgFacilityEquipment> IndBldgFacilityEquipment { get; set; }
        public DbSet<IndBldgFacilityEquipmentCosts> IndBldgFacilityEquipmentCosts { get; set; }
        public DbSet<MotorVehicle> MotorVehicles { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }

        public virtual DbSet<CreateRole> CreateRoles { get; set; }
        public virtual DbSet<CreateUser> CreateUsers { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Signatures> Signatures { get; set; }
        public virtual DbSet<EmployeeInfoes> Employees { get; set; }
        public virtual DbSet<Correction> Corrections { get; set; }
        public virtual DbSet<Reject> Rejects { get; set; }
        public virtual DbSet<Reject> TaskComment { get; set; }
        public virtual DbSet<Reject> TaskManagment { get; set; }
        public virtual DbSet<Reject> TaskNotification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure CreateUsers self-referencing relationship
            // modelBuilder.Entity<CreateUser>()
            //     .HasOne(u => u.Supervisor)
            //     .WithMany()
            //     .HasForeignKey(u => u.SupervisorId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<Collateral>()
            //     .HasOne(c => c.Case)
            //     .WithMany()
            //     .HasForeignKey(c => c.CaseId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<Signatures>()
            //     .HasOne(s => s.CreateUser)
            //     .WithOne()
            //     .HasForeignKey<Signatures>(s => s.CreateUserId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<PCECase>()
            //     .HasOne(p => p.PCECaseOriginator)
            //     .WithMany()
            //     .HasForeignKey(p => p.PCECaseOriginatorId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<ProductionCapacity>()
            //     .HasOne(p => p.PCECase)
            //     .WithMany()
            //     .HasForeignKey(p => p.PCECaseId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<ProductionLine>()
            //     .Property(pl => pl.Id)
            //     .HasDefaultValueSql("NEWSEQUENTIALID()")
            //     .ValueGeneratedOnAdd();

            modelBuilder.Entity<ProductionLine>()
                .HasOne(pl => pl.PCEEvaluation)
                .WithMany(pe => pe.ProductionLines)
                .HasForeignKey(pl => pl.PCEEvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductionLine>(entity =>
            {
                entity.Property(p => p.ActualCapacity).HasPrecision(18, 2);
                entity.Property(p => p.AttainableCapacity).HasPrecision(18, 2);
                entity.Property(p => p.ConversionRatio).HasPrecision(18, 2);
                entity.Property(p => p.DesignCapacity).HasPrecision(18, 2);
                entity.Property(p => p.TotalInput).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ProductionLineInput>()
                .HasOne(pli => pli.ProductionLine)
                .WithMany(pl => pl.ProductionLineInputs)
                .HasForeignKey(pli => pli.ProductionLineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductionLineInput>(entity =>
            {
                entity.Property(p => p.Quantity).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Signatures>()
                .HasOne(c => c.SignatureFile)
                .WithOne()
                .HasForeignKey<Signatures>(s => s.SignatureFileId);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                var idProperty = entity.Property("Id");
                if (idProperty != null && idProperty.Metadata.ClrType == typeof(Guid))
                {
                    idProperty.HasDefaultValueSql("NEWID()");
                    idProperty.ValueGeneratedOnAdd();
                }
            }

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<PCEEvaluation>(entity =>
            {
                entity.Property(e => e.InspectionDate).HasConversion(dateOnlyConverter);
            });

            base.OnModelCreating(modelBuilder);
        }
        public CbeContext(DbContextOptions<CbeContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //       => optionsBuilder.UseSqlServer("Server=DESKTOP-OJQ5A2C\\DOMAIN;Database=mechanical;Trusted_Connection=True;TrustServerCertificate=true;");
    }
}
