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
            IEnumerable<LessonSignalDto> lessonSignalDtos = new List<LessonSignalDto>();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var cnn = new MySqlConnection(connectionString);
            cnn.Open();
            Console.WriteLine("Connection Open ! ");
            const string dataTable = "SELECT * FROM lesson_signal";
            var cmd = new MySqlCommand(dataTable);
            var adapter = new MySqlDataAdapter(cmd);
            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");
            var rows = dataset.Tables[0].Rows;
            foreach (DataRow row in rows)
            {
                var signal = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["timestamp_"],
                    Type = (LessonSignalType) Convert.ToInt32(row["signal_type"]),
                    UserId = (string) row["user_id"]
                };
                lessonSignalDtos.Append(signal);
            }

            cnn.Close();
            return lessonSignalDtos;
        }

        [HttpGet]
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
            var table = dataset.Tables[0];
            if (table.Rows.Count != 1)
                return null;

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
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var cnn = new MySqlConnection(connectionString);
            const string query =
                "INSERT INTO lesson_signal (@user_id, @signal_type) VALUES (@user_id, @signal_type)";

            cnn.Open();
            var command = cnn.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddRange(new[]
            {
                new MySqlParameter("userId", userId),
                new MySqlParameter("signalType", signalType)
            });
            await command.ExecuteNonQueryAsync();
            cnn.Close();
            return Accepted();
        }

        [HttpDelete]
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
            cnn.Close();
            return Accepted();
        }
    }
}