using Orleans;
using VoteWebApplication.ServiceDefaults.Shared;

namespace VoteWebApplication.ApiService.Grains
{
    public sealed class VoteGrain : Grain, IVoteGrain
    {
        private bool _created;
        private List<string> _options = new();
        private readonly Dictionary<string, int> _voteCountByOption = new();
        private readonly Dictionary<string, string> _userVotes = new();

        public Task<bool> CreatePoll(List<string> options, string createdBy)
        {
            if (_created)
            {
                return Task.FromResult(false);
            }
            _created = true;
            _options = options.ToList();

            foreach (var option in _options)
            {
                _voteCountByOption[option] = 0;
            }

            return Task.FromResult(true);
        }

        public Task<bool> Vote(string user, string place)
        {
            if (!_created || !_voteCountByOption.ContainsKey(place))
            {
                return Task.FromResult(false);
            }

            if (_userVotes.TryGetValue(user, out var prev) && prev == place)
            {
                return Task.FromResult(false);
            }

            if (_userVotes.TryGetValue(user, out prev))
            {
                _voteCountByOption[prev] = Math.Max(0, _voteCountByOption[prev] - 1);
            }

            _voteCountByOption[place]++;
            _userVotes[user] = place;
            return Task.FromResult(true);
        }

        public Task<Dictionary<string, int>> GetVotingCount()
        {
            return Task.FromResult(_voteCountByOption);
        }

        public Task<string?> GetUserVote(string? user)
        {
            return Task.FromResult(_userVotes.TryGetValue(user!, out var vote) ? vote : null);
        }
    }
}
