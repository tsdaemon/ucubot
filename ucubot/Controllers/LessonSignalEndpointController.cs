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
            using (var connect = new MySqlConnection(connectionString))
            {
                try
                {
                    connect.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                var table = new DataTable();
                var fin = new List<LessonSignalDto>();
                var cmd = new MySqlCommand("SELECT * FROM lesson_signal", connect);
                var adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(table);
                adapter.Dispose();

                if (table.Rows.Count == 0)
                {
                    return fin;
                }

                foreach (DataRow row in table.Rows)
                {
                    var dto_item = new LessonSignalDto
                    {
                        Id = int.Parse(row["id"].ToString()),
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = row["UserId"].ToString(),
                        Timestamp = Convert.ToDateTime(row["Timestamp"].ToString())
                    };
                    fin.Add(dto_item);
                }

                return fin;
            }
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var connect = new MySqlConnection(connectionString))
            {
                try
                {
                    connect.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                var table = new DataTable();
                var cmd = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=" + id, connect);
                var adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(table);
                adapter.Dispose();

                if (table.Rows.Count == 0)
                {
                    return null;
                }

                foreach (DataRow row in table.Rows)
                {
                    var dto_item = new LessonSignalDto
                    {
                        Id = int.Parse(row["id"].ToString()),
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = row["UserId"].ToString(),
                        Timestamp = Convert.ToDateTime(row["Timestamp"].ToString())
                    };
                    return dto_item;
                }

                return null;
            }
        }

        [HttpPost]

            public async Task<IActionResult> CreateSignal(SlackMessage message)
            {
                var userId = message.user_id;
                var signalType = message.text.ConvertSlackMessageToSignalType();
                var connectionString = _configuration.GetConnectionString("BotDatabase");

                using (var connect = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connect.Open();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return BadRequest();
                    }

                    string cmd_sql = "INSERT INTO lesson_signal (UserId, SignalType) VALUES (@first, @second)";
                    var cmd = new MySqlCommand(cmd_sql, connect);
                    cmd.Parameters.Add("@first", MySqlDbType.Text).Value = userId;
                    cmd.Parameters.Add("@second", MySqlDbType.Int32).Value = signalType;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                return Accepted();
            }

            [

            HttpDelete("{id}")]

            public async Task<IActionResult> RemoveSignal(long id)
            {
                var connectionString = _configuration.GetConnectionString("BotDatabase");
                using (var connect = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connect.Open();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return BadRequest();
                    }

                    var cmd = new MySqlCommand("DELETE FROM lesson_signal WHERE Id=" + id + ";", connect);
                    cmd.ExecuteNonQuery();

                    return Accepted();
                }
            }
        }
    }
