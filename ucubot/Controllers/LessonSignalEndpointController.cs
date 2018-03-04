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

            var  connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                const string query = "SELECT * FROM LessonSignal;";
                var cmd = new MySqlCommand(query);
                var dataSet = new DataSet();

                var adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dataSet, "LessonSignal");

                foreach (DataRow row in dataSet.Tables[0].Rows)
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
            var  connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var query = "SELECT * FROM LessonSignal WHERE id = @id;";
            var cmd = new MySqlCommand(query);
            var parameter = new MySqlParameter("ID", MySqlDbType.Int32);
            cmd.Parameters.Add(parameter);
            
            conn.Open();

            using (var adapter = new MySqlDataAdapter(cmd))
            {
                var _dataSet = new DataSet();
                adapter.Fill(_dataSet, "LessonSignal");
                if (_dataSet.Tables[0].Rows.Count < 1)
                {
                    return null;
                }

                var row = _dataSet.Tables[0].Rows[0];
                var _lessonSignalDto = new LessonSignalDto
                {
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
                    new MySqlParameter("signalType", signalType)
                });
                cmd.CommandText = "INSERT INTO LessonSignal (UserID, SignalType) VALUES (@userId, @signalType);";
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
                cmd.CommandText = "DELETE FROM LessonSignal WHERE ID = @id;";
                cmd.Parameters.Add(new MySqlParameter("ID", id));
                await cmd.ExecuteNonQueryAsync();
            }   
            return Accepted();
        }
    }
}
