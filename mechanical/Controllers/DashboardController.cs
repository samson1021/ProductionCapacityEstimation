using mechanical.Data;
using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using mechanical.Services.PCE.PCECaseService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly CbeContext _cbeContext;

        private readonly IPCECaseService _PCECaseService;
        public DashboardController(ICaseService caseService, CbeContext cbeContext, IPCECaseService PCECaseService)
        {
            _caseService = caseService;
            _cbeContext = cbeContext;
            _PCECaseService = PCECaseService;
        }
        public async Task<IActionResult> RM()
        {
            var latestCase = await _caseService.GetRmLatestCases(base.GetCurrentUserId());
            var newCases = await _PCECaseService.GetLatestPCECases(base.GetCurrentUserId());
            ViewData["NewCases"] = newCases;
            return View(latestCase);
        }
        public async Task<IActionResult> MM()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> CM()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> DVM()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> MTL()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> CTL()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> MO()
        {
            var latestCase = await _caseService.GetMoLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> CO()
        {
            var latestCase = await _caseService.GetMoLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> HO()
        {
            var latestCase = await _caseService.GetHOLatestCases(base.GetCurrentUserId());
            var newCases = await _PCECaseService.GetLatestHOPCECases();
            ViewData["NewCases"] = newCases;
            return View(latestCase);
        }
        public async Task<IActionResult> Admin()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return RedirectToAction("Index", "UserManagment");
        }
        
        public async Task<IActionResult> Index(string Role)
        {
            var userId = base.GetCurrentUserId();
            if (Role == null)  
            {
                    Role = (await _cbeContext.Users.Include(res=>res.Role).Where(res=>res.Id == userId).FirstOrDefaultAsync()).Role.Name;
            }       
            
            if (Role == "Relation Manager")
                return RedirectToAction("RM");
            else if (Role == "Maker Manager")
                return RedirectToAction("MM");
            else if (Role == "Maker TeamLeader")
                return RedirectToAction("MTL");
            else if (Role == "Maker Officer")
                return RedirectToAction("MO");
            else if (Role == "Checker Manager")
                return RedirectToAction("CM");
            else if (Role == "Checker TeamLeader")
                return RedirectToAction("CTL");
            else if (Role == "Checker Officer")
                return RedirectToAction("CO");
            else if (Role == "Higher Official")
                return RedirectToAction("HO");
            else return RedirectToAction("Admin");

            // if (role.Role.Name == "Relation Manager")
            //     return RedirectToAction("RM");
            // else if (role.Role.Name == "Maker Manager")
            //     return RedirectToAction("MM");
            // else if (role.Role.Name == "Maker TeamLeader")
            //     return RedirectToAction("MTL");
            // else if (role.Role.Name == "Maker Officer")
            //     return RedirectToAction("MO");
            // else if (role.Role.Name == "Checker Manager")
            //     return RedirectToAction("MM");
            // else if (role.Role.Name == "Checker TeamLeader")
            //     return RedirectToAction("MM");
            // else if (role.Role.Name == "Checker Officer")
            //     return RedirectToAction("MM");
            // else return RedirectToAction("Admin");
        }
    }
}
