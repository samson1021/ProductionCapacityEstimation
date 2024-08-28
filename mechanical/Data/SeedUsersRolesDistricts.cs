using mechanical.Models.Entities;
using mechanical.Models;
using mechanical.Data;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Data
{
    public class SeedUsersRolesDistricts
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CbeContext>();

                if (!context.CreateRoles.Any())
                {
                    context.CreateRoles.AddRange(new List<CreateRole>()
                    {
                        new CreateRole() { Id = Guid.Parse("62fca32f-7d61-4a08-bcfd-be6f7e54a725"), Name = "Maker Manager" },
                        new CreateRole() { Id = Guid.Parse("b77fed6d-ede5-4f3c-b782-235bfc91d25c"), Name = "Maker Officer" },
                        new CreateRole() { Id = Guid.Parse("45aaaf6e-2bfa-49ba-ac86-ddf579d713b6"), Name = "Relation Manager" },
                        new CreateRole() { Id = Guid.Parse("2e081360-8c03-4a14-b521-af7f3b46695f"), Name = "Checker Manager" },
                        new CreateRole() { Id = Guid.Parse("b8908ad3-2ebb-42cb-bb3c-13abfa20acfb"), Name = "Checker Officer" },
                        new CreateRole() { Id = Guid.Parse("54d1ce90-33bb-4614-aea9-1d851c8be03e"), Name = "Maker TeamLeader" },
                        new CreateRole() { Id = Guid.Parse("23d86ed5-beb9-447e-999c-a310785a973b"), Name = "Checker TeamLeader" },
                        new CreateRole() { Id = Guid.Parse("dbc8f8c4-2f78-4191-81c1-982b4ebaa40e"), Name = "Higher Official" },
                        new CreateRole() { Id = Guid.Parse("fd6e2ae8-7837-457c-9ea9-a4074983b6c3"), Name = "District Valuation Manager" },
                        new CreateRole() { Id = Guid.Parse("9c49e04d-3b46-4899-bce2-021cdbe2a496"), Name="Super Admin" }, 
                        new CreateRole() { Id = Guid.Parse("45aaaf6e-2bfa-49ba-ac86-ddf579d71fb6"), Name="Admin"}
                    });
                    context.SaveChanges();
                }

                if (!context.Districts.Any())
                {
                    context.Districts.AddRange(new List<District>()
                    {
                        new District() { Id = Guid.Parse("42031e60-8525-4d30-80c4-058d8566e800"), Name = "Jimma" },
                        new District() { Id = Guid.Parse("f59ab3c7-1dd2-4eb0-8b76-14a4e5bd71cc"), Name = "Debre Markos" },
                        new District() { Id = Guid.Parse("ee10d1f4-0b8f-470b-9a12-238eb29034e1"), Name = "Gullelie" },
                        new District() { Id = Guid.Parse("3cb93ef5-15d1-41bc-873d-349301bdce51"), Name = "Kality" },
                        new District() { Id = Guid.Parse("8834a076-2051-414c-9e47-3578b3199243"), Name = "Hossana" },
                        new District() { Id = Guid.Parse("b16a0df2-1b03-4ec0-ac38-35e73093e3ff"), Name = "Shire" },
                        new District() { Id = Guid.Parse("a085b52c-ffc7-4a21-84d4-3ecb85bdc473"), Name = "Bole" },
                        new District() { Id = Guid.Parse("42962900-4fd6-4602-9786-4bc5c43b52be"), Name = "Kirkos" },
                        new District() { Id = Guid.Parse("cb400177-4a27-4875-a307-5660e53c2ae4"), Name = "Dilla" },
                        new District() { Id = Guid.Parse("fed3fc60-862a-4874-97d3-585a14e9de7f"), Name = "Nekemte" },
                        new District() { Id = Guid.Parse("448db04e-4e8f-44c1-9032-5e0f3106e9cb"), Name = "Adama" },
                        new District() { Id = Guid.Parse("aed277ca-a1d8-4949-9aa4-661fbf3157a2"), Name = "Shashemene" },
                        new District() { Id = Guid.Parse("dbb920b8-997f-4c97-86d7-68d8f1c796a6"), Name = "Yeka" },
                        new District() { Id = Guid.Parse("5bc6b5d2-6e5f-40c0-8013-709a00e2f1b1"), Name = "Semera" },
                        new District() { Id = Guid.Parse("d90a0bc1-f272-47a1-8dc6-739b8331fb0c"), Name = "Hawassa" },
                        new District() { Id = Guid.Parse("60a4d0fd-f45e-446f-992d-823fb87c356d"), Name = "Nifas Silk" },
                        new District() { Id = Guid.Parse("55af2c78-9f83-48fa-be67-82d0d0b3f9be"), Name = "Dire Dawa" },
                        new District() { Id = Guid.Parse("bb95eeb5-7fbe-427f-9544-84ec85c21ac3"), Name = "Mettu" },
                        new District() { Id = Guid.Parse("d8afce22-d091-44bd-a0d5-87084f384187"), Name = "Bahir Dar" },
                        new District() { Id = Guid.Parse("9c58689a-77cb-41b6-9c80-9432d6fb960f"), Name = "Dessie" },
                        new District() { Id = Guid.Parse("98c48a1c-085e-4df9-9eb9-948a8e9875a2"), Name = "Debre Berehan" },
                        new District() { Id = Guid.Parse("6093b92e-3345-4ea7-9bfb-98db18e5c98d"), Name = "Assela" },
                        new District() { Id = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"), Name = "Head Office" },
                        new District() { Id = Guid.Parse("3ecd3895-ba6a-40d2-8787-b8c69b650985"), Name = "Megenangna" },
                        new District() { Id = Guid.Parse("a71b72eb-e21e-4123-b4a0-c0edb241d1c0"), Name = "Arada" },
                        new District() { Id = Guid.Parse("bc36afd3-6a62-442f-a153-d184d7eac505"), Name = "Gondar" },
                        new District() { Id = Guid.Parse("0a25a72c-f22c-448f-9f29-d27973344bc1"), Name = "Ambo" },
                        new District() { Id = Guid.Parse("a9e6bb51-b159-4f26-b028-d87e47b0d8cb"), Name = "Woldia" },
                        new District() { Id = Guid.Parse("e5ad0e52-21ab-4a53-ac91-ee577e464702"), Name = "Mekelle" },
                        new District() { Id = Guid.Parse("3249d8c5-2736-4c48-9a05-f01d39abfeb5"), Name = "Merkato" },
                        new District() { Id = Guid.Parse("98a684c3-ac1e-4a35-b8b5-f109ee8760a5"), Name = "Wolaita Sodo" },
                        new District() { Id = Guid.Parse("98e3136c-e9a2-45d5-8dc0-f16929691a95"), Name = "Kolfe" },
                        new District() { Id = Guid.Parse("74827608-2ecd-4cb0-8db9-f9a9306db4af"), Name = "Jigjiga" }
                    });

                    context.SaveChanges();
                }

                if (!context.CreateUsers.Any())
                {
                    context.CreateUsers.AddRange(new List<CreateUser>()
                    {
                        new CreateUser()
                        {
                            Id = Guid.Parse("bd481732-183e-482e-ae38-30c261bfd675"),
                            Name = "Admin",
                            emp_ID = "050203",
                            Email = "ADMIN@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("45aaaf6e-2bfa-49ba-ac86-ddf579d71fb6"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234", 
                            Status = "Activated",
                            SupervisorId = null,
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("47315c47-f8f0-4b68-a7bd-ba2e3c3561a1"),
                            Name = "RM",
                            emp_ID = "050202",
                            Email = "RM@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("45aaaf6e-2bfa-49ba-ac86-ddf579d713b6"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = null,
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("a0713be3-dcf7-46e5-82f2-50960d7f7bf8"),
                            Name = "CM",
                            emp_ID = "050203",
                            Email = "CM@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("2e081360-8c03-4a14-b521-af7f3b46695f"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = null,
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("b905823c-ab76-48f7-b0d2-5847b32128b5"),
                            Name = "CTL",
                            emp_ID = "050202",
                            Email = "CTL@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("23d86ed5-beb9-447e-999c-a310785a973b"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = Guid.Parse("a0713be3-dcf7-46e5-82f2-50960d7f7bf8"),
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("29f8c595-2b6a-4650-90e8-90a8f7273ab6"),
                            Name = "CO",
                            emp_ID = "050203",
                            Email = "CO@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("b8908ad3-2ebb-42cb-bb3c-13abfa20acfb"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = Guid.Parse("b905823c-ab76-48f7-b0d2-5847b32128b5"),
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("c05f71be-51ad-4f15-9825-6975aacccc1f"),
                            Name = "MM",
                            emp_ID = "050121",
                            Email = "MM@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("62fca32f-7d61-4a08-bcfd-be6f7e54a725"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = null,
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("69f56626-3ed8-49bb-ae0d-e917e1a8dfe2"),
                            Name = "MTL",
                            emp_ID = "050203",
                            Email = "MTL@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("54d1ce90-33bb-4614-aea9-1d851c8be03e"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = Guid.Parse("c05f71be-51ad-4f15-9825-6975aacccc1f"),
                            Department = "Mechanical"
                        },
                        new CreateUser()
                        {
                            Id = Guid.Parse("f164ba41-8ae1-4115-b4b0-94d749cb8bdd"),
                            Name = "MO",
                            emp_ID = "050121",
                            Email = "MO@GMAIL.COM",
                            PhoneNO = "0925473240",
                            Branch = "Head Office",
                            RoleId = Guid.Parse("b77fed6d-ede5-4f3c-b782-235bfc91d25c"),
                            DistrictId = Guid.Parse("4c2739d0-162a-4da8-a3a4-b0149800b295"),
                            Password = "1234",
                            Status = "Activated",
                            SupervisorId = Guid.Parse("69f56626-3ed8-49bb-ae0d-e917e1a8dfe2"),
                            Department = "Mechanical"
                        }
                    });

                    context.SaveChanges();
                }

            }
        }
    }
}