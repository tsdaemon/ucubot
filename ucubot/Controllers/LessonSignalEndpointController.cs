using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
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

            using (MySqlConnection connection = new MySqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT * FROM lesson_signal", connection);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    using (DataTable table = new DataTable())
                    {
                        adapter.Fill(table);

                        List<LessonSignalDto> signals = new List<LessonSignalDto>();
                        foreach (DataRow row in table.Rows)
                        {
                            signals.Add(
                                new LessonSignalDto()
                                {
                                    Id = Convert.ToInt32(row["Id"]),
                                    Timestamp = (DateTime) row["Timestamp"],
                                    Type = (LessonSignalType) Convert.ToInt32(row["SignalType"]),
                                    UserId = row["UserId"].ToString()
                                }
                            );
                        }

                        return signals;
                    }
                }
            }
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            using (MySqlConnection connection = new MySqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("BotDatabase");
                connection.Open();
                MySqlCommand command = new MySqlCommand(String.Format("SELECT * FROM lesson_signal WHERE id={0}", id),
                    connection);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    using (DataTable table = new DataTable())
                    {
                        adapter.Fill(table);
                        try
                        {
                            DataRow row = table.Rows[0];
                            return new LessonSignalDto()
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                Timestamp = (DateTime) row["Timestamp"],
                                Type = (LessonSignalType) Convert.ToInt32(row["SignalType"]),
                                UserId = row["UserId"].ToString()
                            };
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Console.Out.WriteLine(e.Message);
                            return null;
                        }


                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            using (MySqlConnection connection = new MySqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("BotDatabase");
                connection.Open();
                MySqlCommand command = new MySqlCommand(String.Format("INSERT INTO lesson_signal(SignalType, UserId), VALUES({0}, {1})", signalType, userId), connection);
                command.ExecuteScalar();
            }

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