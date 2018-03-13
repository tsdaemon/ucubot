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
            var LessonSignalArray = new List<LessonSignalDto>();
            var dataTable = new DataTable();
            var  connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                const string query = "SELECT * FROM lesson_signal;";
                var cmd = new MySqlCommand(query, conn);

                var adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    LessonSignalArray.Add(new LessonSignalDto
                    {
                        Id = (int) row["ID"],
                        Timestamp = (DateTime) row["Timestamp_"],
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = row["UserID"].ToString()
                    });
                }
                conn.Close();
            }
            
            return LessonSignalArray;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var _dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var query = "SELECT * FROM lesson_signal WHERE id = @id;";
            
            conn.Open();
            
            var cmd = new MySqlCommand(query, conn);
            var parameter = new MySqlParameter("ID", MySqlDbType.Int32);
            cmd.Parameters.Add(parameter);

            using (var adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count < 1)
                {
                    return null;
                }

                var row = _dataTable.Rows[0];
                var _lessonSignalDto = new LessonSignalDto
                {
                    Id = (int) row["ID"],
                    Timestamp = (DateTime) row["Timestamp_"],
                    Type = (LessonSignalType) row["SignalType"],
                    UserId = row["UserID"].ToString()
                };

                conn.Close();
                return _lessonSignalDto;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var  connectionString = _configuration.GetConnectionString("BotDatabase");
            
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.Parameters.AddRange(new []
                {
                    new MySqlParameter("userId", userId),
                    new MySqlParameter("signalType", signalType),
                    new MySqlParameter("timestamp", DateTime.Now)
                });
                cmd.CommandText = "INSERT INTO lesson_signal (UserID, SignalType, TimeStamp_) VALUES (@userId, @signalType, @timestamp);";
                await cmd.ExecuteNonQueryAsync();
                conn.Close();
            }
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var  connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString))   
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM lesson_signal WHERE ID = @id;";
                cmd.Parameters.Add(new MySqlParameter("ID", id));
                await cmd.ExecuteNonQueryAsync();
            }
            
            return Accepted();
        }
    }
}
