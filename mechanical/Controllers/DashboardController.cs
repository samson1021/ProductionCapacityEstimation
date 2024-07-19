using mechanical.Data;
using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly CbeContext _cbeContext;
        public DashboardController(ICaseService caseService, CbeContext cbeContext)
        {
            _caseService = caseService;
            _cbeContext = cbeContext;
        }
        public async Task<IActionResult> RM()
        {
            var latestCase = await _caseService.GetRmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> MO()
        {
            var latestCase = await _caseService.GetMoLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        public async Task<IActionResult> MM()
        {
           var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
        
        //public async Task<IActionResult> RoleBased()
        //{
        //    var currentUser = base.GetCurrentUserId();
        //    var role = await _cbeContext.CreateUsers.Include(res=>res.Role).Where(res=>res.Id == currentUser).FirstOrDefaultAsync();
        //    if (role.Role.Name == "Relation Manager")
        //        return RedirectToAction("RM");

        //    else if (role.Role.Name == "Maker Manager")
        //        return RedirectToAction("MM");
        //    else if (role.Role.Name == "Maker TeamLeader")
        //        return RedirectToAction("MM");
        //    else if (role.Role.Name == "Maker Officer")
        //        return RedirectToAction("MM");
        //    else if (role.Role.Name == "Checker Manager")
        //        return RedirectToAction("MM");
        //    else if (role.Role.Name == "Checker TeamLeader")
        //        return RedirectToAction("MM");
        //    else if (role.Role.Name == "Checker Officer")
        //        return RedirectToAction("MM");
        //    else return RedirectToAction("Admin");

        //}
        public async Task<IActionResult> HO()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
    }

}
