using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Rewrite.Internal.PatternSegments;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;
        private string connectionString;
        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("BotDatabase");
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            sqlConnection.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM lesson_signal;", sqlConnection);
            MySqlDataAdapter query = new MySqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();
            query.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                LessonSignalDto signalDto = new LessonSignalDto();
                signalDto.Id = (int) row["id"];
                signalDto.Timestamp = (DateTime) row["timestamp_"];
                signalDto.Type = SignalTypeUtils.ConvertSlackMessageToSignalType( (string) row["signal_type"])  ;
                signalDto.UserId = (string) row["user_id"];

                yield return signalDto;
            }

            sqlConnection.Close();      

        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            /*
            LessonSignalDto lessonSignalDto = new LessonSignalDto();
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select * from lesson_signal where id='@id';",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            lessonSignalDto.Id = reader.GetInt32("id");
                            lessonSignalDto.Timestamp = reader.GetDateTime("Timestamp");
                            lessonSignalDto.UserId = reader.GetString("UserId");
                            lessonSignalDto.Type =
                                SignalTypeUtils.ConvertSlackMessageToSignalType(reader.GetString("signal_type"));

                        }

                    }
                }

            



            return lessonSignalDto;*/
            
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            
            mySqlConnection.Open();
            
            string commandString = "SELECT * FROM lesson_signal WHERE id =@id;";
            
            MySqlCommand command = new MySqlCommand(commandString, mySqlConnection);
            
            command.Parameters.AddWithValue("id", id);
            
            MySqlDataAdapter query = new MySqlDataAdapter(command);
            
            DataSet dataSet = new DataSet();
            
            query.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count < 1)
            {
                return null;
            }

            var row = dataSet.Tables[0].Rows[0];
            LessonSignalDto signalDto = new LessonSignalDto();
            
            signalDto.Id = (int) row["id"];
            signalDto.Timestamp = (DateTime) row["timestamp_"];
            signalDto.Type = SignalTypeUtils.ConvertSlackMessageToSignalType((string) row["signal_type"]);
            signalDto.UserId = (string) row["user_id"];
            
            mySqlConnection.Close();
            
            return signalDto;
   
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            LessonSignalType signalType;
            try
            {
                signalType = message.text.ConvertSlackMessageToSignalType();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Forbid();
            }
            


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                String str =
                    "INSERT INTO lesson_signal (signalType, Timestamp, userId) VALUES (@signalType, @Date, @userId)";
                
                using (MySqlCommand cmd = new MySqlCommand(str, connection))
                {
                    
                    cmd.Parameters.AddWithValue("@signalType", signalType);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    if (cmd.ExecuteNonQuery() == -1)
                    {
                        return Forbid();
                    }
                }
                connection.Close();
            }



            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            if (id <= 0)
            {
                return Forbid();
            }            


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("delete * from lesson_signal where id='@id';",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQueryAsync();
                }
                connection.Close();

            }

            return Accepted();
        }
    }
}
    
