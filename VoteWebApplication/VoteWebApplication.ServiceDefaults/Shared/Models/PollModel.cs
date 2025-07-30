using System.ComponentModel.DataAnnotations;

namespace VoteWebApplication.ServiceDefaults.Shared.Models
{
    public class PollModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(120)]
        public string? Question { get; set; }

        [MinLength(2), MaxLength(5)]
        public List<PollOptionModel> Options { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? User { get; set; }
    }
}
