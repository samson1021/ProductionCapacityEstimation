namespace mechanical.Models.Dto.MotorVehicleDto
{
    public class CaseCommenAttributeDto
    {
        public string PropertyOwner { get; set; } =  string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid CollateralId { get; set; }

    }
}
