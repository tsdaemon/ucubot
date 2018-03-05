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
            var query = "SELECT * FROM lesson_signal";
            var conn = new MySqlConnection(connectionString); 
            var dataTable = new DataTable();
            var cmd = new MySqlCommand(query, conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                conn.Open();
                da.Fill(dataTable);
                conn.Close();
            }
            catch
            {
                
            }
            //initialize list of objects
            
            var enumerable = new List<LessonSignalDto>();
            
            foreach(DataRow row in dataTable.Rows)
            {
                var obj = new LessonSignalDto
                {
                    Id = (int)row["id"],
                    UserId = (string)row["user_id"],
                    Type = (LessonSignalType)row["signal_type"],
                    Timestamp = (DateTime)row["time_stamp"]
                };
                // add object to list
                enumerable.Add(obj);
            }

            return enumerable;
            /**/


            // return list of objects
            //return new LessonSignalDto[0];
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // 
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // 
            var query = "SELECT * FROM lesson_signal WHERE id=@id";
            
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", id);
            var da = new MySqlDataAdapter(cmd);
            var dataTable = new DataTable();
            try
            {
                conn.Open();
                da.Fill(dataTable);
                conn.Close();
            }
            catch
            {
                
            }
            //initialize list of objects

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            var obj = new LessonSignalDto
            {
                Timestamp = (DateTime) dataTable.Rows[0]["time_stamp"],
                Type = (LessonSignalType) dataTable.Rows[0]["signal_type"],
                UserId = (string) dataTable.Rows[0]["user_id"]
            };

            

            /**/
            return obj;

        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("userId",userId);
            cmd.Parameters.AddWithValue("signalType", signalType);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                
            }

            return Accepted();
            
            
            //return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "DELETE FROM lesson_signal WHERE id=@id";
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", id);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                
            }

            
            return Accepted();
        }
    }
}
