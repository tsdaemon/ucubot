using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MySql.Data.MySqlClient;
using SQLitePCL;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]/:[id]")]
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
            var lessonSignalArray = new List<LessonSignalDto>();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            var adapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", conn);

            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");
            var table = dataset.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                var signal = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["timestamp_"],
                    Type = (LessonSignalType) row["SignalType"],
                    UserId = row["UserId"].ToString()
                };
                lessonSignalArray.Add(signal);
            }

            conn.Close();
            return lessonSignalArray;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = " + id + ";", conn);
            var adapter = new MySqlDataAdapter(command);
                
            var dataset = new DataSet();

            adapter.Fill(dataset, "'lesson_signal");
            var table = dataset.Tables[0];
            if (table.Rows.Count < 1)
            {
                return null;
            }

            var row = table.Rows[0];
              
            var lessonSignalDto = new LessonSignalDto
            {
                Timestamp = (DateTime) row["timestamp_"],
                Type = (LessonSignalType) row["SignalType"],
                UserId = row["UserId"].ToString()
            };
            
            return lessonSignalDto;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();

            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var conn = new MySqlConnection(connectionString);
            
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = "INSERT INTO lesson_signal (UserId, SignalType) Values (@UserId, @SignalType);";
            command.Parameters.AddRange(new[]
            {
                new MySqlParameter("UserId", userId),
                new MySqlParameter("SignalType", signalType)
            });
            await command.ExecuteNonQueryAsync();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = "DELETE FROM lesson_signal WHERE ID = @id;";
            command.Parameters.Add(new MySqlParameter("id", id));
            await command.ExecuteNonQueryAsync();
            
            return Accepted();
        }
    }
}
