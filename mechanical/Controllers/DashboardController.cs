﻿using mechanical.Data;
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
        public DashboardController(ICaseService caseService, CbeContext cbeContext, IPCECaseService pCECaseService)
        {
            _caseService = caseService;
            _cbeContext = cbeContext;
            _PCECaseService = pCECaseService;
        }
        public async Task<IActionResult> RM()
        {
            var latestCase = await _caseService.GetRmLatestCases(base.GetCurrentUserId());
            var newCases = await _PCECaseService.GetRmLatestPCECases(base.GetCurrentUserId());
            ViewData["NewCases"] = newCases;
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
    
        public async Task<IActionResult> HO()
        {
            var latestCase = await _caseService.GetMmLatestCases(base.GetCurrentUserId());
            return View(latestCase);
        }
    }

}
