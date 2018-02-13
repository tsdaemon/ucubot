using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
                connection.Open();
                
                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM lesson_signal"
                };
               
                using (var dataAdapter = new MySqlDataAdapter(command))
                {           
                    var dataTable = new DataTable();
                
                    dataAdapter.Fill(dataTable);
                    
                    foreach (DataRow tableRow in dataTable.Rows)
                    {
                        var lessonSignalDto = new LessonSignalDto
                        {
                            Id = (int) (long) tableRow["id"],
                            
                            Timestamp = (DateTime) tableRow["timestamp"],
                          
                            Type = (LessonSignalType) tableRow["signal_type"],

                            UserId = (string) tableRow["user_id"]
                        };
                        
                        yield return lessonSignalDto;
                    }
                    
                    dataAdapter.Dispose();
                }
               
                connection.Close();
                
            }
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM lesson_signal WHERE id = " + id
                };
               
              
                var dataReader = command.ExecuteReader();

                if (dataReader.Read())
                {
                    var lessonSignalDto = new LessonSignalDto
                    {
                        Id = (int) (long) dataReader["id"],

                        Timestamp = (DateTime) dataReader["timestamp"],

                        Type = (LessonSignalType) dataReader["signal_type"],

                        UserId = (string) dataReader["user_id"]
                    };

                    return lessonSignalDto;
                }

                connection.Close();
                
            }
     
            return null;
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
                
                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "INSERT INTO lesson_signal(user_id, timestamp, signal_type) " +
                                  "VALUES('" + userId + "', NOW(),'" + (int) signalType + "')"  
                };

                await command.ExecuteNonQueryAsync();
                
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
                
                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "DELETE FROM lesson_signal WHERE id = " + id
                };

                await command.ExecuteNonQueryAsync();
                
                connection.Close();
            }
            
            return Accepted();
        }
    }
}
