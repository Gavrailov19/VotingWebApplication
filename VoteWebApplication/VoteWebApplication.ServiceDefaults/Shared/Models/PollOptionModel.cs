namespace VoteWebApplication.ServiceDefaults.Shared.Models
{
    public class PollOptionModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Votes { get; set; }
    }
}
