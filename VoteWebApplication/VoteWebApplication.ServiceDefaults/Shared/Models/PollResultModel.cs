using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteWebApplication.ServiceDefaults.Shared.Models
{
    public class PollResultModel
    {
        public string Id { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public List<PollOptionModel> Options { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;
        public Dictionary<string, int> VotingResults { get; set; } = new();
        public string? LoggedUserChoice { get; set; }
        public bool isVoteActive { get; set; }
    }
}
