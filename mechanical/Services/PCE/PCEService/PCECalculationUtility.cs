
namespace mechanical.Services.PCE.PCEService
{
    public static class PCECalculationUtility
    {
        public static decimal? CalculatePerShiftProduction(int? effectiveProductionHourPerShift, decimal? productionPerHour)
        {
            return effectiveProductionHourPerShift.HasValue && productionPerHour.HasValue 
                ? effectiveProductionHourPerShift.Value * productionPerHour.Value 
                : (decimal?)0;
        }

        public static decimal? CalculatePerDayProduction(int? shiftsPerDay, decimal? perShiftProduction)
        {
            return shiftsPerDay.HasValue && perShiftProduction.HasValue 
                ? shiftsPerDay.Value * perShiftProduction.Value 
                : (decimal?)0;
        }

        public static decimal? CalculatePerMonthProduction(int? workingDaysPerMonth, decimal? perDayProduction)
        {
            return workingDaysPerMonth.HasValue && perDayProduction.HasValue 
                ? workingDaysPerMonth.Value * perDayProduction.Value 
                : (decimal?)0;
        }

        public static decimal? CalculatePerYearProduction(decimal? perMonthProduction)
        {
            return perMonthProduction.HasValue 
                ? perMonthProduction.Value * 12 
                : (decimal?)0;
        }
    }
}