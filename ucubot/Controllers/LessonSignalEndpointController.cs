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
            using (var conn = new MySqlConnection(connectionString)){
                conn.Open();
                var adapt = new MySqlDataAdapter("SELECT * FROM lesson_signal",conn);
                var dataset = new DataSet();
                adapt.Fill(dataset, "lesson_signal");
                conn.Close();
                foreach (row in dataset.Tables[0].Rows){
                    var LessonSignalDto = new LessonSignalDto{
                        Id = (int) row["id"],
                        Timestamp = Convert.ToDateTime(row["timestamp1"]),
                        Type = (int) (row["SignalType"]),
                        UserId = (string) row["UserId"]
                    }
                }               
            }
            // TODO: add query to get all signals
            yield return LessonSignalDto;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString)){
                conn.Open();
                var adapt = new MySqlDataAdapter("SELECT * FROM lesson_signal WHERE id = @id",conn);
                command.Parameters.AddWithValue("id",id);
                var dataset = new DataSet();
                adapt.Fill(dataset, "lesson_signal");
                conn.Close();
                foreach (row in dataset.Tables[0].Rows){
                    var LessonSignalDto = new LessonSignalDto{
                        Timestamp = Convert.ToDateTime(row["timestamp1"]),
                        Type = (int) (row["SignalType"]),
                        UserId = (string) row["UserId"]
                    }
                }               
            }
            // TODO: add query to get a signal by the given id
            if(rows.Count >= 0){
                return LessonSignalDto;
            }
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString)){
            // TODO: add insert command to store signal
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = "INSERT INTO lesson_signal (UserId, SignalType) VALUES (@UserId, @SignalType);";
                comm.Parameters.AddWithValue("@SignalType", SignalType);
                comm.Parameters.AddWithValue("@UserId", UserId);
                conn.Close();
            }
            return Accepted();
            
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString)){
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = "DELETE FROM lesson_signal WHERE id = @id;";
                comm.Parameters.AddWithValue("@id",id);
                conn.Close();
            //TODO: add delete command to remove signal
            
            return Accepted();
            }
        }
    }
}
