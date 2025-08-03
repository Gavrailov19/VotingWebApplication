using Microsoft.AspNetCore.SignalR;
using Orleans;
using VoteWebApplication.ServiceDefaults.Shared;
using VoteWebApplication.ServiceDefaults.Shared.Models;

namespace VoteWebApplication.ApiService.Voting
{
    public class VotingHub : Hub
    {
        private readonly IClusterClient _client;
        public VotingHub(IClusterClient client)
        {
            _client = client;
        }

        public async Task RegisterPoll(string pollId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, pollId);
            await GetVotes(pollId);
        }

        public async Task DeregisterPoll(string pollId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, pollId);
        }

        public async Task RegisterVote(string pollId, string user, string option)
        {
            IVoteGrain? grain = _client.GetGrain<IVoteGrain>(pollId);
            await grain.Vote(user, option);
            await GetVotes(pollId, grain);
        }

        public async Task GetVotes(string pollId, IVoteGrain? grain = null)
        {
            if (grain is null)
            {
                grain = _client.GetGrain<IVoteGrain>(pollId);
            }
            Dictionary<string, int> votingResults = await grain.GetVotingCount();
            await Clients.Group(pollId).SendAsync("ReceiveVotes", votingResults, pollId);
        }
    }
}