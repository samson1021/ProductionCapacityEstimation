using mechanical.Models.Entities;
using mechanical.Data;
using mechanical.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace CreditBackOffice.Controllers
{
    [Authorize(Roles = "Admin,Super Admin,Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    public class DistrictManagmentController : Controller
    {
        private readonly CbeContext _context;
        public DistrictManagmentController(CbeContext context)
        {
            _context = context;
        }
        // GET: DistrictManagmentController
        public ActionResult Index()
        {   var DistrictList =  _context.Districts.ToList();
            return View(DistrictList);
        }

        // GET: DistrictManagmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DistrictManagmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DistrictManagmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(District model)
        {
            
            _context.Districts.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
            
        }

        // GET: DistrictManagmentController/Edit/5
        public ActionResult Edit(Guid id)
        {   
            var District = _context.Districts.FirstOrDefault( c => c.Id == id);
            return View(District);
        }

        // POST: DistrictManagmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model)
        {       
               var district= _context.Districts.FirstOrDefault(c => c.Id == model.Id);
                district.Name= model.Name;
                _context.Entry(district).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
             
        }

        // GET: DistrictManagmentController/Delete/5
        public ActionResult Delete(Guid id)
        {
            var District = _context.Districts.FirstOrDefault(c => c.Id == id);
            return View(District);
            
        }

        // POST: DistrictManagmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            var district = _context.Districts.Find(id);

            _context.Districts.Remove(district);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
