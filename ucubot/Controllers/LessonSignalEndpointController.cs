using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
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
            
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            
            var dt = new DataTable();
            var adapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", connection);
            adapter.Fill(dt);
            
            connection.Close();
            adapter.Dispose();

            var arr = new List<LessonSignalDto>();
            
            foreach (DataRow r in dt.Rows)
            {
                var lsd = new LessonSignalDto
                {
                    Id = (int) r["id"],
                    Timestamp = (DateTime) r["time_stamp"],
                    Type = (LessonSignalType) Convert.ToInt32(r["signal_type"]),
                    UserId = (string) r["user_id"]
                };
                arr.Add(lsd);
            }

            return arr.ToArray();

        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            var dt = new DataTable();
            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", connection);
            command.Parameters.AddWithValue("id", id);

            var adapter = new MySqlDataAdapter(command);
            adapter.Fill(dt);

            connection.Close();
            adapter.Dispose();

            if (dt.Rows.Count != 1)
            {
                return null;
            }
            else
            {
                var r = dt.Rows[0];
                var lsd = new LessonSignalDto
                {
                    Id = (int) r["id"],
                    Timestamp = (DateTime) r["time_stamp"],
                    Type = (LessonSignalType) Convert.ToInt32(r["signal_type"]),
                    UserId = (string) r["user_id"]
                };
                return lsd;
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
    
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
            command.Parameters.Add(new MySqlParameter("userId", userId));
            command.Parameters.Add(new MySqlParameter("signalType", signalType));
            
            await command.ExecuteNonQueryAsync();
            
            connection.Close();
            
            return Accepted();
        }
      
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM lesson_signal WHERE ID = @id;";
            command.Parameters.Add(new MySqlParameter("id", id));

            await command.ExecuteNonQueryAsync();
            //connection.Close();
            
            return Accepted();
        }
    }
}