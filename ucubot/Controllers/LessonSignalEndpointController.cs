using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Remotion.Linq.Clauses.ResultOperators;
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
            
            connection.Open();
            
            var command = new MySqlCommand("SELECT * FROM lesson_signal", connection);
            var adapter = new MySqlDataAdapter(command);
            
            connection.Close();

            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");
            var table = dataset.Tables[0];
            
            foreach(DataRow row in table.Rows){
                yield return new LessonSignalDto
                {
                    Id = (int)row["id"],
                    UserId = (string)row["user_id"],
                    Type = (LessonSignalType)row["signal_type"],
                    Timestamp = (DateTime)row["timestamp_"]
                };
            }
            
            
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            var connection = new MySqlConnection(connectionString);
            
            connection.Open();
            
            var command = new MySqlCommand(
                "SELECT * FROM lesson_signal WHERE id=" + id.ToString(),
                connection
            );
            
            connection.Close();
            
            var adapter = new MySqlDataAdapter(command);
            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");

            var table = dataset.Tables[0];
            if (table.Rows.Count != 1)
                return null;
            
            return new LessonSignalDto
            {
                Id = (int)table.Rows[0]["id"],
                Timestamp = (DateTime)table.Rows[0]["timestamp_"],
                Type = (LessonSignalType)table.Rows[0]["signal_type"],
                UserId = (string)table.Rows[0]["user_id"]
            };
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO lesson_signal (user_id, signal_type) " +
                "VALUES (@user_id, @signal_type)";
            command.Parameters.Add(new MySqlParameter("user_id", userId));
            command.Parameters.Add(new MySqlParameter("signal_type", signalType));

            await command.ExecuteNonQueryAsync();
            
            connection.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM lesson_signal " +
                "WHERE id=@id";
            command.Parameters.Add(new MySqlParameter("id", id));

            await command.ExecuteNonQueryAsync();
            
            connection.Close();
            
            return Accepted();
        }
    }
}
