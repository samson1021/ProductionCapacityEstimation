using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Signature_management.Model.Entities;
using System.Reflection.Emit;

namespace Signature_management.Data
{
    public class MyDbContext:DbContext
        {
        protected override void OnModelCreating(ModelBuilder  modelBuilder )
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                var idProperty = entity.Property("Id");
                if (idProperty != null && idProperty.Metadata.ClrType == typeof(Guid))
                {
                    //idProperty.HasDefaultValueSql("NEWID()");
                    idProperty.ValueGeneratedOnAdd();
                }
            }
        }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Acknowledgement> acknowledgements { get; set; }
        public  DbSet<Signatures> signatures { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        //    => optionsBuilder.UseSqlServer("Server=DEVOPS\\SQLEXPRESS;Database=mechanical;Trusted_Connection=false;TrustServerCertificate=true;");

    }
}
