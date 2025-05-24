using mechanical.Models.Entities;
using mechanical.Data;
using mechanical.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditBackOffice.Controllers
{
    public class RoleManagmentController : Controller
    {
        // GET: RoleManagmentController
        private readonly CbeContext _context;
        public RoleManagmentController(CbeContext context)
        {
            _context = context;
        }
        // GET: DistrictManagmentController
        public ActionResult Index()
        {
            var RoleList = _context.Roles.ToList();
            return View(RoleList);
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
        public ActionResult Create(Role model)
        {

            _context.Roles.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");

        }

        // GET: DistrictManagmentController/Edit/5
        public ActionResult Edit(Guid id)
        {
            var Role = _context.Roles.FirstOrDefault(c => c.Id == id);
            return View(Role);
        }

        // POST: DistrictManagmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model)
        {
            var role = _context.Roles.FirstOrDefault(c => c.Id == model.Id);
            role.Name = model.Name;
            _context.Entry(role).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: DistrictManagmentController/Delete/5
        public ActionResult Delete(Guid id)
        {
            var role = _context.Roles.FirstOrDefault(c => c.Id == id);
            return View(role);

        }

        // POST: DistrictManagmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            var role = _context.Roles.Find(id);

            _context.Roles.Remove(role);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
