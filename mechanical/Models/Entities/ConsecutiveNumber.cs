namespace mechanical.Models.Entities
{
    public class ConsecutiveNumber
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NextNumber { get; set; }

        public virtual User? User { get; set; }

    }
}
