﻿using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Microsoft.EntityFrameworkCore;

public class PCECaseTimeLineService : IPCECaseTimeLineService
{

    private readonly CbeContext _cbeContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public PCECaseTimeLineService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _cbeContext = cbeContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PCECaseTimeLinePostDto> PCECaseTimeLine(PCECaseTimeLinePostDto pCECaseTimeLinePostDto)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var caseTimeline = _mapper.Map<PCECaseTimeLine>(pCECaseTimeLinePostDto);
        if (caseTimeline.UserId == Guid.Empty)
            caseTimeline.UserId = Guid.Parse(httpContext.Session.GetString("userId"));
        caseTimeline.CreatedAt = DateTime.Now;

        await _cbeContext.PCECaseTimeLines.AddAsync(caseTimeline);
        await _cbeContext.SaveChangesAsync();
        return _mapper.Map<PCECaseTimeLinePostDto>(caseTimeline);
    }



    //public Guid Id { get; set; }
    //public required Guid CaseId { get; set; }
    //public required Guid UserId { get; set; }
    //public DateTime CreatedAt { get; set; }
    //public required string Activity { get; set; }
    //public required string CurrentStage { get; set; }
    //public virtual PCECase? NewCase { get; set; }
    //public virtual CreateUser? User { get; set; }


    public async Task<IEnumerable<PCECaseTimeLineReturnDto>> GetPCECaseTimeLines(Guid CaseId)
    {
        var caseTimelines = await _cbeContext.PCECaseTimeLines.Where(a=>a.CaseId == CaseId).Include(res => res.User).ThenInclude(res => res.Role).OrderBy(res => res.CreatedAt).ToListAsync(); 

        return _mapper.Map<IEnumerable<PCECaseTimeLineReturnDto>> (caseTimelines); 
    }

    //public async Task<IEnumerable<CaseTimeLineReturnDto>> GetCaseTimeLines(Guid CaseId)
    //{
    //    var caseTimelines = await _cbeContext.CaseTimeLines.Where(res => res.CaseId == CaseId).Include(res => res.User).ThenInclude(res => res.Role).OrderBy(res => res.CreatedAt).ToListAsync();
    //    return _mapper.Map<IEnumerable<CaseTimeLineReturnDto>>(caseTimelines);
    //}

}


