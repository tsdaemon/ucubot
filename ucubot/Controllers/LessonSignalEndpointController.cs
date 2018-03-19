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

            var dataTable = new DataTable();
            var queryCommand = new MySqlCommand("SELECT * FROM lesson_signal", connection);

            connection.Open();

            var adapter = new MySqlDataAdapter(queryCommand);
            adapter.Fill(dataTable);

            var lessonSignalDtos = new List<LessonSignalDto>();

            foreach (DataRow row in dataTable.Rows)
            {
                var lessonSignalDto = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    UserId = (string) row["user_id"],
                    Type = (LessonSignalType) row["signal_type"],
                    Timestamp = Convert.ToDateTime(row["time_stamp"])
                };

                lessonSignalDtos.Add(lessonSignalDto);
            }

            connection.Close();
            adapter.Dispose();
            return lessonSignalDtos;
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            //            $"select * from lesson_signal where id={id}"

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM lesson_signal WHERE id=@id";
            command.Parameters.Add(new MySqlParameter("id", id));
            var adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);
            connection.Close();
            adapter.Dispose();

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

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);


            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@user_id, @signal_type)";
            command.Parameters.Add(new MySqlParameter("user_id", userId));
            command.Parameters.Add(new MySqlParameter("signal_type", signalType));

            try
            {
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
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
