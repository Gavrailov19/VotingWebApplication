using Microsoft.AspNetCore.SignalR;
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

        public async Task<bool> CreatePoll(string pollId, CreateRequestPollModel req)
        {
            IVoteGrain? grain = _client.GetGrain<IVoteGrain>(pollId);
            bool isCreatedPollValid = await grain.CreatePoll(req.Date, req.Options, req.User ?? Context.ConnectionId);
            if (!isCreatedPollValid)
            {
                return false;    
            } 

            await Groups.AddToGroupAsync(Context.ConnectionId, pollId);

            Dictionary<string,int> votingResults = await grain.GetVotingCount();
            PollResultModel model = new()
            {
                Id = pollId,
                Question = req.Question!,
                Options = req.Options.Select(option => new PollOptionModel { Text = option, Votes = votingResults[option] }).ToList(),
                CreatedAt = DateTime.UtcNow,
                User = req.User!,
                Results = votingResults
            };

            await Clients.Group(pollId).SendAsync("PollCreated", model);
            return true;
        }

        public async Task JoinPoll(string pollId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, pollId);
            IVoteGrain? grain = _client.GetGrain<IVoteGrain>(pollId);
            Dictionary<string, int> votingResults = await grain.GetVotingCount();

            await Clients.Caller.SendAsync("ReceiveResults", votingResults, pollId);
        }

        public async Task Vote(string pollId, string user, string option)
        {
            IVoteGrain? grain = _client.GetGrain<IVoteGrain>(pollId);
            if (!await grain.Vote(user, option)) return;

            Dictionary<string, int> votingResults = await grain.GetVotingCount();

            await Clients.Group(pollId).SendAsync("ReceiveResults", votingResults, pollId);
        }
    }
}