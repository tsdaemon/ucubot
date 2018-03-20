using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;
        private DataTable table = new DataTable();
            
        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT * FROM lesson_signal;";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);

            List<LessonSignalDto> lst = new List<LessonSignalDto>();
            foreach (DataRow element in table.Rows)
            {
                LessonSignalDto lesson = new LessonSignalDto();
                lesson.UserId = (string) element["user_id"];
                lesson.Type =  (LessonSignalType) element["signal_type"];
                lesson.Timestamp = Convert.ToDateTime(element["timestamp"]);
                lesson.Id = (int) element["id"];
                lst.Add(lesson);
            }
            
            connection.Close();
            adapter.Dispose();
            

            return lst;

        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {   
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            
            string sql = "SELECT * FROM lesson_signal WHERE id=" + id;
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            foreach (DataRow element in table.Rows)
            {
                LessonSignalDto lesson = new LessonSignalDto();
                lesson.UserId = (string) element["user_id"];
                lesson.Type = (LessonSignalType) element["signal_type"];
                lesson.Timestamp = Convert.ToDateTime(element["timestamp"]);
                lesson.Id = (int) element["id"];
                return lesson;
            }
            
            
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            var newCommand = new MySqlCommand("INSERT INTO lesson_signal (timestamp, signal_type, user_id) VALUES(@0, @1, @2);");
            var timestamp = DateTime.Now;
            newCommand.Parameters.AddWithValue("@0",timestamp);
            newCommand.Parameters.AddWithValue("@1", signalType);
            newCommand.Parameters.AddWithValue("@2", userId);
            newCommand.ExecuteNonQuery();
            connection.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            
            var newCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE id=" + id);
            newCommand.ExecuteNonQuery();
            connection.Close();
            
          
            return Accepted();
        }
    }
}
