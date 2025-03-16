﻿using mechanical.Models.Entities;
using mechanical.Models;
using mechanical.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UserService;
using System.Data;
using mechanical.Services.AuthenticatioinService;
using System.Text.Json;
using AutoMapper;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Admin,Super Admin,Maker Manager,Maker TeamLeader")]
    public class UserManagmentController : BaseController
    {
        private readonly CbeContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        public UserManagmentController(CbeContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }
        
        public IActionResult Index()
        {

            return View();
        }
        [AllowAnonymous]
        public JsonResult GetOneDistrict(Guid id)
        {
            //var districts = _context.Districts.ToList();
            var district = _context.Districts.FirstOrDefault(c => c.Id == id);
            //var districts = _context.Districts.Select(c => new { DistrictId = c.Id, Name = c.Name }).ToList();
            return Json(district);

        }
        [AllowAnonymous]

        public JsonResult GetUsers()
        {
            var response = base.GetCurrentUserId();
            var role = _context.CreateUsers.Include(c => c.Role).Where(res => res.Id == response).FirstOrDefault();
            List<CreateUser> usersWithDistricts = new List<CreateUser>();
            if (role.Name == "Admin")
            {
                usersWithDistricts = _context.CreateUsers.Include(u => u.District).Include(c => c.Role).Where(res => res.Role.Name != "Admin").ToList();
            }
            else
            {
                usersWithDistricts = _context.CreateUsers.Include(u => u.District).Include(c => c.Role).ToList();
            }
            var usersData = usersWithDistricts.Select(u => new
            {
                u.emp_ID,
                u.Name,
                DistrictName = u.District != null ? u.District.Name : "",
                RoleName = u.Role != null ? u.Role.Name : "",
                u.Department,
                u.Status,
                u.Id
            });

            return Json(usersData);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> MyProfile()
        {
            var user = await _userService.GetUser();
            return View(user);
        }
        [HttpGet]
        [AllowAnonymous]

        public async Task<ActionResult> Profile(Guid id)
        {
            var user = await _userService.GetUser(id);
            return View(user);
        }
        [AllowAnonymous]
        public JsonResult GetDistrict()
        {
            //var districts = _context.Districts.ToList();
            var districts = _context.Districts.ToList();
            districts.Sort((p1,p2)=>p1.Name.CompareTo(p2.Name));
            return Json(districts);

        }
        public JsonResult GetSupervisors(Guid roleId , Guid districtId,string Department)
        {
            var result = _context.CreateRoles
                .Where(u => u.Id == roleId)
                .Select(u => new {
                    u.Id,
                    u.Name
                }).ToList();
            string RoleName = "maker";
            Guid? RoleId = null;
            //Guid RoleId = 00000000-0000-0000-0000-000000000000;
            foreach (var item in result)
            {
                RoleName = item.Name;
                RoleId = item.Id;
                // Do something with the supervisorName
            }
            //string RoleName = result.FirstOrDefault()?.Name;
            //Guid RoleId = (Guid)(result.FirstOrDefault()?.Id);
            var allUsers = _context.CreateUsers.Where(u => u.RoleId == roleId).ToList();

            if (RoleName == "Maker Officer")
            {
                 allUsers = _context.CreateUsers.Where(u => u.Role.Name == "Maker TeamLeader" && u.DistrictId == districtId && u.Department == Department).ToList();
            }
            else if (RoleName == "Checker Officer")
            {
                var allRoles = _context.CreateRoles.Where(u => u.Name == "Checker TeamLeader").Select(
                    u => new { u.Id, u.Name, }).ToList();
                Guid SuperRoleId = (Guid)(allRoles.FirstOrDefault()?.Id);
                 allUsers = _context.CreateUsers.Where(u => u.RoleId == SuperRoleId && u.Department == Department).ToList();

            }
            else if (RoleName == "Maker TeamLeader")
            {
                var allRoles = _context.CreateRoles.Where(u => u.Name == "Maker Manager").Select(
                    u => new { u.Id, u.Name, }).ToList();
                Guid SuperRoleId = (Guid)(allRoles.FirstOrDefault()?.Id);
                allUsers = _context.CreateUsers.Where(u => u.RoleId == SuperRoleId && u.Department == Department).ToList();

            }
            else if (RoleName == "Checker TeamLeader")
            {
                var allRoles = _context.CreateRoles.Where(u => u.Name == "Checker Manager").Select(
                    u => new { u.Id, u.Name, }).ToList();
                Guid SuperRoleId = (Guid)(allRoles.FirstOrDefault()?.Id);
                allUsers = _context.CreateUsers.Where(u => u.RoleId == SuperRoleId && u.Department == Department).ToList();
            }
            else {
                allUsers = null;
            }
            return Json(allUsers);
        }
        [HttpGet]
        public async Task<ActionResult> GetUserDetails(Guid id)
        {
            var user = await _context.CreateUsers.FindAsync();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetMakerOfficer()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var teamLeaderId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == teamLeaderId);
            if (manager == null || manager.Role.Name != "Maker TeamLeader")
            {
                return BadRequest();
            }
            var makerTeamleaders = _context.CreateUsers.Where(res => res.Role.Name == "Maker Officer" && res.SupervisorId == teamLeaderId).ToList();
            return Json(makerTeamleaders);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCheckerOfficer()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var teamLeaderId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == teamLeaderId);
            if (manager == null || manager.Role.Name != "Checker TeamLeader")
            {
                return BadRequest();
            }
            var makerTeamleaders = _context.CreateUsers.Where(res => res.Role.Name == "Checker Officer" && res.SupervisorId == teamLeaderId).ToList();
            return Json(makerTeamleaders);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetMakerTeamleader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var managerId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == managerId);
            if (manager.Role.Name == "District Valuation Manager")
            {
                var makerTeamleader = _context.CreateUsers.Where(res => res.Role.Name == "Maker Officer" && res.Department == manager.Department && res.DistrictId == manager.DistrictId).ToList();
                return Json(makerTeamleader);
            }
            if (manager == null || manager.Role.Name != "Maker Manager" )
            {
                return BadRequest();
            }
            else
            {
            var makerTeamleaders = _context.CreateUsers.Where(res=>res.Role.Name == "Maker TeamLeader" && res.Department == manager.Department && res.DistrictId == manager.DistrictId).ToList();
            return Json(makerTeamleaders);
            }

        }
        [HttpGet]
        [AllowAnonymous]
 
        public async Task<ActionResult> GetCheckerTeamleader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var managerId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == managerId);
            if (manager.Role.Name == "District Valuation Manager")
            {
                var checkerTeamleader = _context.CreateUsers.Where(res => res.Role.Name == "Checker Officer" && res.Department==manager.Department && res.DistrictId == manager.DistrictId).ToList();
                return Json(checkerTeamleader);
            }
            if (manager == null || manager.Role.Name != "Checker Manager")
            {
                return BadRequest();
            }
            var checkerTeamleaders = _context.CreateUsers.Where(res => res.Role.Name == "Checker TeamLeader" && res.Department == manager.Department && res.DistrictId == manager.DistrictId).ToList();
            return Json(checkerTeamleaders);
        }
        public JsonResult GetRole()
        {
            //var districts = _context.Districts.ToList();
            var roles = _context.CreateRoles.Select(c => new { RoleId = c.Id, Name = c.Name }).ToList();
            return Json(roles);

        }
        // GET: UserManagmentController/Create
        public IActionResult Create()
        {
            //var roles = _context.CreateRoles.ToList();
            return View();
            
            /* var userrole = new UserRoleList
            {
                ValueTask = IDataTokensMetadata;
            diplay=name;
          }
        CreateUser.UserRoleList=userrole*/

        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUser model)
        {
            var existingUser = await _context.CreateUsers.FirstOrDefaultAsync(res => res.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                return View(model);
            }
            existingUser = await _context.CreateUsers.FirstOrDefaultAsync(res => res.emp_ID == model.emp_ID);
            if (existingUser != null)
            {
                ModelState.AddModelError("emp_ID", "A user with this Employee Id already exists.");
                return View(model);
            }
            model.Status = "Activated";
            await _context.CreateUsers.AddAsync(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult GetEmployeeInfo(string id)
        {
            var employe = _authenticationService.GetEmployeeInfo(id);
            string jsonData = JsonConvert.SerializeObject(employe);

            return Content(jsonData, "application/json");
        }
        public JsonResult GetRoles()
        {   var userId = base.GetCurrentUserId();
            var RoleName = _context.CreateUsers.Include(res => res.Role).Where(c => c.Id == userId).FirstOrDefault();
            var roles = new List<object>();
            if (RoleName.Role.Name == "Super Admin")
            {
                roles = _context.CreateRoles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name == "Admin").Cast<object>().ToList();
            }
            if (RoleName.Role.Name == "Admin")
            {
                roles = _context.CreateRoles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Admin" && c.Name != "Super Admin").Cast<object>().ToList();
            }

            return Json(roles);
        }
       
        [HttpPost]
        public ActionResult Edit(CreateUser model)
        {

            var user = _context.CreateUsers.FirstOrDefault(u => u.Id == model.Id);
            if(user == null)
            {
                ModelState.AddModelError("", "User not found. Please try again.");
                return View(model);
            }
            var employe = _authenticationService.GetEmployeeInfo(user.emp_ID);
            _mapper.Map(model,user); 

            _mapper.Map(employe, user);

            _context.CreateUsers.Update(user);
            _context.SaveChanges();

             return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            var user = _context.CreateUsers.Include(u => u.Supervisor).FirstOrDefault(c => c.Id == id);
            ViewBag.Districts = await _context.Districts.ToListAsync();
            ViewBag.Roles = await _context.CreateRoles.ToListAsync();
           
            //var supervisors = JsonConvert.DeserializeObject<IEnumerable<CreateUser>>(GetSupervisors(user.RoleId, user.DistrictId, user.Department).ToString());
            //ViewBag.Supervisors =  GetSupervisors(user.RoleId, user.DistrictId, user.Department);

            return View(user);
        }
        public ActionResult Delete(Guid id)
        {
            var user = _context.CreateUsers.FirstOrDefault(c => c.Id == id);

            return View(user);
            
            
        }
        // POST: UserManagmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            var user = _context.CreateUsers.Find(id);

            _context.CreateUsers.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
