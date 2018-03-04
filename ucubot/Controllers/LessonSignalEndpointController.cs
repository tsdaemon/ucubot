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
            var dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "SELECT * FROM lesson_signal";
            var conn = new MySqlConnection(connectionString);
            
            var cmd = new MySqlCommand(query, conn);
            var adapter = new MySqlDataAdapter(cmd);

            try
            {
                conn.Open();
                adapter.Fill(dataTable);
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            var list = new List<LessonSignalDto>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["time_stamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                });
            }


            return list;
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "SELECT * FROM lesson_signal WHERE id = @id";
            var conn = new MySqlConnection(connectionString);
            
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            var adapter = new MySqlDataAdapter(cmd);

            try
            {
                conn.Open();
                adapter.Fill(dataTable);
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            var row = dataTable.Rows[0];

            var signalDto = new LessonSignalDto
            {
                Timestamp = (DateTime) row["time_stamp"],
                Type = (LessonSignalType) row["signal_type"],
                UserId = (string) row["user_id"]
            };

            return signalDto;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id.Replace("'", "\"");
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var query = "INSERT INTO lesson_signal (user_id, signal_type) VALUES(@user_id, @signal_type)";

            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@signal_type", signalType);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "DELETE FROM lesson_signal WHERE id = @id";
            var conn = new MySqlConnection(connectionString);
            
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Accepted();
        }
    }
}