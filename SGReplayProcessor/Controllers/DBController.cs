using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace SGReplayProcessor.Controllers
{
    
    [ApiController]
    [Route("DB")]
    public class DBController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<ReplayController> _logger;

        public DBController(ILogger<ReplayController> logger)
        {
            _logger = logger;
        }
        

        [HttpGet(Name = "RefreshDB")]
        public bool RefreshDB()
        {
            string cs = @"server=localhost;userid=root;password=VictoryOverEvil8610;database=replaydatabase";//change
            var con = new MySqlConnection(cs);
            con.Open();
            ReplayMessage.tableBackup(con);
            ReplayMessage.tableRefresh(con);
            con.Close();
            return true;
        }

    }
}