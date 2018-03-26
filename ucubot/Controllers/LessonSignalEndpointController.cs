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
			var signals = new List<LessonSignalDto>();
			using (connection){
				connection.Open();
				var table = new DataTable();
				var command = new MySqlCommand("SELECT * FROM lesson_signal;", connection);
				var adapter = new MySqlDataAdapter(command);
				adapter.Fill(table);
				foreach(DataRow row in table.Rows)
				{
					var recData = new LessonSignalDto
					{
						Id = (int) row["id"],
						Timestamp = (DateTime) row["time_stamp"],
						Type = (LessonSignalType) row["signal_type"],
						UserId = (string) row["user_id"]
					};
					signals.Add(recData);
				}
				connection.Close();
			}
			return signals;
		}
		
		[HttpGet("{id}")]
		public LessonSignalDto ShowSignal(long id)
		{
			var connectionString = _configuration.GetConnectionString("BotDatabase");
			var connection = new MySqlConnection(connectionString);
			using (connection)
			{
				connection.Open();
				var table = new DataTable();
				var command = new MySqlCommand(String.Format("SELECT * FROM lesson_signal WHERE id = {0};", id), connection);
				var adapter = new MySqlDataAdapter(command);
				adapter.Fill(table);
				if (table.Rows.Count == 0){
					return null;
				}
				var recData = new LessonSignalDto{
					Id = (int) table.Rows[0]["id"],
					Timestamp = (DateTime) table.Rows[0]["time_stamp"],
					Type = (LessonSignalType) table.Rows[0]["signal_type"],	
					UserId = (string) table.Rows[0]["user_id"]
				};
				connection.Close();
				return recData;

			}
			// TODO: add query to get a signal by the given id
		}
		
		[HttpPost]
		public async Task<IActionResult> CreateSignal(SlackMessage message)
		{
			var userId = message.user_id;
			var signalType = message.text.ConvertSlackMessageToSignalType();

			var connectionString = _configuration.GetConnectionString("BotDatabase");
			var connection = new MySqlConnection(connectionString);
			using (connection)
			{
				connection.Open();
				var command =
					new MySqlCommand(
						String.Format("INSERT INTO lesson_signal (time_stamp, signal_type, user_id) VALUES ({0}, {1}, {2});", 
							DateTime.Now, signalType, userId), connection);
				await command.ExecuteNonQueryAsync();
				connection.Close();
			}
			// TODO: add insert command to store signal
			return Accepted();
		}
		
		[HttpDelete("{id}")]
		public async Task<IActionResult> RemoveSignal(long id)
		{
			var connectionString = _configuration.GetConnectionString("BotDatabase");
			var connection = new MySqlConnection(connectionString);
			using (connection)
			{
				connection.Open();
				var command =
					new MySqlCommand(String.Format("DELETE FROM lesson_signal WHERE id = {0};", id), connection);
				await command.ExecuteNonQueryAsync();
				connection.Close();
			}

			//TODO: add delete command to remove signal
			return Accepted();
		}
	}
}