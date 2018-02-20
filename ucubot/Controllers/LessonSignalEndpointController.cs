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
            var conn = new MySqlConnection(connectionString);
            const string query = "select * from lesson_signal";
            var dt = new DataTable();
            var cmd = new MySqlCommand(query, conn);
            conn.Open();
            var da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            var result = new List<LessonSignalDto>();
            if (dt.Rows.Count == 0)
            {
                return result;
            }
            
            foreach(DataRow row in dt.Rows)
            {
                var item = new LessonSignalDto
                {
                    Id = int.Parse(row["id"].ToString()),
                    UserId = row["UserId"].ToString(),
                    Type = (LessonSignalType) row["SignalType"],
                    Timestamp = Convert.ToDateTime(row["Timestamp"].ToString())
                };
                result.Add(item);
            }

            return result;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var query = $"select * from lesson_signal where id = {id}";
            var dt = new DataTable();
            var cmd = new MySqlCommand(query, conn);
            conn.Open();
            var da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            var result = new LessonSignalDto
            {
                Id = int.Parse(dt.Rows[0]["id"].ToString()),
                UserId = dt.Rows[0]["UserId"].ToString(),
                Type = (LessonSignalType) dt.Rows[0]["SignalType"],
                Timestamp = Convert.ToDateTime(dt.Rows[0]["Timestamp"].ToString())
            };

            return result;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var userId = message.user_id.Replace("'", "''");
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var cmd = new MySqlCommand($"INSERT INTO lesson_signal (UserId, SignalType) VALUES ('{userId}', '{(int) signalType}');", conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand($"DELETE FROM lesson_signal WHERE id = {id}", conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return Accepted();
        }
    }
}
