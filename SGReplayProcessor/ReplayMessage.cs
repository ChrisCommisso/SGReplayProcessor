using System;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace SGReplayProcessor
{
    [Serializable]
    public class ReplayMessage
    {
        public ReplayMessage() { 
        
        } 
        public static void tableBackup(MySqlConnection _con) {
            //var cmd = new MySqlCommand(tableBackupString(), _con);
            //cmd.ExecuteNonQuery();
        }
        public static void tableRefresh(MySqlConnection _con) { 

        var cmd = new MySqlCommand(tableDropString(), _con);
        cmd.ExecuteNonQuery();
        cmd = new MySqlCommand(tableCreate(), _con);
        cmd.ExecuteNonQuery();
        }
        public static string tableDropString() { return "DROP TABLE IF EXISTS `ReplayMessages`;"; }
        public static string tableBackupString() { return "CREATE TABLE ReplayMessages"+DateTime.Now.ToString()+" AS SELECT * FROM ReplayMessages"; }
        public static string tableCreate() {return "CREATE TABLE `ReplayMessages` (`replayCreated`	varchar(300),`player1`	varchar(300),`player2`	varchar(300),`rnddata`	BLOB(268000),`inidata`	varchar(1000),`id`	varchar(300),`submission` bigint unsigned,`resolved`	boolean,`winner`  varchar(300),PRIMARY KEY (id, submission));"; }

        void tableInsert(MySqlConnection _con) {
            var data = rnddata;
            using (var cmd = new MySqlCommand("INSERT INTO `ReplayMessages` (`replayCreated`, `player1`, `player2`, `rnddata`, `inidata`, `id`,`submission`, `resolved`, `winner`) VALUES('" + this.replayCreated + "', '" + this.playernames[0] + "', '" + this.playernames[1] + "', @rnd, '" + this.inidata + "','"+this.id+"', "+this.submissionNum+", 0, '');",
                                              _con))
            {
                cmd.Parameters.Add("@rnd", MySqlDbType.Blob).Value = data;
                cmd.ExecuteNonQuery();

            }


            ; }

        public DateTime getCreationTime() {
            DateTime dateTime = DateTime.MinValue;
            string[] dateFormats = new[] { "MM/dd/yyyyHH:mm:ss" };
            CultureInfo provider = new CultureInfo("en-US");
            if (DateTime.TryParseExact(replayCreated, dateFormats, provider, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                return dateTime;
            }
            else if (replayCreated == "") 
            {
                return DateTime.MinValue;
            }
            else
            {
                throw new Exception("the replay datetime was incorrectly formatted");
            }
        }
        int getSubmissionNum(MySqlConnection con, MySqlConnection con1) {
            
            var isPresent = "SELECT * FROM `ReplayMessages` WHERE ((`player1` = '"+playernames[0]+ "' AND `player2` = '" + playernames[1] + "') OR (`player1` = '" + playernames[1] + "' AND `player2` = '" + playernames[0] + "')) AND `replayCreated` LIKE '" + replayCreated.Substring(0, 17) + "%';";
            
            var cmd = new MySqlCommand(isPresent, con);

            var rowsAffected = cmd.ExecuteReader();
            bool contained = rowsAffected.Read();
           
            if (contained)
            {
                string getSubmissionNum = "SELECT MAX(`submission`) FROM `ReplayMessages` WHERE `id` = "+id;
                cmd = new MySqlCommand(getSubmissionNum, con1);

                return ((int)cmd.ExecuteReader().GetValue(0))+1;
            }
            else { 
            return 0;
            }
        }
        public bool addToDB(MySqlConnection con, MySqlConnection con1, MySqlConnection con2, MySqlConnection con3) {
            
            this.submissionNum = this.getSubmissionNum(con,con1);

            var isPresent = "SELECT * FROM `ReplayMessages` WHERE ((`player1` = '" + playernames[0] + "' AND `player2` = '" + playernames[1] + "') OR (`player1` = '" + playernames[1] + "' AND `player2` = '" + playernames[0] + "')) AND `replayCreated` LIKE '" + replayCreated.Substring(0, 17) + "%';";

            var cmd = new MySqlCommand(isPresent, con2);

            var rowsAffected = cmd.ExecuteReader();
            bool contained = rowsAffected.Read();
            if (!contained) {
                tableInsert(con3);
            }
            return !contained;
        }
        public string replayCreated;
        public string[] playernames;
        public string rnddata;
        public string inidata;
        public string id;//submittersteamid
        public int submissionNum;//submission number
        public bool resolved;
        public string winner;
    }
}
