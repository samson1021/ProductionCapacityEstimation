using AutoMapper;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Services.UserService;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.AuthenticatioinService;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Admin,Super Admin,Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
   
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
            var role = _context.Users.Include(c => c.Role).Where(res => res.Id == response).FirstOrDefault();
            List<User> usersWithDistricts = new List<User>();
            if (role.Role.Name == "Super Admin")
            {
                usersWithDistricts = _context.Users.Include(u => u.District).Include(c => c.Role).Where(res => res.Role.Name != "Super Admin").ToList();
            }
            else if (role.Role.Name == "Admin")
            {
                usersWithDistricts = _context.Users.Include(u => u.District).Include(c => c.Role).Where(res => res.Role.Name != "Admin" && res.Role.Name != "Super Admin").ToList();
            }
            else
            {
                usersWithDistricts = _context.Users.Where(res => res.Status == "Activated").Include(u => u.District).Include(c => c.Role).ToList();
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
            var result = _context.Roles
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
            var allUsers = _context.Users.Where(u => u.RoleId == roleId && u.Status == "Activated").ToList();

            if (RoleName == "Maker Officer")
            {
                allUsers = _context.Users.Where(u => u.Role.Name == "Maker TeamLeader" && u.DistrictId == districtId && u.Department == Department && u.Status == "Activated").ToList();
            }
            else if (RoleName == "Checker Officer")
            {
                var allRoles = _context.Roles.Where(u => u.Name == "Checker TeamLeader").Select(
                    u => new { u.Id, u.Name, }).ToList();
                Guid SuperRoleId = (Guid)(allRoles.FirstOrDefault()?.Id);
                allUsers = _context.Users.Where(u => u.RoleId == SuperRoleId && u.Department == Department && u.DistrictId == districtId && u.Status == "Activated").ToList();

            }
            else if (RoleName == "Maker TeamLeader")
            {
                var allRoles = _context.Roles.Where(u => u.Name == "Maker Manager").Select(
                    u => new { u.Id, u.Name, }).ToList();
                Guid SuperRoleId = (Guid)(allRoles.FirstOrDefault()?.Id);
                allUsers = _context.Users.Where(u => u.RoleId == SuperRoleId && u.Department == Department && u.DistrictId == districtId && u.Status == "Activated").ToList();

            }
            else if (RoleName == "Checker TeamLeader")
            {
                var allRoles = _context.Roles.Where(u => u.Name == "Checker Manager").Select(
                    u => new { u.Id, u.Name, }).ToList();
                Guid SuperRoleId = (Guid)(allRoles.FirstOrDefault()?.Id);
                allUsers = _context.Users.Where(u => u.RoleId == SuperRoleId && u.Department == Department && u.DistrictId == districtId && u.Status == "Activated").ToList();
            }
            else {
                allUsers = null;
            }
            return Json(allUsers);
        }
        [HttpGet]
        public async Task<ActionResult> GetUserDetails(Guid id)
        {
            var user = await _context.Users.FindAsync();
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
            var manager = await _context.Users.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == teamLeaderId);
            if (manager == null || manager.Role.Name != "Maker TeamLeader")
            {
                return BadRequest();
            }
            var makerTeamleaders = _context.Users.Where(res => res.Role.Name == "Maker Officer" && res.SupervisorId == teamLeaderId && res.Status == "Activated").ToList();
            return Json(makerTeamleaders);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCheckerOfficer()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var teamLeaderId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.Users.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == teamLeaderId);
            if (manager == null || manager.Role.Name != "Checker TeamLeader")
            {
                return BadRequest();
            }
            var makerTeamleaders = _context.Users.Where(res => res.Role.Name == "Checker Officer" && res.SupervisorId == teamLeaderId && res.Status == "Activated").ToList();
            return Json(makerTeamleaders);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetMakerTeamleader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var managerId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.Users.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == managerId);
            

            if (manager == null || manager.Role.Name != "Maker Manager")
            {
                return BadRequest();
            }
            else if (manager.Role.Name == "District Valuation Manager")
            {
                var makerTeamleader = _context.Users.Where(res => res.Role.Name == "Maker Officer" && res.Department == manager.Department && res.DistrictId == manager.DistrictId && res.Status == "Activated").ToList();
                return Json(makerTeamleader);
            }
            else
            {
                var makerTeamleaders = _context.Users.Where(res => res.Role.Name == "Maker TeamLeader" && res.Department == manager.Department && res.DistrictId == manager.DistrictId && res.Status == "Activated").ToList();
                return Json(makerTeamleaders);
            }

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCheckerTeamleader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var managerId = Guid.Parse(httpContext.Session.GetString("userId"));
            var manager = await _context.Users.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == managerId);
            if (manager.Role.Name == "District Valuation Manager")
            {
                var checkerTeamleader = _context.Users.Where(res => res.Role.Name == "Checker Officer" && res.Department == manager.Department && res.DistrictId == manager.DistrictId && res.Status == "Activated").ToList();
                return Json(checkerTeamleader);
            }
            if (manager == null || manager.Role.Name != "Checker Manager")
            {
                return BadRequest();
            }
            var checkerTeamleaders = _context.Users.Where(res => res.Role.Name == "Checker TeamLeader" && res.Department == manager.Department && res.DistrictId == manager.DistrictId && res.Status == "Activated").ToList();
            return Json(checkerTeamleaders);
        }
        public JsonResult GetRole()
        {
            //var districts = _context.Districts.ToList();
            var roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).ToList();
            return Json(roles);

        }
        // GET: UserManagmentController/Create
        public IActionResult Create()
        {
            //var roles = _context.Roles.ToList();
            return View();
            /* var userrole = new UserRoleList
            {
                ValueTask = IDataTokensMetadata;
            diplay=name;
          }
        User.UserRoleList=userrole*/

        }
        [HttpPost]
        public async Task<IActionResult> Create(User model)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(res => res.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                return View(model);
            }
            existingUser = await _context.Users.FirstOrDefaultAsync(res => res.emp_ID == model.emp_ID);
            if (existingUser != null)
            {
                ModelState.AddModelError("emp_ID", "A user with this Employee Id already exists.");
                return View(model);
            }
            var RoleName = await _context.Roles.FirstOrDefaultAsync(res => res.Id == model.RoleId);
            var DistrictName = await _context.Districts.FirstOrDefaultAsync(res => res.Id == model.DistrictId);
            if (RoleName.Name == "Maker Manager" || RoleName.Name == "Checker Manager" || RoleName.Name== "District Valuation Manager")
            {
                var existingManager = await _context.Users.FirstOrDefaultAsync(res => res.RoleId == model.RoleId && res.DistrictId == model.DistrictId && res.Status =="Activated");
                if (existingManager != null)
                {
                    ModelState.AddModelError("emp_ID", $"A user with role '{RoleName.Name}' already exists for district '{DistrictName.Name}'. " +"Only one Manager can be assigned per district.");
                    return View(model);
                }
            }


            model.Status = "Activated";
            await _context.Users.AddAsync(model);
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
        [HttpGet]
        public JsonResult GetRoles(Guid districtId)
        {
            var userId = base.GetCurrentUserId();
            var RoleName = _context.Users.Include(res => res.Role).Where(c => c.Id == userId).FirstOrDefault();
            var roles = new List<object>();
            var districtName = _context.Districts.Where(c => c.Id == districtId).FirstOrDefault();
            if (RoleName.Role.Name == "Admin")
            {   
                if(districtName.Name == "Head Office")
                    roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Admin" && c.Name != "Super Admin").Cast<object>().ToList();
                else
                    roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Admin" && c.Name != "Super Admin" && c.Name != "Checker Manager" && c.Name != "Checker Teamleader" && c.Name != "Maker Teamleader" && c.Name != "Maker Manager").Cast<object>().ToList();
            }
            else
            {
                if (districtName.Name == "Head Office")
                    roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Super Admin").Cast<object>().ToList();
                else
                    roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Super Admin" && c.Name != "Checker Manager" && c.Name != "Checker Teamleader" && c.Name != "Maker Teamleader" && c.Name != "Maker Manager").Cast<object>().ToList();
            }
            return Json(roles);
        }
        public JsonResult GetRoles()
        {
            var userId = base.GetCurrentUserId();
            var RoleName = _context.Users.Include(res => res.Role).Where(c => c.Id == userId).FirstOrDefault();
            var roles = new List<object>();
            
            if (RoleName.Role.Name == "Admin")
            {
                
                    roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Admin" && c.Name != "Super Admin").Cast<object>().ToList();
               
            }
            else
            {
                roles = _context.Roles.Select(c => new { RoleId = c.Id, Name = c.Name }).Where(c => c.Name != "Super Admin").Cast<object>().ToList();
            }

            return Json(roles);
        }
        [HttpPost]
        public ActionResult SaveEdited(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);

            user.Name = model.Name;
            user.Email = model.Email;
            user.Status = model.Status;
            user.Branch = model.Branch;
            //user.Status = model.Status;
            user.PhoneNO = model.PhoneNO;
            user.Department = model.Department;
            if (model.RoleId != Guid.Empty && model.RoleId != new Guid())
            {
                user.RoleId = model.RoleId;
            }
            if (model.DistrictId != Guid.Empty && model.DistrictId != new Guid())
            {
                user.DistrictId = model.DistrictId;
            }

            //_context.Users.Update(model);
            _context.Entry(user).State = EntityState.Modified;

            // Save the changes to the database
            _context.SaveChanges();
            // transaction.Commit();
            // Redirect to a different action or view

            return RedirectToAction("Index");
        }
        // GET: UserManagmentController/Edit/5
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var user = _context.Users.Include(u => u.Supervisor).FirstOrDefault(c => c.Id == id);

            return View(user);
        }
        public ActionResult Delete(Guid id)
        {
            var user = _context.Users.FirstOrDefault(c => c.Id == id);

            return View(user);
        }
        // POST: UserManagmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        { 
            var userId = _context.Users.Find(id);
            if(userId != null){
                userId.Status = "Deactivated";
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }    
        }

        [HttpGet]
        public async Task<IActionResult> GetPeerRMs()
        {
            var rms = await _userService.GetPeerRMs(base.GetCurrentUserId());
            var result = rms
                .Where(crm => crm.Id != base.GetCurrentUserId())
                .Select(rm => new { Id = rm.Id, Name = rm.Name });
            return Ok(result);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetuserInfos(Guid id)
        {
            //var districts = _context.Districts.ToList();
            var user = await _userService.GetUser(id);
            return Json(user);
        }
    }
}
