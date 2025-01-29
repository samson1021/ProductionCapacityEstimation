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
            //using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetService<CbeContext>();
            //    if (!context.CreateRoles.Any())
            //    {
            //        context.CreateRoles.AddRange(new List<CreateRole>()
            //        {
            //            new CreateRole()
            //            {
            //                Name = "Maker Manager"
            //            },
            //            new CreateRole()
            //            {
            //                Name = "Maker Officer"
            //            },
            //            new CreateRole() {
            //                 Name = "Relation Manager"
            //            },
            //            new CreateRole()
            //            {
            //                Name = "Checker Manager"
            //            },
            //            new CreateRole()
            //            {
            //                Name = "Checker Officer"
            //            },
            //            new CreateRole() {
            //                 Name = "Maker TeamLeader"
            //            },
            //            new CreateRole()
            //            {
            //                Name = "Checker TeamLeader"
            //            }
            //            ,
            //            new CreateRole()
            //            {
            //                Name = "Higher Official"
            //            },
            //            new CreateRole()
            //            {
            //                Name = "District Valuation Manager"
            //            },
            //            new CreateRole()
            //            {
            //                Name="Admin"
            //            }
            //            ,
            //            new CreateRole()
            //            {
            //                Name="Super Admin"
            //            }
            //        });
            //        context.SaveChanges();
            //    }
            //}
        }
    }
}