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
        private DataTable dataTable = new DataTable();


        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var connection = new MySqlConnection(connectionString);
            string query = "select * from lesson_signal";
            connection.Open();
            MySqlCommand cmd = new  MySqlCommand(query, connection);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(dataTable);
            
            List<LessonSignalDto> lessonSignalDtos = new List<LessonSignalDto>();
            
            foreach(DataRow row in dataTable.Rows)
            {
                LessonSignalDto less = new LessonSignalDto();
                less.Id = (int) row["id"];
                less.UserId = (string) row["user_id"];
                less.Type = SignalTypeUtils.ConvertSlackMessageToSignalType((string)row["signal_type"]) ;
                less.Timestamp = Convert.ToDateTime(row["time_stamp"]);
                lessonSignalDtos.Add(less);
            }
            connection.Close();
            dataAdapter.Dispose();
            return lessonSignalDtos;

        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)

        {   
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            var dataTable = new DataTable();
            string query = "select * from lesson_signal where id ="+id;
            MySqlCommand cmd = new  MySqlCommand(query, connection);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(dataTable);
            
            var row = dataTable.Rows[0];
            var lessonSignalDto = new LessonSignalDto
            {
                UserId = (string) row["user_id"],
                Timestamp = Convert.ToDateTime(row["timestamp"]),
                Type = (LessonSignalType) row["signal_type"],
                Id = (int) row["id"]
            };
            return lessonSignalDto;
            
            connection.Close();
            dataAdapter.Dispose();
            return null;
            

        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            var connectionString = _configuration.GetConnectionString("BotDatabase");
       
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand("INSERT INTO lesson_signal (signal_type, user_id, timestamp) " +
                                       "VALUES (@signalType, @userId, @timestamp)",
                connection);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            cmd.Parameters.AddWithValue("@signalType", signalType);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@timestamp", timestamp);
            cmd.ExecuteNonQuery();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            return Accepted();
        }
    }
}
