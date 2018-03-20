using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public IEnumerable<LessonSignalDto> ShowSignals() //Done
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            var data = new DataTable();
            var command = new MySqlCommand("SELECT * FROM lesson_signal", connection);
            
            connection.Open();

            var dataAdapter = new MySqlDataAdapter(command);    
            dataAdapter.Fill(data);

            var signals = new List<LessonSignalDto>();
            foreach (DataRow row in data.Rows)
            {
                var signalDto = new LessonSignalDto
                {
                    Id = (int) row["Id"],
                    Timestamp = (DateTime) row["time_stamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                };
                signals.Add(signalDto);
            }
            connection.Close();
            return signals;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id) //done
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            var data = new DataTable();
            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE Id ="+id,connection);
            
            connection.Open();
            
            var dataAdapter = new MySqlDataAdapter(command);
            dataAdapter.Fill(data);
            if (data.Rows.Count < 1)
            {
                return null;
            }

            var signal = new LessonSignalDto
            {
                Id = (int) data.Rows[0]["Id"],
                Timestamp = (DateTime) data.Rows[0]["time_stamp"],
                Type = (LessonSignalType) data.Rows[0]["signal_type"],
                UserId = (string) data.Rows[0]["user_id"]
            };

            connection.Close();
            return signal;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message) //done
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            
            connection.Open();
            var command = new MySqlCommand("INSERT INTO lesson_signal (signal_type, user_id) VALUES (@signal_type, @user_id)", connection);
            command.Parameters.Add(new MySqlParameter("@user_id", userId));
            command.Parameters.Add(new MySqlParameter("@signal_type", signalType));
            command.ExecuteNonQuery();
            connection.Close();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            
            connection.Open();
            var command = new MySqlCommand("DELETE FROM lesson_signal WHERE Id ="+id, connection);
            command.ExecuteNonQuery();
            connection.Close();
            return Accepted();
        }
    }
}
