using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteWebApplication.ServiceDefaults.Shared.Models
{
    public class CreateRequestPollModel
    {
        public string? Question { get; set; }

        public List<string> Options { get; set; } = new();

        public string? CreatedByUser { get; set; }
    }
}
