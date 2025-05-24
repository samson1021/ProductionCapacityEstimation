using mechanical.Models.Entities;
using mechanical.Models;
using mechanical.Data;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Data
{
    public class SeedUsersRolesAndDistricts
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CbeContext>();
                context.Database.EnsureCreated(); // Ensure database is created

                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new List<Role>()
                    {
                        new Role() { Name="Super Admin" },
                        new Role() { Name="Admin"},
                        new Role() { Name = "Relation Manager" },
                        new Role() { Name = "District Valuation Manager" },
                        new Role() { Name = "Maker Manager" },
                        new Role() { Name = "Maker TeamLeader" },
                        new Role() { Name = "Maker Officer" },
                        new Role() { Name = "Checker Manager" },
                        new Role() { Name = "Checker TeamLeader" },
                        new Role() { Name = "Checker Officer" },
                        new Role() { Name = "Higher Official" }
                    });
                    context.SaveChanges();
                }

                if (!context.Districts.Any())
                {
                    context.Districts.AddRange(new List<District>()
                    {
                        new District() { Name = "Head Office" },
                        new District() { Name = "Jigjiga" },
                        new District() { Name = "Adama" },
                        new District() { Name = "Dire Dawa" },
                        new District() { Name = "Jimma" },
                        new District() { Name = "Debre Markos" },
                        new District() { Name = "Gullelie" },
                        new District() { Name = "Kality" },
                        new District() { Name = "Hossana" },
                        new District() { Name = "Shire" },
                        new District() { Name = "Bole" },
                        new District() { Name = "Kirkos" },
                        new District() { Name = "Dilla" },
                        new District() { Name = "Nekemte" },
                        new District() { Name = "Shashemene" },
                        new District() { Name = "Yeka" },
                        new District() { Name = "Semera" },
                        new District() { Name = "Hawassa" },
                        new District() { Name = "Nifas Silk" },
                        new District() { Name = "Mettu" },
                        new District() { Name = "Bahir Dar" },
                        new District() { Name = "Dessie" },
                        new District() { Name = "Debre Berehan" },
                        new District() { Name = "Assela" },
                        new District() { Name = "Megenangna" },
                        new District() { Name = "Arada" },
                        new District() { Name = "Gondar" },
                        new District() { Name = "Ambo" },
                        new District() { Name = "Woldia" },
                        new District() { Name = "Mekelle" },
                        new District() { Name = "Merkato" },
                        new District() { Name = "Wolaita Sodo" },
                        new District() { Name = "Kolfe" }
                    });

                    context.SaveChanges();
                }

                //if (!context.Users.Any())
                //{
                //    context.Users.AddRange(new List<User>()
                //    {
                //        new User()
                //        {
                //            Name = "Admin",
                //            emp_ID = "050203",
                //            Email = "ADMIN@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Admin").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "RM",
                //            emp_ID = "050202",
                //            Email = "RM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Relation Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "MM",
                //            emp_ID = "050121",
                //            Email = "MM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Maker Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "RM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "MTL",
                //            emp_ID = "050203",
                //            Email = "MTL@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Maker TeamLeader").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "MM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "MO",
                //            emp_ID = "050121",
                //            Email = "MO@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Maker Officer").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "MTL").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "CM",
                //            emp_ID = "050203",
                //            Email = "CM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Checker Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "RM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "CTL",
                //            emp_ID = "050202",
                //            Email = "CTL@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Checker TeamLeader").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "CM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "CO",
                //            emp_ID = "050203",
                //            Email = "CO@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.Roles.Single(r => r.Name == "Checker Officer").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "CTL").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "DVM",
                //            emp_ID = "050122",
                //            Email = "DVM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Adama",
                //            RoleId = context.Roles.Single(r => r.Name == "District Valuation Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Adama").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "RM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new User()
                //        {
                //            Name = "DMO",
                //            emp_ID = "050123",
                //            Email = "DMO@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Adama",
                //            RoleId = context.Roles.Single(r => r.Name == "Maker Officer").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Adama").Id,
                //            Status = "Activated",
                //            // SupervisorId = context.Users.Single(r => r.Name == "MTL").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        }
                //    });

                //    context.SaveChanges();
                //}

                var rmUser = context.Users.SingleOrDefault(u => u.Name == "RM");
                var mmUser = context.Users.SingleOrDefault(u => u.Name == "MM");
                var dvmUser = context.Users.SingleOrDefault(u => u.Name == "DVM");
                var mtlUser = context.Users.SingleOrDefault(u => u.Name == "MTL");
                var cmUser = context.Users.SingleOrDefault(u => u.Name == "CM");
                var ctlUser = context.Users.SingleOrDefault(u => u.Name == "CTL");

                if (rmUser != null)
                {
                    var userToUpdate1 = context.Users.SingleOrDefault(u => u.Name == "MM");
                    if (userToUpdate1 != null) userToUpdate1.SupervisorId = rmUser.Id;

                    var userToUpdate2 = context.Users.SingleOrDefault(u => u.Name == "CM");
                    if (userToUpdate2 != null) userToUpdate2.SupervisorId = rmUser.Id;
                }

                if (mmUser != null)
                {
                    var userToUpdate3 = context.Users.SingleOrDefault(u => u.Name == "MTL");
                    if (userToUpdate3 != null) userToUpdate3.SupervisorId = mmUser.Id;
                }

                if (cmUser != null)
                {
                    var userToUpdate4 = context.Users.SingleOrDefault(u => u.Name == "CTL");
                    if (userToUpdate4 != null) userToUpdate4.SupervisorId = cmUser.Id;
                }

                if (dvmUser != null)
                {
                    var userToUpdate5 = context.Users.SingleOrDefault(u => u.Name == "DMO");
                    if (userToUpdate5 != null) userToUpdate5.SupervisorId = dvmUser.Id;
                }

                if (mtlUser != null)
                {
                    var userToUpdate6 = context.Users.SingleOrDefault(u => u.Name == "MO");
                    if (userToUpdate6 != null) userToUpdate6.SupervisorId = mtlUser.Id;
                }

                if (ctlUser != null)
                {
                    var userToUpdate7 = context.Users.SingleOrDefault(u => u.Name == "CO");
                    if (userToUpdate7 != null) userToUpdate7.SupervisorId = ctlUser.Id;
                }

                context.SaveChanges();

            }
        }
    }
}