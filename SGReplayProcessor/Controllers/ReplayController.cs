using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Diagnostics;

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

        [HttpGet(Name = "GetReplayMessages")]
        public string Get() {
            
            
            Microsoft.AspNetCore.Http.IFormCollection form;
            form = ControllerContext.HttpContext.Request.Form;
            Microsoft.Extensions.Primitives.StringValues playernames;
            
            Microsoft.Extensions.Primitives.StringValues id;
           
            Microsoft.Extensions.Primitives.StringValues submission;
            
            Microsoft.Extensions.Primitives.StringValues replayCreated;
            
            string str = "";
            if (form.TryGetValue("playernames", out playernames)) {
                string[] dirtyString = JsonConvert.DeserializeObject<string[]>(playernames.ToString());
                string[] cleanstring = { "", "" };
                for (int i = 0; i < 2; i++) {
                    cleanstring[i] = new String(dirtyString[i].Where(Char.IsLetterOrDigit).ToArray());
                }
                str += "SELECT * FROM `ReplayMessages` WHERE player1 LIKE '" + cleanstring[0] + "%' AND player2 LIKE '" + cleanstring[1] + "%'";
            }
            if (form.TryGetValue("id", out id))
            {
                if (str.Length > 0) {
                    str += " UNION ";
                }
                string dirtyString = JsonConvert.DeserializeObject<string>(id.ToString());
                string cleanstring = "";
                
                    cleanstring = new String(dirtyString.Where(Char.IsLetterOrDigit).ToArray());
                
                str += "SELECT * FROM `ReplayMessages` WHERE id = '"+cleanstring+"'";
            }
            if (form.TryGetValue("submissionnum", out submission))
            {
                if (str.Length > 0)
                {
                    str += " UNION ";
                }
                string dirtyString = JsonConvert.DeserializeObject<string>(submission.ToString());
                string cleanstring = "";

                cleanstring = new String(dirtyString.Where(Char.IsLetterOrDigit).ToArray());

                str += "SELECT * FROM `ReplayMessages` WHERE submission = " + cleanstring + "";
            }
            if (form.TryGetValue("replayCreated", out replayCreated))
            {
                if (str.Length > 0)
                {
                    str += " UNION ";
                }
                string dirtyString = JsonConvert.DeserializeObject<string>(replayCreated.ToString());
                string cleanstring = "";

                cleanstring = new String(dirtyString.Where(Char.IsLetterOrDigit).ToArray());

                str += "SELECT * FROM `ReplayMessages` WHERE replayCreated = " + cleanstring + "";
            }
            str += ";";
            string cs = @"server=localhost;userid=root;password=VictoryOverEvil8610;database=replaydatabase";//change
            var con = new MySqlConnection(cs);
            con.Open();
            var cmd = new MySqlCommand(str, con);
            MySqlDataReader values = cmd.ExecuteReader();
            List<ReplayMessage> messages = new List<ReplayMessage>();
            while (values.Read()) {
                ReplayMessage message = new ReplayMessage();
                message.playernames = new string[]{"",""};
                message.replayCreated = (string)values.GetValue(0);
                message.playernames[0] = (string)values.GetValue(1);
                message.playernames[1] = (string)values.GetValue(2);
                message.rnddata = (byte[])values.GetValue(3);
                message.inidata = (string)values.GetValue(4);
                message.id = (string)values.GetValue(5);
                message.submissionNum = (ulong)values.GetValue(6);
                message.resolved = (bool)values.GetValue(7);
                message.winner = (string)values.GetValue(8);
                messages.Add(message);
            }
            return JsonConvert.SerializeObject(messages);
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
                    replayMessage.rnddata = fileBytes;
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