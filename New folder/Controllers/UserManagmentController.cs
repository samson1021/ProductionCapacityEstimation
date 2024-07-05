using mechanical.Models.Entities;
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

namespace mechanical.Controllers
{
    [Authorize(Roles = "Admin,Super Admin,Maker Manager,Maker TeamLeader")]
    public class UserManagmentController : BaseController
    {
        private readonly CbeContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public UserManagmentController(CbeContext context, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _userService = userService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GenerateValidPassword(int length)
        {

            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";

            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            const string numericChars = "0123456789";

            const string specialChars = "!@#$%^&*()";

            int requiredLength = 8;

            int requiredLowerCase = 1;

            int requiredUpperCase = 1;

            int requiredNumeric = 1;

            int requiredSpecialChars = 1;



            var random = new Random();



            var passwordChars = new List<char>();



            // Generate required characters

            for (int i = 0; i < requiredLowerCase; i++)

            {

                passwordChars.Add(lowerCaseChars[random.Next(lowerCaseChars.Length)]);

            }

            for (int i = 0; i < requiredUpperCase; i++)

            {

                passwordChars.Add(upperCaseChars[random.Next(upperCaseChars.Length)]);

            }

            for (int i = 0; i < requiredNumeric; i++)

            {

                passwordChars.Add(numericChars[random.Next(numericChars.Length)]);

            }

            for (int i = 0; i < requiredSpecialChars; i++)

            {

                passwordChars.Add(specialChars[random.Next(specialChars.Length)]);

            }
            // Generate remaining random characters

            int remainingLength = requiredLength - passwordChars.Count;

            for (int i = 0; i < remainingLength; i++)

            {

                string charSet = lowerCaseChars + upperCaseChars + numericChars + specialChars;

                passwordChars.Add(charSet[random.Next(charSet.Length)]);

            }
            // Shuffle the characters

            for (int i = passwordChars.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                var temp = passwordChars[i];

                passwordChars[i] = passwordChars[j];

                passwordChars[j] = temp;

            }
            return new string(passwordChars.ToArray());

        }
        private bool IsValidPassword(string password)
        {
            const string passwordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, passwordRegexPattern);
        }
        public static string HashPassword(string password)
        {

            using (var sha256 = SHA256.Create())

            {

                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hash;

            }

        }
        // GET: UserManagmentController    
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
            //    var usersWithDistricts = dbContext.Users
            //.Include(u => u.District) // Include the District navigation property
            //.ToList();

            //    return View(usersWithDistricts);
            //var usersWithDistricts = _context.CreateUsers.Include(u => u.District).ToList();
            // return Json(usersWithDistricts);

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
                u.Name,
                u.Email,
                DistrictName = u.District != null ? u.District.Name : "",
                RoleName = u.Role != null ? u.Role.Name : "",
                u.Branch,
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
                    u.Name,
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
        public IActionResult CreateUser(CreateUser model)
        {
            var password = "1234";
            //var password = GenerateValidPassword(8);
            string EncryptedPass = null;
          //  EncryptedPass = HashPassword(password);

            //if (IsValidPassword(password))
            //{
            //    model.Password = EncryptedPass;
            //}
            model.Password = password;
            model.Status = "Activated";
            var UpperEmail = model.Email.ToUpper();
            model.Email = UpperEmail;
            
                _context.CreateUsers.Add(model);
                _context.SaveChanges();
            
                // Save the form data to the database
                // You can use your data access logic or an ORM here
                //using (var dbContext = new CbeCreditContext())
                //{
                //    // Map the form data to your database entity
                //    var user = new CreateUser
                //    {
                //        Name = model.Name,
                //        Email = model.Email,
                //        District = model.District,
                //        Branch = model.Branch,
                //        Password = model.Password
                //    };

                //    // Add the user entity to the context and save changes
                //    dbContext.CreateUsers.Add(user);
                //    dbContext.SaveChanges();

                    // Redirect to a success page or perform any other necessary actions
                    return RedirectToAction("Index");
                //}
            //}
            // If the model state is not valid, return the same view with the validation errors
    

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
        //public JsonResult GetId()
        //{
        //    var AllData = _context.CreateRoles.Select(c => new { Id = c.Id}).ToList();
        //    return Json(AllData);
        //}




        // POST: UserManagmentController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        [HttpPost]
        public ActionResult SaveEdited(CreateUser model)
        {

            var user = _context.CreateUsers.FirstOrDefault(u => u.Id == model.Id);

            user.Name = model.Name;
            user.Email = model.Email;
            
            user.Branch = model.Branch;
            user.Status = model.Status;
            user.Department = model.Department;
            if (model.RoleId != Guid.Empty && model.RoleId != new Guid())
            {
                user.RoleId = model.RoleId;
            }
            if (model.DistrictId != Guid.Empty && model.DistrictId != new Guid())
            {
                user.DistrictId = model.DistrictId;
            }
            //if (model.RoleId != 0)
            //{
            //    user.RoleId = model.RoleId;
            //}

            //if (model.DistrictId != 0)
            //{
            //    user.DistrictId = model.DistrictId;
            //}

            //_context.CreateUsers.Update(model);
            _context.Entry(user).State = EntityState.Modified;

            // Save the changes to the database
            _context.SaveChanges();
               // transaction.Commit();
                // Redirect to a different action or view
                
           
            
                return RedirectToAction("Index");
            
           

            //_context.CreateUsers.Add(model);
            //_context.SaveChanges();

            //return RedirectToAction("Index");
        }
        // GET: UserManagmentController/Edit/5
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            //var user = _userRepository.GetUserById(id);
            var user = _context.CreateUsers.FirstOrDefault(c => c.Id == id);
            //var model = new CreateUser
            //{
            //    Id = user.Id,
            //    Name = user.Name,
            //    Email = user.Email,
            //    District = user.District,
            //    Branch = user.Branch,
            //    RoleId = user.RoleId,
            //    Password = user.Password,
            //    Status = user.Status
            //};

            return View(user);
            //return View(userData);
        }
        // GET: UserManagmentController/Delete/5
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
