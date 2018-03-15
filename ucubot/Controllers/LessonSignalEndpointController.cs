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
            var query = "SELECT * FROM LessonSignal";
            var connection = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            var adapter = new MySqlDataAdapter(query, connection);
            
			try
			{
				connection.Open();
				adapter.Fill(dataTable);
				connection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
            
            var lst = new List<LessonSignalDto>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                lst.Add(new LessonSignalDto
                {
                    Id = (int) row["Id"],
                    Timestamp = (DateTime) row["Timestamp"],
                    Type = (LessonSignalType) row["SignalType"],
                    UserId = (string) row["UserId"]
                });
            }
    
            return lst;

        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
 
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "SELECT * FROM LessonSignal WHERE Id = @id";
            var connection = new MySqlConnection(connectionString);
            
            var dataTable = new DataTable();
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            var adapter = new MySqlDataAdapter(command);
            try
			{
				connection.Open();
				adapter.Fill(dataTable);
				connection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
                       
            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            var row = dataTable.Rows[0];
                        
            var signalDto = new LessonSignalDto
                {
                    Timestamp = (DateTime) row["TimeStamp"],
                    Type = (LessonSignalType) row["SignalType"],
                    UserId = (string) row["UserId"]
                };
            
                
            return signalDto;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
	        var userId = message.user_id.Replace("'", "\"");
            var signalType = message.text.ConvertSlackMessageToSignalType();

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "INSERT INTO LessonSignal (UserId, SignalType) VALUES (@user_id, @signal_type)";
            var connection = new MySqlConnection(connectionString);
            
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@user_id", userId);
            command.Parameters.AddWithValue("@signal_type", signalType);

            try
			{
				connection.Open();
				command.ExecuteNonQuery();
				connection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "DELETE FROM LessonSignal WHERE Id = @id";
            var connection = new MySqlConnection(connectionString);
            
            var command = new MySqlCommand(query, connection);
	        command.Parameters.AddWithValue("@id", id);
            try
			{
				connection.Open();
				command.ExecuteNonQuery();
				connection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
            
            return Accepted();
        }
    }
}
