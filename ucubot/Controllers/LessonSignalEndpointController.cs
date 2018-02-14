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
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            connection.Open();

            var sqlexpressionAdapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", connection);

            var data = new DataSet();
            sqlexpressionAdapter.Fill(data, "lesson_signal");

            foreach (DataRow row in data.Tables[0].Rows)
            {
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

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            connection.Open();

            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", connection);
            command.Parameters.Add(new MySqlParameter("id", id));

            var adapter = new MySqlDataAdapter(command);

            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");

            using (var res = dataset.Tables[0])
            {
                if (res.Rows.Count < 1)
                {
                    return null;
                }

                var signalDto = new LessonSignalDto
                {
                    Id = (int) res.Rows[0]["id"],
                    Timestamp = (DateTime) res.Rows[0]["timestamp"],
                    Type = (LessonSignalType) Convert.ToInt32(res.Rows[0]["signal_type"]),
                    UserId = (string) res.Rows[0]["user_id"]
                };
                return signalDto;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            Console.WriteLine("start");

            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            Console.WriteLine(message);
            Console.WriteLine(userId);
            Console.WriteLine(signalType);

            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            connection.Open();

            var command =
                new MySqlCommand("INSERT INTO lesson_signal (signal_type, user_id) VALUES (@userId, @signalType)",
                    connection);
            command.Parameters.Add(new MySqlParameter("userId", userId));
            command.Parameters.Add(new MySqlParameter("signalType", signalType));

            command.ExecuteNonQuery();

            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            connection.Open();

            var command = new MySqlCommand("DELETE FROM lesson_signal WHERE id = @id", connection);
            command.Parameters.Add(new MySqlParameter("id", id));

            command.ExecuteNonQuery();

            return Accepted();
        }
    }
}