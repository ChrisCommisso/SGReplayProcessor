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
        public async Task<string> Post()
        {
            Microsoft.AspNetCore.Http.IFormCollection form;
            form = ControllerContext.HttpContext.Request.Form;
            ReplayMessage replayMessage = new ReplayMessage();
            
            Microsoft.Extensions.Primitives.StringValues playernames;
            form.TryGetValue("playernames", out playernames);
            Microsoft.Extensions.Primitives.StringValues id;
            form.TryGetValue("id", out id);
            Microsoft.Extensions.Primitives.StringValues submission;
            form.TryGetValue("submissionnum", out submission);
            Microsoft.Extensions.Primitives.StringValues replayCreated;
            form.TryGetValue("replayCreated", out replayCreated);
            replayMessage.winner = "";
            replayMessage.replayCreated = JsonConvert.DeserializeObject<string>(replayCreated.ToString());
            if (form.Files[0].Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    form.Files[0].CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    replayMessage.rnddata = System.Text.Encoding.Latin1.GetString(fileBytes);
                    // act on the Base64 data
                }
            }
            if (form.Files[1].Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    form.Files[1].CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    replayMessage.inidata = System.Text.Encoding.Latin1.GetString(fileBytes);
                    // act on the Base64 data
                }
            }
            replayMessage.playernames = JsonConvert.DeserializeObject<string[]>(playernames.ToString());
            replayMessage.id = JsonConvert.DeserializeObject<string>(id.ToString()); ;
            replayMessage.submissionNum = ulong.Parse(JsonConvert.DeserializeObject<string>(submission.ToString()));
            //ReplayMessage message = JsonConvert.DeserializeObject<ReplayMessage>();
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
            bool success = replayMessage.addToDB(con,con1,con2,con3);
            con.Close();
            con1.Close();
            con2.Close();
            con3.Close();

            _logger.LogDebug("add success: " + true);
            return JsonConvert.SerializeObject(true);
        }

    }
}