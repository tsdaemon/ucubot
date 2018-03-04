using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var mySqlConnection = new MySqlConnection(connectionString))
            {  
                var dataset = new DataSet();
                mySqlConnection.Open();
                var tableInfo = new MySqlDataAdapter("SELECT * FROM lesson_signal", mySqlConnection);
                /*Fill our dataset with info*/
                tableInfo.Fill(dataset, "lesson_signal");
                /*go through each row saving user id, signal type id,
                 id and timestamp converting/casting it to needed format*/
                var rows = dataset.Tables[0].Rows;
                mySqlConnection.Close();
                foreach (DataRow eachDataRow in rows){
                    var lessonSignalDto = new LessonSignalDto{
                        UserId = (string) eachDataRow["user_id"],
                        Type = (LessonSignalType)Convert.ToInt32(eachDataRow["signal_type"]),
                        Id = (int) eachDataRow["id"],
                        Timestamp = (DateTime) eachDataRow["timestamp_"]
                        };
                    yield return lessonSignalDto;
                }
            }
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            /*Will be alsmost the same as ShowSignals() but with using id
             as identitficator to decide to choose or not to choose a piece of info*/
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var mySqlConnection = new MySqlConnection(connectionString))
            {
                var dataset = new DataSet();
                mySqlConnection.Open();
                var mySqlCommand = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", mySqlConnection);
                mySqlCommand.Parameters.AddWithValue("id", id);
                var tableInfo = new MySqlDataAdapter(mySqlCommand);
                tableInfo.Fill(dataset, "lesson_signal");
                var rows = dataset.Tables[0].Rows;
                mySqlConnection.Close();
                /*If as in the tip in the instructions*/
                if (rows.Count >= 1)
                {
                    return new LessonSignalDto{UserId = (string) rows[0]["user_id"], Type = (LessonSignalType) rows[0]["signal_type"], Timestamp = (DateTime) rows[0]["timestamp_"]};;
                }
                return null;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            /*
             Close to before but with insertion
             */
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var userId = message.user_id;
            using (var mySqlConnection = new MySqlConnection(connectionString))
            {
                var dataset = new DataSet();
                mySqlConnection.Open();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
                mySqlCommand.Parameters.AddRange(new[]{new MySqlParameter("userId", userId), new MySqlParameter("signalType", signalType)});
                await mySqlCommand.ExecuteNonQueryAsync();
                mySqlConnection.Close();
            }
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            /*
             Close to before but with DELETE
             */
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var mySqlConnection = new MySqlConnection(connectionString))
            {
                mySqlConnection.Open();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "DELETE FROM lesson_signal WHERE ID = @id;";
                mySqlCommand.Parameters.Add(new MySqlParameter("id", id));
                await mySqlCommand.ExecuteNonQueryAsync();
                mySqlConnection.Close();
            }
            return Accepted();
        }
        /*
        Bad idea - delete before submitting
        public MySqlCommand SetMySqlCommandsTextAndParameters(MySqlCommand mySqlCommand, string text, string parameter)
        {
            mySqlCommand.CommandText = text;
            return mySqlCommand;
        }*/
    }
}
