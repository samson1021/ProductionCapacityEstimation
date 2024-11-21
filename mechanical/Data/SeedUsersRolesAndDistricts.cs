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
                
                //context.Database.EnsureCreated(); // Ensure database is created

                //if (!context.CreateRoles.Any())
                //{
                //    context.CreateRoles.AddRange(new List<CreateRole>()
                //    {
                //        new CreateRole() { Name="Super Admin" }, 
                //        new CreateRole() { Name="Admin"},
                //        new CreateRole() { Name = "Relation Manager" },
                //        new CreateRole() { Name = "District Valuation Manager" },
                //        new CreateRole() { Name = "Maker Manager" },
                //        new CreateRole() { Name = "Maker TeamLeader" },
                //        new CreateRole() { Name = "Maker Officer" },
                //        new CreateRole() { Name = "Checker Manager" },
                //        new CreateRole() { Name = "Checker TeamLeader" },
                //        new CreateRole() { Name = "Checker Officer" },
                //        new CreateRole() { Name = "Higher Official" }
                //    });
                //    context.SaveChanges();
                //}

                //if (!context.Districts.Any())
                //{
                //    context.Districts.AddRange(new List<District>()
                //    {
                //        new District() { Name = "Head Office" },
                //        new District() { Name = "Jigjiga" },
                //        new District() { Name = "Adama" },
                //        new District() { Name = "Dire Dawa" },
                //        new District() { Name = "Jimma" },
                //        new District() { Name = "Debre Markos" },
                //        new District() { Name = "Gullelie" },
                //        new District() { Name = "Kality" },
                //        new District() { Name = "Hossana" },
                //        new District() { Name = "Shire" },
                //        new District() { Name = "Bole" },
                //        new District() { Name = "Kirkos" },
                //        new District() { Name = "Dilla" },
                //        new District() { Name = "Nekemte" },
                //        new District() { Name = "Shashemene" },
                //        new District() { Name = "Yeka" },
                //        new District() { Name = "Semera" },
                //        new District() { Name = "Hawassa" },
                //        new District() { Name = "Nifas Silk" },
                //        new District() { Name = "Mettu" },
                //        new District() { Name = "Bahir Dar" },
                //        new District() { Name = "Dessie" },
                //        new District() { Name = "Debre Berehan" },
                //        new District() { Name = "Assela" },
                //        new District() { Name = "Megenangna" },
                //        new District() { Name = "Arada" },
                //        new District() { Name = "Gondar" },
                //        new District() { Name = "Ambo" },
                //        new District() { Name = "Woldia" },
                //        new District() { Name = "Mekelle" },
                //        new District() { Name = "Merkato" },
                //        new District() { Name = "Wolaita Sodo" },
                //        new District() { Name = "Kolfe" }
                //    });

                //    context.SaveChanges();
                //}

                //if (!context.CreateUsers.Any())
                //{
                //    context.CreateUsers.AddRange(new List<CreateUser>()
                //    {
                //        new CreateUser()
                //        {                            
                //            Name = "Admin",
                //            emp_ID = "050203",
                //            Email = "ADMIN@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Admin").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,                            
                //            Password = "1234", 
                //            Status = "Activated",
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "RM",
                //            emp_ID = "050202",
                //            Email = "RM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Relation Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,  
                //            Password = "1234",
                //            Status = "Activated",
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "MM",
                //            emp_ID = "050121",
                //            Email = "MM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Maker Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id, 
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "RM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "MTL",
                //            emp_ID = "050203",
                //            Email = "MTL@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Maker TeamLeader").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id, 
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "MM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "MO",
                //            emp_ID = "050121",
                //            Email = "MO@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Maker Officer").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id, 
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "MTL").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "CM",
                //            emp_ID = "050203",
                //            Email = "CM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Checker Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,  
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "RM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "CTL",
                //            emp_ID = "050202",
                //            Email = "CTL@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Checker TeamLeader").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id,  
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "CM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "CO",
                //            emp_ID = "050203",
                //            Email = "CO@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Head Office",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Checker Officer").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Head Office").Id, 
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "CTL").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "DVM",
                //            emp_ID = "050122",
                //            Email = "DVM@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Adama",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "District Valuation Manager").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Adama").Id, 
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "RM").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        },
                //        new CreateUser()
                //        {                            
                //            Name = "DMO",
                //            emp_ID = "050123",
                //            Email = "DMO@GMAIL.COM",
                //            PhoneNO = "0925473240",
                //            Branch = "Adama",
                //            RoleId = context.CreateRoles.Single(r => r.Name == "Maker Officer").Id,
                //            DistrictId = context.Districts.Single(d => d.Name == "Adama").Id, 
                //            Password = "1234",
                //            Status = "Activated",
                //            // SupervisorId = context.CreateUsers.Single(r => r.Name == "MTL").Id,
                //            SupervisorId = null,
                //            Department = "Mechanical"
                //        }
                //    });

                //    context.SaveChanges();
                //}
                                
                //var rmUser = context.CreateUsers.SingleOrDefault(u => u.Name == "RM");
                //var mmUser = context.CreateUsers.SingleOrDefault(u => u.Name == "MM");
                //var dvmUser = context.CreateUsers.SingleOrDefault(u => u.Name == "DVM");
                //var mtlUser = context.CreateUsers.SingleOrDefault(u => u.Name == "MTL");
                //var cmUser = context.CreateUsers.SingleOrDefault(u => u.Name == "CM");
                //var ctlUser = context.CreateUsers.SingleOrDefault(u => u.Name == "CTL");

                //if (rmUser != null)
                //{
                //    var userToUpdate1 = context.CreateUsers.SingleOrDefault(u => u.Name == "MM");
                //    if (userToUpdate1 != null) userToUpdate1.SupervisorId = rmUser.Id;                     

                //    var userToUpdate2 = context.CreateUsers.SingleOrDefault(u => u.Name == "CM");
                //    if (userToUpdate2 != null) userToUpdate2.SupervisorId = rmUser.Id;  
                //}

                //if (mmUser != null)
                //{
                //    var userToUpdate3 = context.CreateUsers.SingleOrDefault(u => u.Name == "MTL");
                //    if (userToUpdate3 != null) userToUpdate3.SupervisorId = mmUser.Id; 
                    
                //}

                //if (cmUser != null)
                //{
                //    var userToUpdate4 = context.CreateUsers.SingleOrDefault(u => u.Name == "CTL");
                //    if (userToUpdate4 != null)  userToUpdate4.SupervisorId = cmUser.Id;                    
                //}

                //if (dvmUser != null)
                //{
                //    var userToUpdate5 = context.CreateUsers.SingleOrDefault(u => u.Name == "DMO");
                //    if (userToUpdate5 != null) userToUpdate5.SupervisorId = dvmUser.Id;                    
                //}

                //if (mtlUser != null)
                //{
                //    var userToUpdate6 = context.CreateUsers.SingleOrDefault(u => u.Name == "MO");
                //    if (userToUpdate6 != null) userToUpdate6.SupervisorId = mtlUser.Id;                    
                //}

                //if (ctlUser != null)
                //{
                //    var userToUpdate7 = context.CreateUsers.SingleOrDefault(u => u.Name == "CO");
                //    if (userToUpdate7 != null) userToUpdate7.SupervisorId = ctlUser.Id;                    
                //}

                //context.SaveChanges();

            }
        }
    }
}