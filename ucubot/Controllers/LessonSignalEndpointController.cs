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
            // TODO: add query to get all signals
            using var con = MySqlConnection(connectionString){
                con.Open();
                string query = "SELECT * FROM lesson_signal";
                var cmd = new MySqlCommand(query, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt= new DataTable();
                da.Fill(dt, "lesson_signal");
                List<LessonSignalDto> values = new List<LessonSignalDto>();
                foreach(DataRow row in dt.Rows){
                    var lsidto = new LessonSignalDto
                    {
                        Id = (int) row["id"],
                        Timestamp = (DateTime) row["date_time"],
                        Type = (LessonSignalType)row["signal_type"],
                        UserId = (string) row["user_id"]
                        };
                        
                    values.add(lsidto)
                    }
                    return values;
        }
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
             using (var con = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"))){
                con.Open();
                var cmd = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=@id", con);
                cmd.Parameters.AddWithValue("id", id);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                
                DataTable dt = new DataTable();
                
                da.Fill(dt, "lesson_signal");
                
                if (dt.Rows.Count < 1)
                    return null;
                
                var row = dt.Rows[0];
                var lsidto = new LessonSignalDto
                {
                    Id = (int) row["id"],
                	Timestamp = (DateTime) row["date_time"],
                    Type = (LessonSignalType)row["signal_type"],
                    UserId = (string) row["user_id"]
                };
                return lsidto;
             }
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            using (var con = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText =
                    "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
                cmd.Parameters.AddRange(new[]
                {
                	new MySqlParameter("userId", userId),
                    new MySqlParameter("signalType", signalType)
                });
                await cmd.ExecuteNonQueryAsync();
            }
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            using (var con = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText =
                    "DELETE FROM lesson_signal WHERE ID = @id;";
            	cmd.Parameters.Add(new MySqlParameter("id", id));
                await cmd.ExecuteNonQueryAsync();
            }
            return Accepted();
        }
    }
}
