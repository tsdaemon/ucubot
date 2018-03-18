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
            // TODO: add query to get all signals
            var connection = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            try
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM lesson_signal", connection);
                var dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
                var results = new List<LessonSignalDto>();
                foreach(DataRow row in dataTable.Rows)
                {
                    var result = new LessonSignalDto
                    {
                        Id = (int) row["Id"],
                        Timestamp = (DateTime) row["TimeStamp"],
                        Type = (LessonSignalType) (row["SignalType"]),
                        UserId = (string) row["UserId"]
                    };
                    results.Add(result);
                }
                connection.Close();
                return results;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            return null;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get a signal by the given id
            var connection = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            try
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE ID=" + id, connection);
                var dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
                var row = dataTable.Rows[0];
                var result = new LessonSignalDto
                {
                    Id = (int) row["Id"],
                    Timestamp = (DateTime) row["TimeStamp"],
                    Type = (LessonSignalType) (row["SignalType"]),
                    UserId = (string) row["UserId"]
                };
                
                connection.Close();
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get a signal by the given id
            var connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO lesson_signal (TimeStamp, SignalType, UserId) VALUES " +
                                               "(@timestamp, @signalType, @userId)", connection);
                command.Parameters.Add(new MySqlParameter(){ParameterName = "@userId", Value = userId});
                command.Parameters.Add(new MySqlParameter(){ParameterName = "@signalType", Value = signalType});
                command.Parameters.Add(new MySqlParameter(){ParameterName = "@timestamp", Value = DateTime.Now});
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get a signal by the given id
            var connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM lesson_signal WHERE ID=" + id, connection);
                command.ExecuteNonQuery();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return BadRequest();
            }
            connection.Close();
            return Accepted();
        }
    }
}
