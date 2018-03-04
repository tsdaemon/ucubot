using System;
using System.Collections.Generic;
using System.Data;
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
            var lessonSignalDtos = new List<LessonSignalDto>();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var cnn = new MySqlConnection(connectionString);
            cnn.Open();
            Console.WriteLine("Connection Open ! ");

            const string dataTable = "SELECT * FROM lesson_signal";
            var adapter = new MySqlDataAdapter(dataTable, cnn);
            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");
            var rows = dataset.Tables[0].Rows;
            foreach (DataRow row in rows)
            {
                var signal = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["timestamp_"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                };
                lessonSignalDtos.Add(signal);
            }

            cnn.Close();
            return lessonSignalDtos;
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var cnn = new MySqlConnection(connectionString);
            cnn.Open();
            Console.WriteLine("Connection Open ! ");
            var dataTable = "SELECT * FROM lesson_signal WHERE id=" + id;
            var cmd = new MySqlCommand(dataTable, cnn);
            var adapter = new MySqlDataAdapter(cmd);
            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");
            if (dataset.Tables[0].Rows.Count < 1) return null;
            var row = dataset.Tables[0].Rows[0];

            var lessonSignalDto = new LessonSignalDto
            {
                Timestamp = (DateTime) row["timestamp_"],
                Type = (LessonSignalType) row["signal_type"],
                UserId = (string) row["user_id"]
            };
            return lessonSignalDto;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var cnn = new MySqlConnection(connectionString);
            const string query =
                "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@user_id, @signal_type)";

            cnn.Open();
            var command = cnn.CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("user_id", userId));
            command.Parameters.Add(new MySqlParameter("signal_type", signalType));
            await command.ExecuteNonQueryAsync();
            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var cnn = new MySqlConnection(connectionString);
            const string query = "DELETE FROM lesson_signal WHERE ID = @id;";
            cnn.Open();
            var command = cnn.CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("id", id));
            await command.ExecuteNonQueryAsync();
            return Accepted();
        }
    }
}