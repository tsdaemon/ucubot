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
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            string query = "SELECT * FROM lesson-signal";
			var dataTable = new DataTable();
			var connection = new MySqlConnection(connectionString);
			var command = new MySqlCommand(query, connection);
			var lessonSignalsArray = new List<LessonSignalDto>();
			
			connection.Open();
			
			var adapter = new MySqlDataAdapter(command);
			adapter.Fill(dataTable);
			
			connection.Close();
			adapter.Dispose();
			
			foreach(DataRow row in dataTable.Rows)
			{
				lessonSignalsArray.Add(
				{
					Id = (int) row["Id"],
					DataTime = (DateTime) row["TIMESTAMP"],
					Type = (LessonSignalType) row["SignalType"],
					UserId = (string) row["UserId"]
				});
				
			}
			
			
            return lessonSignalsArray;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            return Accepted();
        }
    }
}
