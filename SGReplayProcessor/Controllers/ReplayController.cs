using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

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
        public string Post(object replayMessage)
        {
            ReplayMessage message = JsonConvert.DeserializeObject<ReplayMessage>(replayMessage.ToString());
            _logger.LogTrace("attempting to add replay: " + replayMessage);
            string cs = @"server=localhost;userid=root;password=VictoryOverEvil8610;database=replaydatabase";//change
            var con = new MySqlConnection(cs);
            var con1 = new MySqlConnection(cs);
            var con2 = new MySqlConnection(cs);
            var con3 = new MySqlConnection(cs);
            con.Open();
            con1.Open();
            con2.Open();
            con3.Open();
            bool success = message.addToDB(con,con1,con2,con3);
            con.Close();
            con1.Close();
            con2.Close();
            con3.Close();

            _logger.LogDebug("add success: " + true);
            return JsonConvert.SerializeObject(replayMessage);
        }

    }
}