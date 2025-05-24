using mechanical.Models.Entities;
using mechanical.Models;
using mechanical.Data;
using Microsoft.EntityFrameworkCore;
namespace mechanical.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CbeContext>();
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new List<Role>()
                    {
                        new Role()
                        {
                            Name = "Maker Manager"
                        },
                        new Role()
                        {
                            Name = "Maker Officer"
                        },
                        new Role() {
                             Name = "Relation Manager"
                        },
                        new Role()
                        {
                            Name = "Checker Manager"
                        },
                        new Role()
                        {
                            Name = "Checker Officer"
                        },
                        new Role() {
                             Name = "Maker TeamLeader"
                        },
                        new Role()
                        {
                            Name = "Checker TeamLeader"
                        }
                        ,
                        new Role()
                        {
                            Name = "Higher Official"
                        },
                        new Role()
                        {
                            Name = "District Valuation Manager"
                        },
                        new Role()
                        {
                            Name="Admin"
                        }
                        ,
                        new Role()
                        {
                            Name="Super Admin"
                        }
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}