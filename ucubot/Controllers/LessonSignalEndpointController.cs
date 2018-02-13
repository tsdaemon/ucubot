using System;
using System.Collections.Generic;
using System.Data;
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

            var cnn = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            try
            {
                Console.WriteLine("Connected");
                var cmd = new MySqlCommand("SELECT * FROM lesson_signal", cnn);
                var da = new MySqlDataAdapter(cmd);
                da.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            var lessonSignalDtos = new List<LessonSignalDto>();
            foreach (DataRow row in dataTable.Rows)
            {
                var lessonSignalDto = new LessonSignalDto
                {
                    UserId = (string) row["user_id"],
                    Timestamp = Convert.ToDateTime(row["timestamp"]),
                    Type = (LessonSignalType) row["signal_type"],
                    Id = (int) row["id"]
                };
                lessonSignalDtos.Add(lessonSignalDto);
            }
            return lessonSignalDtos;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {   
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var cnn = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=@id", cnn);
                cmd.Parameters.AddWithValue("@id", id);
                var da = new MySqlDataAdapter(cmd);
                da.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            var row = dataTable.Rows[0];
            var lessonSignalDto = new LessonSignalDto
            {
                UserId = (string) row["user_id"],
                Timestamp = Convert.ToDateTime(row["timestamp"]),
                Type = (LessonSignalType) row["signal_type"],
                Id = (int) row["id"]
            };
            return lessonSignalDto;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            
            MySqlConnection cnn = new MySqlConnection(connectionString);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                cnn.Open();
                var cmd = new MySqlCommand("INSERT INTO lesson_signal (signal_type, user_id, timestamp) " +
                                                    "VALUES (@signalType, @userId, @timestamp)",
                    cnn);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.AddWithValue("@signalType", signalType);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            cnn.Close();
            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var cnn = new MySqlConnection(connectionString);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                cnn.Open();
                var cmd = new MySqlCommand("DELETE FROM lesson_signal WHERE id=@id",
                    cnn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            cnn.Close();
            return Accepted();
        }
    }
}
