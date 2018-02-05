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
            string query = "select * from lesson_signal where id =";//to do connection.Open();
            MySqlCommand cmd = new  MySqlCommand(query, connection);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(dataTable);
            
            
            
            connection.Close();
            dataAdapter.Dispose();
            return null;
            

        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            
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
