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
            using (MySqlConnection connection = new MySqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("BotDatabase");
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=@id", connection);
                command.Parameters.AddWithValue("id", id);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    using (DataTable table = new DataTable())
                    {
                        adapter.Fill(table);
                        if (table.Rows.Count == 0)
                        {
                            return null;
                        }

                        DataRow row = table.Rows[0];
                        return new LessonSignalDto()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Timestamp = (DateTime) row["Timestamp"],
                            Type = (LessonSignalType) Convert.ToInt32(row["SignalType"]),
                            UserId = row["UserId"].ToString()
                        };
                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            try
            {
                var signalType = message.text.ConvertSlackMessageToSignalType();
                using (MySqlConnection connection = new MySqlConnection())
                {
                    connection.ConnectionString = _configuration.GetConnectionString("BotDatabase");
                    connection.Open();
                    MySqlCommand command =
                        new MySqlCommand("INSERT INTO lesson_signal(SignalType, UserId) VALUES(@st, @uid)", connection);
                    command.Parameters.AddWithValue("st", (int) signalType);
                    command.Parameters.AddWithValue("uid", userId);
                    command.ExecuteNonQuery();
                    return Accepted();
                }
            }
            catch (CanNotParseSlackCommandException e)
            {
                Console.Out.WriteLine(e.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            using (MySqlConnection connection = new MySqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("BotDatabase");
                connection.Open();
                MySqlCommand command = new MySqlCommand(String.Format("SELECT * FROM lesson_signal WHERE id=@id", id),
                    connection);
                command.Parameters.AddWithValue("id", id);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    using (DataTable table = new DataTable())
                    {
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            return BadRequest();
                        }

                        MySqlCommand deleteCommand =
                            new MySqlCommand("DELETE FROM lesson_signal WHERE id=@id", connection);
                        deleteCommand.Parameters.AddWithValue("id", id);
                        deleteCommand.ExecuteNonQuery();
                        return Accepted();
                    }
                }
            }
        }
    }
}