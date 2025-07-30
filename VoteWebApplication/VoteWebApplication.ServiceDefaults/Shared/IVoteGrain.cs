using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteWebApplication.ServiceDefaults.Shared
{
    public interface IVoteGrain : IGrainWithStringKey
    {
        Task<bool> CreatePoll(DateOnly date, List<string> places, string createdBy);
        Task<bool> Vote(string user, string place);
        Task<Dictionary<string, int>> GetVotingCount();
    }
}
