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
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            var table = new DataTable();
            var comm = new MySqlCommand("SELECT * FROM lesson_signal", connection);
            var adapter = new MySqlDataAdapter(comm);
            var signalDtos = new List<LessonSignalDto>();
            
            adapter.Fill(table);
            connection.Open();
            
            foreach (DataRow row in table.Rows)
            {
                var signalDto = new LessonSignalDto
                {
                    UserId = (string) row["user_id"],
                    Timestamp = (DateTime) row["timestamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    Id = (int) row["id"]
                };
                signalDtos.Add(signalDto);
            }
            connection.Close();
            return signalDtos;     
         
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var comm = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=" + id, connection);
                var adapter = new MySqlDataAdapter(comm);
                var table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count == 0) return null;

                var row = table.Rows[0];
                connection.Close();
                return new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["Timestamp"],
                    Type = (LessonSignalType) row["SignalType"],
                    UserId = row["UserId"] as string
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            
            var comm = new MySqlCommand("INSERT INTO lesson_signal (signal_type, user_id) " +
                                        "VALUES (@signalType, @userId)", 
                                        conn);
            comm.Parameters.Add(new MySqlParameter("userId", userId));
            comm.Parameters.Add(new MySqlParameter("signalType", signalType));
            comm.ExecuteNonQuery();
            
            conn.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn = new MySqlConnection(connectionString);
            
            conn.Open();

            var comm = new MySqlCommand("DELETE FROM lesson_signal WHERE id = @id", conn);
            comm.Parameters.Add(new MySqlParameter("id", id));
            comm.ExecuteNonQuery();
            
            conn.Close();

            return Accepted();
        }
    }
}
