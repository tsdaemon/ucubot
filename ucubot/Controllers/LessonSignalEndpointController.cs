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
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            var connection = new MySqlConnection(connectionString);
	    connection.Open();
		var sqlcommand = new MySqlCommand("SELECT * FROM lesson_signal", connection);
		var myadapter = new MySqlDataAdapter(sqlcommand);
		var dataset = new DataSet();
		myadapter.Fill(dataset, "lesson_signal");
		connection.Close();
		foreach (DataRow row in data.Tables[0].Rows){
                var signalDto = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["timestamp"],
                    Type = (LessonSignalType) Convert.ToInt32(row["signal_type"]),
                    UserId = (string) row["user_id"]
                };

                yield return signalDto;
		}
        }
        
        [HttpGet]
        public LessonSignalDto ShowSignal(long id)
        {   
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
		connection.Open();
	var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=" + id.ToString(), connection);
		connection.Close();
		command.Parameters.AddWithValue("id", id);
		var adapter = new MySqlDataAdapter(command);
		var dataset = new DataSet();
		adapter.Fill(dataset, "lesson_signal");
		if (dataset.Tables[0].Rows.Count < 1){
			return null;}
		var row = dataset.Tables[0].Rows[0];
		var signalDto = new LessonSignalDto
		{
			Id = (int)row["id"],
			Timestamp = (DateTime)row["timestamp],
			Type = (LessonSignalType)row["signal_type"],
			UserId = (string)row["user_id"]
			
		};
		
            return signalDto;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();

            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
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
        
        [HttpDelete]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
		connection.Open();
		
		var command = connection.CreateCommand();
            command.CommandText =
                "DELETE FROM lesson_signal" +
                "WHERE id=@id";
            command.Parameters.Add(new MySqlParameter("id", id));

		await command.ExecuteNonQueryAsync();
		connection.Close();
            return Accepted();
        }
    }
}
