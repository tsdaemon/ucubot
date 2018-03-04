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
            var conn  = new MySqlConnection(connectionString);
            conn.Open();
            var query = new MySqlDataAdapter("SELECT * FROM lesson_signal", conn);
            var dataset = new DataSet();
            query.Fill(dataset, "lesson_signal");
            conn.Close();
            var Signals = new List<LessonSignalDto>();
            foreach (DataRow row in dataset.Tables[0].Rows){
            	var signalDto = new LessonSignalDto
            	{
            		Timestamp = (DateTime) row["timestamp_"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
            	};
            	Signals.Add(signalDto);
            }
            return Signals;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn  = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", conn);
            command.Parameters.AddWithValue("id", id);
            var adapter = new MySqlDataAdapter(command);
            
            var dataset = new DataSet();
                
            adapter.Fill(dataset, "lesson_signal");
            if (dataset.Tables[0].Rows.Count < 1)
                return null;
                
            var row = dataset.Tables[0].Rows[0];
            var signalDto = new LessonSignalDto
            {
             	Timestamp = (DateTime) row["timestamp_"],
                Type = (LessonSignalType) row["signal_type"],
                UserId = (string) row["user_id"]
            };
            conn.Close();
            return signalDto;

            
            
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn  = new MySqlConnection(connectionString);
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText ="INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
            command.Parameters.AddRange(new[]
                {
                	new MySqlParameter("userId", userId),
                    new MySqlParameter("signalType", signalType)
                });
            await command.ExecuteNonQueryAsync();
            conn.Close();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn  = new MySqlConnection(connectionString);
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText ="DELETE FROM lesson_signal WHERE ID = @id;";
            command.Parameters.Add(new MySqlParameter("id", id));
            await command.ExecuteNonQueryAsync();
            conn.Close();
            return Accepted();
        }
    }
}
