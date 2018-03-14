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
            var mySqlConnection = new MySqlConnection(connectionString);
            var createdDataTable = new DataTable();

            try
            {
                mySqlConnection.Open();
                var mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", mySqlConnection);
                mySqlDataAdapter.Fill(createdDataTable);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(e.ToString());
                return null;
            }


            var lSDs = new List<LessonSignalDto>();

            foreach (DataRow row in createdDataTable.Rows)
            {
                var LSD = new LessonSignalDto
                {
                    UserId = (string) row["user_id"],
                    Timestamp = (DateTime) (row["timestamp"]),
                    Type = ((string) row["signal_type"]).ConvertSlackMessageToSignalType()
                };
                lSDs.Add(LSD);
            }
            mySqlConnection.Close();
            return lSDs;
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var mySqlConnection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            var createdDataTable = new DataTable();
            
            try
            {
                mySqlConnection.Open();
                var mySqlCommand = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", mySqlConnection);
                var mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
                mySqlDataAdapter.Fill(createdDataTable);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            
            if (createdDataTable.Rows.Count < 1)
                return null;
            
            var row = createdDataTable.Rows[0];
            var LSD = new LessonSignalDto
            {
                Timestamp = (DateTime) row["timestamp_"],
                Type = (LessonSignalType) row["signal_type"],
                UserId = (string) row["user_id"]
            };
            
            mySqlConnection.Close();
            return LSD;
        }

        
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            MySqlConnection mySqlConnection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));

            try
            {

                mySqlConnection.Open();
                MySqlCommand command = new MySqlCommand("INSERT INTO lesson_signal (user_id, signal_type) " +
                                                        "VALUES (@userId, @signalType);", mySqlConnection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@signalType", signalType);

                await command.ExecuteNonQueryAsync();
                mySqlConnection.Close();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

            return Accepted();
        }

        
        
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var mySqlConnection = new MySqlConnection(connectionString);
            
            try
            {
                mySqlConnection.Open();
                var mySqlCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE id=" + id, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
                        {
                        Console.WriteLine(ex.ToString());
                        }
            mySqlConnection.Close();
            return Accepted();
        }
        
    }

}
