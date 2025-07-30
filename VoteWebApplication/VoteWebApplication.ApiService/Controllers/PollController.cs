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
        private readonly IHubContext<VotingHub> _hubContext;

        // In-memory poll store
        private static readonly ConcurrentDictionary<string, PollModel> _polls = new();

        public PollController(IGrainFactory grains, IHubContext<VotingHub> hubContext)
        {
            _grains = grains;
            _hubContext = hubContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromQuery] string id, [FromBody] CreateRequestPollModel req)
        {
            if (req.Options.Count < 2 || req.Options.Count > 5)
            {
                return BadRequest("Need 2–5 options");
            }

            IVoteGrain? grain = _grains.GetGrain<IVoteGrain>(id);
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            bool IsPollValid = await grain.CreatePoll(today, req.Options, req.User!);

            if (!IsPollValid)
            {
                return Conflict("Poll already exists or invalid state.");
            }

            var model = new PollModel
            {
                Id = Guid.Parse(id),
                Question = req.Question,
                CreatedAt = DateTime.UtcNow,
                Options = req.Options.Select(o => new PollOptionModel { Id = Guid.NewGuid(), Text = o, Votes = 0 }).ToList(),
                User = req.User
            };
            _polls[id] = model;

            await _hubContext.Clients.Group(id).SendAsync("PollCreated", new PollResultModel
            {
                Id = id,
                Question = model.Question!,
                Options = model.Options,
                CreatedAt = model.CreatedAt,
                User = model.User!,
                Results = model.Options.ToDictionary(o => o.Text, _ => 0)
            });

            return Ok(model);
        }

        [HttpPost("vote")]
        public async Task<IActionResult> Vote([FromQuery] string id, [FromQuery] string user, [FromBody] string option)
        {
            var grain = _grains.GetGrain<IVoteGrain>(id);
            bool isUserVotedValid = await grain.Vote(user, option);
            if (!isUserVotedValid)
            {
                return BadRequest("Vote not allowed");
            }

            var votingCount = await grain.GetVotingCount();
            await _hubContext.Clients.Group(id).SendAsync("ReceiveResults", votingCount, id);

            return Ok();
        }

        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<PollListItemModel>>> GetVotingPolls()
        {
            var list = new List<PollListItemModel>();
            foreach (var poll in _polls)
            {
                var results = await _grains.GetGrain<IVoteGrain>(poll.Key).GetVotingCount();
                list.Add(new PollListItemModel
                {
                    Id = poll.Key,
                    Question = poll.Value.Question,
                    Options = poll.Value.Options,
                    CreatedAt = poll.Value.CreatedAt,
                    User = poll.Value.User,
                    Results = results
                });
            }
            return Ok(list);
        }
    }
}