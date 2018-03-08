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
            using (var connection = new MySqlConnection(connectionString))
            {
                string myQuery = "SELECT * FROM lesson_signal";
                var command = new MySqlCommand(myQuery,connection);
                var dataTable = new DataTable();
                connection.Open();
                var data = new MySqlDataAdapter(command);
                data.Fill(dataTable);
            
                connection.Close();
                data.Dispose();
                            
                var lessonSignal = new List<LessonSignalDto>();
                
                if (dataTable.Rows.Count < 1)
                {
                    return lessonSignal;
                } 
            
                foreach (DataRow row in dataTable.Rows)
                {
                    var signalDto = new LessonSignalDto
                    {
                        Id = int.Parse(row["id"].ToString()),
                        Timestamp = Convert.ToDateTime(row["timestamp"].ToString()),
                        Type = (LessonSignalType) row["signal_type"],
                        UserId = row["user_id"].ToString() 
                    };
                    lessonSignal.Add(signalDto);    
                }
                return lessonSignal;
            }  
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var connection = new MySqlConnection(connectionString))
            {
                var myQuery = "Select * FROM lesson_signal where id = @id"; 
                var command = new MySqlCommand(myQuery, connection);
                command.Parameters.AddWithValue("id", id);
                
                var dataTable = new DataTable();
            
                connection.Open();
            
                var data = new MySqlDataAdapter(command);
                data.Fill(dataTable);
            
                connection.Close();
                data.Dispose();
                
                if (dataTable.Rows.Count < 1)
                {
                    return null;
                }
                
                var signalDto = new LessonSignalDto
                {
                    Id = int.Parse(dataTable.Rows[0]["id"].ToString()),
                    Timestamp = Convert.ToDateTime(dataTable.Rows[0]["timestamp"].ToString()),
                    Type = (LessonSignalType) dataTable.Rows[0]["signal_type"],
                    UserId = dataTable.Rows[0]["user_id"].ToString(),
                };
                return signalDto;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                string myQuery = "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@UserId, @SignalType);";
                var command = new MySqlCommand(myQuery, connection);

                command.Parameters.Add("@UserId", MySqlDbType.Text).Value = userId;
                command.Parameters.Add("@SignalType", MySqlDbType.Text).Value = signalType;
                command.CommandType = CommandType.Text;
                
                command.ExecuteNonQuery();
                
                connection.Close();
            }
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var myQuery = "DELETE FROM lesson_signal WHERE id = @id";
                var command = new MySqlCommand(myQuery, connection);
                command.Parameters.AddWithValue("id", id);

                command.ExecuteNonQuery();
                
                connection.Close();
            }
            return Accepted();
        }
    }
}
