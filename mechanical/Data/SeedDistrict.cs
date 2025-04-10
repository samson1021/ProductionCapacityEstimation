using mechanical.Models.Entities;
using mechanical.Models;
using mechanical.Data;
using Microsoft.EntityFrameworkCore;
//using NuGet.ContentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace mechanical.Data
{
    public class SeedDistrict
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CbeContext>();

                context.Database.EnsureCreated();

                if (!context.Districts.Any())
                {
                    context.Districts.AddRange(new List<District>()
                    {
                        new District()
                        {
                            Name = "Head Office"
                        },
                        new District()
                        {
                            Name = "Kolfe"
                        },
                        new District() {
                             Name = "Arada"
                        },
                        new District()
                        {
                            Name = "Bole"
                        },
                        new District()
                        {
                            Name = "Merkato"
                        },
                        new District() {
                             Name = "Kirkos"
                        },
                        new District()
                        {
                            Name = "Megenangna"
                        },
                        new District()
                        {
                            Name = "Yeka"
                        },
                        new District()
                        {
                            Name = "Nifas silk"
                        },
                        new District() {
                             Name = "Kality"
                        },
                        new District()
                        {
                            Name = "Gullelie"
                        },
                        new District()
                        {
                            Name = "Semera"
                        },
                        new District() {
                             Name = "Mekelle"
                        },
                        new District()
                        {
                            Name = "Woldia"
                        }, new District()
                        {
                            Name = "Jigjiga"
                        },
                        new District()
                        {
                            Name = "Jimma"
                        },
                        new District()
                        {
                            Name = "Shashemene"
                        },
                        new District() {
                             Name = "Ambo"
                        },
                        new District()
                        {
                            Name = "Debre Markos"
                        },
                        new District()
                        {
                            Name = "Dilla"
                        },
                        new District() {
                             Name = "Dire Dawa"
                        },
                        new District()
                        {
                            Name = "Mettu"
                        },
                        new District()
                        {
                            Name = "Assela"
                        },
                        new District() {
                             Name = "Bahir Dar"
                        },
                        new District()
                        {
                            Name = "Adama"
                        },
                        new District()
                        {
                            Name = "Debre Berehan"
                        },
                        new District() {
                             Name = "Dessie"
                        },
                        new District()
                        {
                            Name = "Gondar"
                        },
                        new District()
                        {
                            Name = "Hawassa"
                        },
                        new District()
                        {
                            Name = "Hossana"
                        },
                        new District() {
                             Name = "Nekemte"
                        },
                        new District()
                        {
                            Name = "Wolaita Sodo"
                        },
                        new District()
                        {
                            Name = "Shire"
                        }
                    });
                    context.SaveChanges();
                }


            }
        }
    }
}