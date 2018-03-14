using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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

        // your data table
        private DataTable dataTable = new DataTable();

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // <wl1C8LTJrgT
            // TODO: add query to get all signals
            
            string query = "select * from lesson_signal;";

            var conn = new MySqlConnection(connectionString);        
            var cmd = new MySqlCommand(query, conn);
            conn.Open();

            // create data adapter
            var da = new MySqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            List<LessonSignalDto> arr =new List<LessonSignalDto>();
            foreach(DataRow row in dataTable.Rows)
            {
                arr.Add(new LessonSignalDto
                {
                    Id = (int)row["id"],
                    UserId = (string) row["user_id"],
                    Type = (LessonSignalType) row["signal_type"],
                    Timestamp = Convert.ToDateTime( row["timestamp"])
                       
                });
            }
            
            conn.Close();
            da.Dispose();
            return arr;
        }    
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            var connectionString = _configuration.GetConnectionString(" BotDatabase");
             
            string query = "select * from lesson_signal where id = @id;";

            var conn = new MySqlConnection(connectionString);        
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Add("@id", id);
            conn.Open();
            
           
            var da = new MySqlDataAdapter(cmd);
            da.Fill(dataTable);
            LessonSignalDto sign;
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                sign = new LessonSignalDto
                {
                    Id = (int)row["id"],
                    UserId = (string) row["user_id"],
                    Type = (LessonSignalType) row["signal_type"],
                    Timestamp = Convert.ToDateTime( row["timestamp"])
                       
                };
            }
            else
            {
                sign = null;
            }

            conn.Close();
            da.Dispose();
            return sign;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
             
            string query = "insert into lesson_signal(signal_type, user_id, timestamp) " +
                           "values(@signalType, @userId, @timestamp)  ;";

            var conn = new MySqlConnection(connectionString);        
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Add("@timestamp",DateTime.Now);
            cmd.Parameters.Add("@signalType", signalType);
            cmd.Parameters.Add("@userId", userId);
            conn.Open();
            cmd.ExecuteNonQuery();
          
            conn.Close();

            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
             
            string query = " DELETE FROM lesson_signal where id = @id ;";

            var conn = new MySqlConnection(connectionString);        
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Add("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
            

            conn.Close();
            
            return Accepted();
        }
    }
}
