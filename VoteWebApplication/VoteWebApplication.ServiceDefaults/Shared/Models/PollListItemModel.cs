using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteWebApplication.ServiceDefaults.Shared.Models
{
    public class PollListItemModel
    {
        public string? Id { get; set; }
        public string? Question { get; set; }
        public List<PollOptionModel> Options { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, int> Results { get; set; } = new();
        public string? CreatedByUser { get; set; }
        public string? LoggedUserChoice { get; set; }
        public bool isVoteActive { get; set; }
    }
}
