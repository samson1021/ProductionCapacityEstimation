using AutoMapper;
using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.AnnexService;
using mechanical.Services.CaseTimeLineService;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml;

namespace mechanical.Services.MotorVehicleService
{
    public class MotorVehicleService : IMotorVehicleService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IAnnexService _motorVehicleAnnexService;
        private readonly ICaseTimeLineService _caseTimeLineService;
        public MotorVehicleService(CbeContext cbeContext, IMapper mapper, ICaseTimeLineService caseTimeLineService, IAnnexService motorVehicleAnnexService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _motorVehicleAnnexService = motorVehicleAnnexService;
            _caseTimeLineService = caseTimeLineService;
        }
        public async Task<MotorVehicle> CreateMotorVehicle( Guid userId, CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = _mapper.Map<MotorVehicle>(createMotorVehicleDto);

            var collateral = await _cbeContext.Collaterals.FindAsync(motorVehicle.CollateralId);

            motorVehicle.MarketShareFactor = await _motorVehicleAnnexService.GetMOVMarketShareFactor(motorVehicle.MotorVehicleMake, motorVehicle.BodyType);
            motorVehicle.DepreciationRate = await _motorVehicleAnnexService.GetMOVDepreciationRate(DateTime.Now.Year - motorVehicle.YearOfManufacture, motorVehicle.BodyType);
            motorVehicle.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(motorVehicle.CurrentEqpmntCondition, motorVehicle.AllocatedPointsRange);
            motorVehicle.ReplacementCost = (motorVehicle.InvoiceValue * motorVehicle.ExchangeRate);
            motorVehicle.NetEstimationValue = motorVehicle.MarketShareFactor * motorVehicle.DepreciationRate * motorVehicle.EqpmntConditionFactor * motorVehicle.ReplacementCost;
            motorVehicle.EvaluatorUserID = userId;
            _cbeContext.MotorVehicles.Add(motorVehicle);
      
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker Evaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return motorVehicle;
        }
        public async Task<double> Currency(string currency, DateTime currencyDate)
        {
            double ExchangeRate;
            try
            {
                currency = currency.ToUpper();
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
                var request = (HttpWebRequest)WebRequest.Create("http://172.31.6.113:9095/CREDITVAL1/services?xsd=3");

                request.Headers.Add("SOAPAction", " http://172.31.6.113:9095/CREDITVAL1/services?xsd=3");

                // Set the content type header
                request.ContentType = "text/xml;charset=\"utf-8\"";

                // Set the HTTP method
                request.Method = "POST";
                string soapRequest = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                                xmlns:cred=""http://temenos.com/CREDITVALUATION"">
                                <soapenv:Header/>
                                <soapenv:Body>
                                <cred:TodayExchangeRate>
                                <WebRequestCommon>
                                <company/>
                                <password>123456</password>
                                <userName>MIKIYASSDC1</userName>
                                </WebRequestCommon>
                                <CURRENCYRATETODAYType>
                                <enquiryInputCollection>
                                <columnName>ID</columnName>
                                <criteriaValue>" + currency + @"</criteriaValue>
                                <operand>EQ</operand>
                                </enquiryInputCollection>
                                </CURRENCYRATETODAYType>
                                </cred:TodayExchangeRate>
                                </soapenv:Body>
                                </soapenv:Envelope>";


                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(soapRequest);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                // Send the SOAP request and get the response
                var response = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {

                    //read xml response 
                    var soapResponse = streamReader.ReadToEnd();
                    // Parse the SOAP response
                    var doc = new XmlDocument();
                    doc.LoadXml(soapResponse);

                    // Find the element you're interested in
                    var nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("m", "http://temenos.com/CURRENCYRATETODAY"); // Add namespace mapping for the SOAP response
                    var resultElem = doc.SelectSingleNode("//m:BUYRATE", nsmgr);

                    // Get the text content of the element as a string
                    var resultText = resultElem.InnerText.Trim();

                    ExchangeRate = Convert.ToDouble(resultText);
                    return ExchangeRate;
                }

            }
            catch (System.Exception e)
            {

                return 1;
            }

        }
        public async Task<ReturnMotorVehicleDto> CheckMotorVehicle(Guid userId, Guid Id,CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = _mapper.Map<MotorVehicle>(createMotorVehicleDto);

            var collateral = await _cbeContext.Collaterals.FindAsync(motorVehicle.CollateralId);

            motorVehicle.MarketShareFactor = await _motorVehicleAnnexService.GetMOVMarketShareFactor(motorVehicle.MotorVehicleMake, motorVehicle.BodyType);
            motorVehicle.DepreciationRate = await _motorVehicleAnnexService.GetMOVDepreciationRate(DateTime.Now.Year - motorVehicle.YearOfManufacture, motorVehicle.BodyType);
            motorVehicle.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(motorVehicle.CurrentEqpmntCondition, motorVehicle.AllocatedPointsRange);
            motorVehicle.ReplacementCost = (motorVehicle.InvoiceValue * motorVehicle.ExchangeRate);
            motorVehicle.NetEstimationValue = motorVehicle.MarketShareFactor * motorVehicle.DepreciationRate * motorVehicle.EqpmntConditionFactor * motorVehicle.ReplacementCost;
            var motervechleReturn = _mapper.Map<ReturnMotorVehicleDto>(motorVehicle);
            motervechleReturn.Id = Id;
            return motervechleReturn;
        }

