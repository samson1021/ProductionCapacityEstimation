namespace mechanical.Models.Dto.UserDto
{
    public class UserReturnDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string emp_ID { get; set; }
        public string Email { get; set; }
        public string PhoneNO { get; set; }
        public string? Branch { get; set; }
        public string? Role { get; set; }
        public string? District { get; set; }
        public string? Department { get; set; }
    }
}
