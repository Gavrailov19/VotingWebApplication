using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using VoteWebApplication.ApiService.Voting;
using VoteWebApplication.ServiceDefaults.Shared;
using VoteWebApplication.ServiceDefaults.Shared.Models;

namespace VoteWebApplication.ApiService.Controllers
{
    [ApiController]
    [Route("api/poll")]
    public class PollController : ControllerBase
    {
        private readonly IGrainFactory _grains;

        // In-memory poll store
        private static readonly ConcurrentDictionary<string, PollModel> _polls = new();

        public PollController(IGrainFactory grains)
        {
            _grains = grains;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRequestPollModel req)
        {
            if (req.Options.Count < 2)
            {
                return BadRequest("Need atleast 2 options");
            }

            string pollId = Guid.NewGuid().ToString();
            IVoteGrain? grain = _grains.GetGrain<IVoteGrain>(pollId);
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            bool IsPollValid = await grain.CreatePoll(req.Options, req.CreatedByUser!);

            if (!IsPollValid)
            {
                return Conflict("Poll already exists or invalid state.");
            }

            var model = new PollModel
            {
                Id = Guid.Parse(pollId),
                Question = req.Question,
                CreatedAt = DateTime.UtcNow,
                Options = req.Options.Select(o => new PollOptionModel { Id = Guid.NewGuid(), Text = o, Votes = 0 }).ToList(),
                User = req.CreatedByUser,
                isVoteActive = true,
            };
            _polls[pollId] = model;

            return Ok(model);
        }

        [HttpGet("getall")]
        public async Task<ActionResult<PollListItemModel>> GetVotingPolls([FromQuery] string? user = null)
        {
            List<PollListItemModel> list = new();
            string? userChoice = null;
            foreach (var poll in _polls)
            {
                Dictionary<string,int> results = await _grains.GetGrain<IVoteGrain>(poll.Key).GetVotingCount();
                if (user is not null)
                {
                    userChoice = await _grains.GetGrain<IVoteGrain>(poll.Key).GetUserVote(user);
                }
                list.Add(new PollListItemModel
                {
                    Id = poll.Key,
                    Question = poll.Value.Question,
                    Options = poll.Value.Options,
                    CreatedAt = poll.Value.CreatedAt,
                    CreatedByUser = poll.Value.User,
                    Results = results,
                    LoggedUserChoice = userChoice,
                    isVoteActive = poll.Value.isVoteActive,
                });
            }
            return Ok(list);
        }

        [HttpDelete("delete")]
        public IActionResult RemoveVotingPolls([FromQuery] string pollId)
        {
            if (!_polls.TryRemove(pollId, out _))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}