using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.Entities.ProductionCapacity;

//using mechanical.Migrations;

namespace mechanical.Data
{
    public class CbeContext : DbContext

    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Signatures>()
                .HasOne(c => c.SignatureFile)
                .WithOne()
                .HasForeignKey<Signatures>(c => c.SignatureFileId);

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
 
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductionCapacityEstimation>()
                .HasMany(p => p.SupportingDocuments)
                .WithOne(f => f.PCE)
                .HasForeignKey(f => f.PCEId)
                .OnDelete(DeleteBehavior.Cascade);

            // modelBuilder.Entity<ProductionCapacityEstimation>()
            //     .HasMany(e => e.SupportingEvidences)
            //     .WithOne(f => f.PCE)
            //     .HasForeignKey(f => f.PCEId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // modelBuilder.Entity<ProductionCapacityEstimation>()
            //     .HasMany(e => e.ProductionProcessFlowDiagrams)
            //     .WithOne(f => f.PCE)
            //     .HasForeignKey(f => f.PCEId)
            //     .OnDelete(DeleteBehavior.Cascade);          
                
            var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<TimePeriod>(entity =>
            {
                entity.Property(e => e.Start).HasConversion(timeOnlyConverter);
                entity.Property(e => e.End).HasConversion(timeOnlyConverter);
            });

            modelBuilder.Entity<DatePeriod>(entity =>
            {
                entity.Property(e => e.Start).HasConversion(dateOnlyConverter);
                entity.Property(e => e.End).HasConversion(dateOnlyConverter);
            });

            modelBuilder.Entity<ProductionCapacityEstimation>(entity =>
            {
                entity.Property(e => e.InspectionDate).HasConversion(dateOnlyConverter);
            });
        }
        public CbeContext(DbContextOptions<CbeContext> options) : base(options)
        {
        }
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
        public DbSet<MotorVehicle> MotorVehicles { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }

        ///////
        public virtual DbSet<CollateralEstimationFee> CollateralEstimationFees { get; set; }
        public virtual DbSet<ProductionCapacityEstimation> ProductionCapacityEstimations { get; set; }
        public virtual DbSet<FileUpload> FileUploads { get; set; }
        public virtual DbSet<ProductionCapacitySchedule> ProductionCapacitySchedules { get; set; }
        ///////

        public virtual DbSet<CreateRole> CreateRoles { get; set; }
        public virtual DbSet<CreateUser> CreateUsers { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Signatures>  Signatures { get; set; }
        public virtual DbSet<EmployeeInfoes> Employees { get; set; }
        public virtual DbSet<Correction> Corrections { get; set; }
        public virtual DbSet<Reject> Rejects { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //       => optionsBuilder.UseSqlServer("Server=DESKTOP-OJQ5A2C\\DOMAIN;Database=mechanical;Trusted_Connection=True;TrustServerCertificate=true;");

    }
}
