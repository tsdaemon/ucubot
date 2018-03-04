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
    [Route("api/[controller]/:[id]")]
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
            //TODO: replace with database query
            const string query = "select * from lesson_signal";

            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                var cmd = new MySqlCommand(query, conn);
                conn.Open();
                // create data adapter
                var adapter = new MySqlDataAdapter(cmd);
                var dataSet = new DataSet();

                // this will query your database and return the result to your datatable
                adapter.Fill(dataSet, "lesson_signal");

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    var lessonSignalDto = new LessonSignalDto
                    {
                        Id = (int) dataRow["id"],
                        Timestamp = (DateTime) dataRow["timestamp_"],
                        Type = (LessonSignalType) Convert.ToInt32(dataRow["signal_type"]),
                        UserId = (string) dataRow["user_id"]

                    };
                    yield return lessonSignalDto;

                }
            }

            
            
        }
        
        [HttpGet]
        public LessonSignalDto ShowSignal(long id)
        {
            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                //TODO: replace with database query
                string query = "select * from  lesson_signal where id = @id";

              
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                // create data adapter
                var adapter = new MySqlDataAdapter(cmd);
                var dataSet = new DataSet();

                // this will query your database and return the result to your datatable
                adapter.Fill(dataSet, "lesson_signal");
                if (dataSet.Tables[0].Rows.Count < 1)
                    return null;
                var dataRow = dataSet.Tables[0].Rows[0];
                var lessonSignalDto = new LessonSignalDto
                {
                    Id = (int)id,
                    Timestamp = (DateTime) dataRow["timestamp_"],
                    Type = (LessonSignalType) Convert.ToInt32(dataRow["signal_type"]),
                    UserId = (string) dataRow["user_id"]

                };

                return lessonSignalDto;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();

            //TODO: add code to store above values

            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {

                var query = "insert into lesson_signal (userId, signalType) values (@userId, @signalType) ";

                
                var cmd = conn.CreateCommand();
                cmd.CommandText = query;
                conn.Open();
                cmd.Parameters.AddRange(new[]
                {
                    new MySqlParameter("userId", userId),
                    new MySqlParameter("signalType", signalType)
                });
                await cmd.ExecuteNonQueryAsync();
                
            }
            return Accepted();
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add code to delete a record with the given id


            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                var query = "delete from lesson_signal where userid = @id;";
                cmd.Parameters.Add(new MySqlParameter("id", id));
                await cmd.ExecuteNonQueryAsync();
                
                
            }

            return Accepted();
        }
    }
}
