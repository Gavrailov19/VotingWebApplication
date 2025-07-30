using Orleans;
using VoteWebApplication.ServiceDefaults.Shared;

namespace VoteWebApplication.ApiService.Grains
{
    public sealed class VoteGrain : Grain, IVoteGrain
    {
        private bool _created;
        private List<string> _places = new();
        private readonly Dictionary<string, int> _tally = new();
        private readonly Dictionary<string, string> _userVotes = new();

        public Task<bool> CreatePoll(DateOnly date, List<string> places, string createdBy)
        {
            if (_created) return Task.FromResult(false);
            _created = true;
            _places = places.ToList();
            _tally.Clear();
            foreach (var p in _places) _tally[p] = 0;
            _userVotes.Clear();
            return Task.FromResult(true);
        }

        public Task<bool> Vote(string user, string place)
        {
            if (!_created || !_tally.ContainsKey(place))
            {
                return Task.FromResult(false);
            }

            if (_userVotes.TryGetValue(user, out var prev) && prev == place)
            {
                return Task.FromResult(false);
            }

            if (_userVotes.TryGetValue(user, out prev))
            {
                _tally[prev] = Math.Max(0, _tally[prev] - 1);
            }

            _tally[place]++;
            _userVotes[user] = place;
            return Task.FromResult(true);
        }

        public Task<Dictionary<string, int>> GetVotingCount() => Task.FromResult(_tally.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }
}
