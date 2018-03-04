using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            conn.Open();
            
            var command = new MySqlCommand("SELECT * FROM lesson_signal", conn);
            var adapter = new MySqlDataAdapter(command);
            var dataset = new DataSet();
            adapter.Fill(dataset, "lesson_signal");
            
          
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                var signalDto = new LessonSignalDto()
                {
                    Id = (int) row["Id"],
                    UserId = (string) row["UserId"],
                    Type = (LessonSignalType) Convert.ToInt32(row["SignalType"]),
                    Timestamp = (DateTime) row["Timestamp"]
                };
            yield return signalDto;
            }
            conn.Close();
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE Id = @id;", conn);
                var adapter = new MySqlDataAdapter(command);
                var datatable = new DataTable();
                adapter.Fill(datatable);
                
                if (datatable.Rows.Count < 1)
                {
                    Response.StatusCode = 404;
                    return null;
                }

                var row = datatable.Rows[0];
                var signalDto = new LessonSignalDto()
                
                {
                    Id = (int) row["Id"],
                    UserId = (string) row["UserId"],
                    Type = (LessonSignalType) Convert.ToInt32(row["SignalType"]),
                    Timestamp = (DateTime) row["Timestamp"]
                };
                
                return signalDto;    

            }
                    
            
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            using (var conn = new MySqlConnection(connectionString))
            {
                var command = new MySqlCommand("INSERT INTO lesson_signal (SignalType, UserId) VALUES (@SignalType, @UserId);", conn);
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
                command.Parameters.Add("@SignalType", MySqlDbType.Int32).Value = signalType;
                command.ExecuteNonQuery();
                conn.Close();
            }
            
           return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");

      
            using (var conn = new MySqlConnection(connectionString))
            {

                var command = new MySqlCommand("DELETE FROM lesson_signal WHERE Id = @id;", conn);
                command.Parameters.Add(new MySqlParameter("Id", id));
                command.ExecuteNonQuery();
                conn.Close();
            }
            
            return Accepted();
        }
    }
}
