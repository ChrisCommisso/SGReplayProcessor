using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SGReplayProcessor.Controllers
{
    [ApiController]
    [Route("replay")]
    public class ReplayController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<ReplayController> _logger;

        public ReplayController(ILogger<ReplayController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostReplayMessage")]
        public bool Post(ReplayMessage replayMessage)
        {
            _logger.LogTrace("attempting to add replay: "+replayMessage);
            bool success = replayMessage.addToDB();
            _logger.LogDebug("add success: " + success);
            return success;
        }
    }
}