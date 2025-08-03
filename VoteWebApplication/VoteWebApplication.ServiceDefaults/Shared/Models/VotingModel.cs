using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteWebApplication.ServiceDefaults.Shared.Models
{
    public class VotingModel
    {
        public string Id { get; set; } = string.Empty;
        public string UsernameOfTheVoter { get; set; } = string.Empty;
        public string SelectedOption { get; set; } = string.Empty;
    }
}
