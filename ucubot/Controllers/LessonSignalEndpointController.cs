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
            var lsdList = new List<LessonSignalDto>();
            
            using (var connection = new MySqlConnection(connectionString)){
                try{
                    connection.Open();
                }
                catch(Exception e){
                    Console.WriteLine(e.ToString());
                }

                var adapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", connection);
                
                var dataTable = new DataTable();
                
                adapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    var sgnl = new LessonSignalDto
                    {
                        Id = (int) row["id"],
                        Type = (LessonSignalType) row["signal_type"],
                        UserId = (string) row["user_id"]
                    };
                    lsdList.Add(sgnl);
                }               
            }

            return lsdList;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                try{
                    connection.Open();
                }
                catch (Exception e){
                    Console.WriteLine(e.ToString());
                }
                
                var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", connection);
                command.Parameters.AddWithValue("id", id);
                var adapter = new MySqlDataAdapter(command);
                
                var dataTable = new DataTable();
                
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count < 1)
                    return null;
                
                var row = dataTable.Rows[0];
                var sgnl = new LessonSignalDto
                {
                    Timestamp = (DateTime) row["timestamp_"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                };
                return sgnl;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                try{
                    connection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
                var command = connection.CreateCommand();
                command.CommandText =
                    "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
                command.Parameters.AddRange(new[]
                {
                    new MySqlParameter("userId", userId),
                    new MySqlParameter("signalType", signalType)
                });
                command.ExecuteNonQuery();
            }
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                try{
                    connection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
                var command = connection.CreateCommand();
                command.CommandText =
                    "DELETE FROM lesson_signal WHERE ID = @id;";
                command.Parameters.Add(new MySqlParameter("id", id));
                command.ExecuteNonQuery();
            }
            return Accepted();
        }
    }
}

