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
			var connection = new MySqlConnection(connectionString);
            using (connection){
            	connection.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM lesson_signal;", connection);
                
                var dataset = new DataSet();
                
                adapter.Fill(dataset, "lesson_signal");

                for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                {
                    var signalDto = new LessonSignalDto
                    {
                        id = (int) dataset.Tables[0].Rows[i]["id"],
                        time_stamp = (DateTime) dataset.Tables[0].Rows[i]["time_stamp"],
                        signal_type = (LessonSignalType)Convert.ToInt32(dataset.Tables[0].Rows[i]["signal_type"]),
                        user_id = (string) dataset.Tables[0].Rows[i]["user_id"]
                    };
                    yield return signalDto;
                }
			}
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
			var connectionString = _configuration.GetConnectionString("BotDatabase");
			var connection = new MySqlConnection(connectionString);
			using (connection){
				connection.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM lesson_signal WHERE id = @id;", connection);
                
                var dataset = new DataSet();
                
                adapter.Fill(dataset, "lesson_signal");

				if(dataset.Tables[0].Rows.Count != 0){
					var signalDto = new LessonSignalDto{
						id = (int) dataset.Tables[0].Rows[0]["id"],
						time_stamp = (DateTime) dataset.Tables[0].Rows[0]["time_stamp"],
						signal_type = (LessonSignalType)Convert.ToInt32(dataset.Tables[0].Rows[0]["signal_type"]),
						user_id = (string) dataset.Tables[0].Rows[0]["user_id"]
					};
					return signalDto;
				}
			}
		return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
			var connectionString = _configuration.GetConnectionString("BotDatabase");
			var connection = new MySqlConnection(connectionString);
			using (connection){
				var command = connection.CreateCommand();
				command.CommandText = "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
				command.Parameters.Add(new MySqlParameter("user_id", userId));
				command.Parameters.Add(new MySqlParameter("signal_type", signalType));
				connection.Open();
				await command.ExecuteNonQueryAsync();
				connection.Close();
			}
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
			var connectionString = _configuration.GetConnectionString("BotDatabase");
			var connection = new MySqlConnection(connectionString);
			using (connection){
				var command = connection.CreateCommand();
				command.CommandText = "DELETE FROM lesson_signal WHERE id = @id";
				command.Parameters.Add(new MySqlParameter("id", id));
				connection.Open();
				await command.ExecuteNonQueryAsync();
				connection.Close();
			}
            return Accepted();
        }
    }
}
