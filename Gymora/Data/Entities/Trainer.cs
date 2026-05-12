using System.ComponentModel.DataAnnotations;

namespace Gymora.Data.Entities
{
    public class Trainer
    {
        [Key]
        public Guid TrainerId { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
