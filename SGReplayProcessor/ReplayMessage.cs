using System;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace SGReplayProcessor
{
    public class ReplayMessage
    {
        public readonly string tableDropString = "DROP TABLE IF EXISTS `ReplayMessages`;";

        public readonly string tableCreateString = "CREATE TABLE `ReplayMessages` (`replayCreated`	varchar(300),`playernames`	varchar(300),`rnddata`	VARBINARY(268000),`inidata`	varchar(300),`id`	varchar(300),'submission'   bigint unsigned,`resolved`	boolean,`winner`  varchar(300));";

        public string tableInsertString() {return "INSERT INTO `ReplayMessages` (`replayCreated`, `playernames`, `rnddata`, `inidata`, `id`, `resolved`, `winner`) VALUES(`" + this.replayCreated + "`, `" + this.playernames[0] + " " + this.playernames[1] + "', '" + Google.Protobuf.ByteString.CopyFrom(this.rnddata) + "', '" + this.inidata + "', '', FALSE, '');"; }
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
        int getSubmissionNum() {
            string cs = @"server=localhost;userid=dbuser;password=s$cret;database=testdb";//change
            using var con = new MySqlConnection(cs);
            con.Open();
            var isPresent = "SELECT * FROM `ReplayMessages` WHERE `playernames` LIKE `%" + playernames[0] + " " + playernames[1] + "%` AND `replayCreated` LIKE `" + replayCreated.Substring(0, 17) + "%` OR `playernames` LIKE `%" + playernames[1] + " " + playernames[0] + "%` AND `replayCreated` LIKE `" + replayCreated.Substring(0, 17) + "%`;";
            var stm = tableInsertString();
            var cmd = new MySqlCommand(isPresent, con);

            var rowsAffected = cmd.ExecuteReader();
            bool contained = rowsAffected.Read();
            if (contained)
            {
                string getSubmissionNum = "SELECT MAX(`submission`) FROM `ReplayMessages` WHERE `id` = "+id;
                cmd = new MySqlCommand(getSubmissionNum, con);

                return ((int)cmd.ExecuteReader().GetValue(0))+1;
            }
            else { 
            return 0;
            }
        }
        public bool addToDB() {
            this.submissionNum = getSubmissionNum();
            string cs = @"server=localhost;userid=dbuser;password=s$cret;database=testdb";//change
            using var con = new MySqlConnection(cs);
            con.Open();
            var isPresent = "SELECT * FROM `ReplayMessages` WHERE `playernames` LIKE `%" + playernames[0] + " " + playernames[1] + "%` AND `replayCreated` LIKE `" + replayCreated.Substring(0, 17) + "%` OR `playernames` LIKE `%" + playernames[1] + " " + playernames[0] + "%` AND `replayCreated` LIKE `" + replayCreated.Substring(0, 17) + "%`;";
            var stm = tableInsertString();
            var cmd = new MySqlCommand(isPresent, con);

            var rowsAffected = cmd.ExecuteReader();
            bool contained = rowsAffected.Read();
            if (!contained) {
                cmd = new MySqlCommand(stm, con);
                cmd.ExecuteNonQuery();
            }
            return !contained;
        }
        string replayCreated;
        string[] playernames;
        byte[] rnddata;
        string inidata;
        string id;//submittersteamid
        int submissionNum;//submission number
        bool resolved;
        string winner;
    }
}
