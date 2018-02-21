using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            var dataTable = new DataTable();

            try
            {
                conn.Open();
                var sql = "SELECT * FROM lesson signal";
                var cmd = new MySqlCommand(sql, conn);
                var dataAdapter = new MySqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            var lessonSignalDtos = new List<LessonSignalDto>();
            
            foreach(DataRow row in dataTable.Rows)
            {
                var lessonSignalDto = new LessonSignalDto
                {
                    UserId = (string) row["user_id"],
                    Type = (LessonSignalType) (row["signal_type"]),
                    Timestamp = (DateTime) row["timestamp"],
                    Id = (int) row["id"]
                };
                lessonSignalDtos.Add(lessonSignalDto);
            }
            conn.Close();
            return lessonSignalDtos;
        }
        
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {   
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            var dataTable = new DataTable();

            try
            {
                conn.Open();
                var sql = "SELECT * FROM lesson_signal WHERE id =" + id;
                var cmd = new MySqlCommand(sql, conn);
                var dataAdapter = new MySqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            if (dataTable.Rows.Count < 1)
            {
                Console.WriteLine("No records found :(");
                return null;
            }
            
            var row = dataTable.Rows[0];
            var lessonSignalDto = new LessonSignalDto
            {
                UserId = (string) row["user_id"],
                Type = (LessonSignalType) row["signal_type"],
                Timestamp = (DateTime) row["timestamp"],
                Id = (int) row["id"]
            };
        
            conn.Close();
            return lessonSignalDto;
        }
        
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            
            try
            {
                conn.Open();
                var sql = "INSERT INTO lesson_signal (user_id, signal_type, timestamp) VALUES " +
                          "(@userId, @signalType, @timestamp)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@signalType", signalType);
                cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            conn.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            
            try
            {
                conn.Open();
                var sql = "DELETE FROM lesson_signal WHERE id=" + id;
                var cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            conn.Close();          
            
            return Accepted();
        }
    }
}
