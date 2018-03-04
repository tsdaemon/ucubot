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
            
            conn.Open();
            var cmd = new MySqlCommand(query, conn);
            

            // create data adapter
            var da = new MySqlDataAdapter(cmd);
            var dataSet = new DataSet();
            // this will query your database and return the result to your datatable
            da.Fill(dataSet, "lesson_signal");
            conn.Close();
            
            //initialize list of objects
            
            foreach(DataRow row in dataSet.Tables[0].Rows)
            {
                var obj = new LessonSignalDto
                {
                    Id = (int)row["Id"],
                    UserId = (string)row["UserId"],
                    Type = (LessonSignalType)row["SignalType"],
                    Timestamp = (DateTime)row["Timestamp"]
                };
                // add object to list
                yield return obj;
            }
            
            /**/
            da.Dispose();
            
            // return list of objects
            //return new LessonSignalDto[0];
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get all signals
            var query = "SELECT * FROM lesson_signal WHERE id=" + id.ToString();
            
            var conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
//            cmd.Parameters.AddWithValue("id", id);


                // create data adapter
                var da = new MySqlDataAdapter(cmd);
                var dataSet = new DataSet();
                // this will query your database and return the result to your datatable
                da.Fill(dataSet, "lesson_signal");
                conn.Close();

                //initialize list of objects

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var obj = new LessonSignalDto
                    {
                        Id = (int) row["Id"],
                        UserId = (string) row["UserId"],
                        Type = (LessonSignalType) row["SignalType"],
                        Timestamp = (DateTime) row["Timestamp"]
                    };
                    // add object to list
                    return obj;
                }

                /**/
                da.Dispose();
            }
            catch
            {
                
            }

            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "INSERT INTO lesson_signal (UserID, SignalType) VALUES ("+userId+", "+signalType+");";
            var conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
//            cmd.Parameters.AddWithValue("UserId",userId);
//            cmd.Parameters.AddWithValue("SignalType", signalType);

                await cmd.ExecuteNonQueryAsync();
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
            var query = "DELETE FROM lesson_signal WHERE id="+id+";";
            try
            {
                var conn = new MySqlConnection(connectionString);
                var cmd = new MySqlCommand(query, conn);
//            cmd.Parameters.AddWithValue("id", id);
                await cmd.ExecuteNonQueryAsync();
                conn.Close();
            }
            catch
            {
                
            }

            
            return Accepted();
        }
    }
}
