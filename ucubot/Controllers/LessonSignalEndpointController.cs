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
            connection.Open();

            var dataTable = new DataTable();
            var queryCommand = new MySqlCommand("select * from lesson_signal", connection);
            var adapter = new MySqlDataAdapter(queryCommand);
            adapter.Fill(dataTable);

            IEnumerable<LessonSignalDto> lessonSignalDtos = new List<LessonSignalDto>();

            foreach (DataRow row in dataTable.Rows)
            {
                var lessonSignalDto = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    UserId = (string) row["user_id"],
                    Type = (LessonSignalType) row["signal_type"],
                    Timestamp = Convert.ToDateTime(row["time_stamp"])
                };

                lessonSignalDtos.Append(lessonSignalDto);
            }

            return lessonSignalDtos;
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            var dataTable = new DataTable();
            var queryCommand = new MySqlCommand($"select * from lesson_signal where id={id}", connection);
            var adapter = new MySqlDataAdapter(queryCommand);
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            var row = dataTable.Rows[0];

            return new LessonSignalDto
            {
                Id = (int) row["id"],
                UserId = (string) row["user_id"],
                Type = (LessonSignalType) row["signal_type"],
                Timestamp = Convert.ToDateTime(row["time_stamp"])
            };
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "insert into lesson_signal value(null, @time_stamp, @signal_type, @userID);";
            command.Parameters.Add(new MySqlParameter("time_stamp", timeStamp));
            command.Parameters.Add(new MySqlParameter("signal_type", signalType));
            command.Parameters.Add(new MySqlParameter("user_id", userId));

            await command.ExecuteNonQueryAsync();
            connection.Close();

            return Accepted(); // "insert into lesson_signal value(null, {timeStamp}, {signalType}, {userId});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM lesson_signal WHERE ID = @id;";

            command.Parameters.Add(new MySqlParameter("id", id));

            await command.ExecuteNonQueryAsync();
            connection.Close();
            return Accepted();
        }
    }
}