        public async Task<ReturnMotorVehicleDto> GetMotorVehicle(Guid Id)
        {
            var motorVehicle = await _cbeContext.MotorVehicles.Include(res=>res.Collateral).FirstOrDefaultAsync(res=>res.Id==Id);
            return _mapper.Map<ReturnMotorVehicleDto>(motorVehicle);

        }
        public async Task<ReturnMotorVehicleDto> GetMotorVehicleByCollateralId(Guid collateralId)
        {
            var motorVehicle = await _cbeContext.MotorVehicles.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == collateralId);
            return _mapper.Map<ReturnMotorVehicleDto>(motorVehicle);

        }
        public async Task<ReturnEvaluatedCaseDto> GetEvaluatedMotorVehicle(Guid Id)
        {
            CaseCommenAttributeDto caseCommenAttributeDto = new CaseCommenAttributeDto();
            ReturnEvaluatedCaseDto returnEvaluatedCaseDto = new ReturnEvaluatedCaseDto();
            var motorVehicle = await _cbeContext.MotorVehicles.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == Id);
            if (motorVehicle != null)
            {
                var motorVehicles =  _cbeContext.Collaterals.Where(res => res.Id == motorVehicle.CollateralId).FirstOrDefault();
                caseCommenAttributeDto.PropertyOwner = motorVehicles.PropertyOwner;
                caseCommenAttributeDto.Role = motorVehicles.Role;
                caseCommenAttributeDto.Type = motorVehicles.Type.ToString();
                caseCommenAttributeDto.Category = motorVehicles.Category.ToString();
                caseCommenAttributeDto.CollateralId = motorVehicle.CollateralId;
            }

            returnEvaluatedCaseDto.ReturnMotorVehicleDto = _mapper.Map< ReturnMotorVehicleDto> (motorVehicle);
            returnEvaluatedCaseDto.CaseCommenAttributeDto = caseCommenAttributeDto;

            return returnEvaluatedCaseDto;
        }


        public async Task<MotorVehicle> EditMotorVehicle(Guid Id, CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = await _cbeContext.MotorVehicles.FindAsync(Id);

            _mapper.Map(createMotorVehicleDto, motorVehicle);

            var collateral = await _cbeContext.Collaterals.FindAsync(motorVehicle.CollateralId);

            motorVehicle.MarketShareFactor = await _motorVehicleAnnexService.GetMOVMarketShareFactor(motorVehicle.MotorVehicleMake, motorVehicle.BodyType);
            motorVehicle.DepreciationRate = await _motorVehicleAnnexService.GetMOVDepreciationRate(DateTime.Now.Year - motorVehicle.YearOfManufacture, motorVehicle.BodyType);
            motorVehicle.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(motorVehicle.CurrentEqpmntCondition, motorVehicle.AllocatedPointsRange);
            motorVehicle.ReplacementCost = (motorVehicle.InvoiceValue * motorVehicle.ExchangeRate);
            motorVehicle.NetEstimationValue = motorVehicle.MarketShareFactor * motorVehicle.DepreciationRate * motorVehicle.EqpmntConditionFactor * motorVehicle.ReplacementCost;
           
            _cbeContext.Update(motorVehicle);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker Revaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return motorVehicle;
        }

    }
}
