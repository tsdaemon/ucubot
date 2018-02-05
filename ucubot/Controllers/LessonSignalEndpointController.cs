using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            
            MySqlConnection conn = new MySqlConnection(connectionString);
            DataTable dataTable = new DataTable();

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                string sql = "SELECT * FROM lesson signal";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            
            List<LessonSignalDto> lessonSignalDtos = new List<LessonSignalDto>();
            
            foreach(DataRow row in dataTable.Rows)
            {
                LessonSignalDto lessonSignalDto = new LessonSignalDto();
                lessonSignalDto.UserId = (String) row["user_id"];
                lessonSignalDto.Type = SignalTypeUtils.ConvertSlackMessageToSignalType((String) row["signal_type"]);
                lessonSignalDto.Timestamp = (DateTime) row["timestamp"];
                lessonSignalDtos.Add(lessonSignalDto);
            }
            conn.Close();
            return lessonSignalDtos;
        }
        
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {   
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            MySqlConnection conn = new MySqlConnection(connectionString);
            DataTable dataTable = new DataTable();


            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                string sql = "SELECT * FROM lesson_signal WHERE id =" + id.ToString();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            var row = dataTable.Rows[0];
            LessonSignalDto lessonSignalDto = new LessonSignalDto();
            lessonSignalDto.UserId = (String) row["user_id"];
            lessonSignalDto.Type = SignalTypeUtils.ConvertSlackMessageToSignalType((String) row["signal_type"]);
            lessonSignalDto.Timestamp = (DateTime) row["timestamp"];

            if (dataTable.Rows.Count == 0)
            {
                Console.WriteLine("No records found :(");
                return null;
            }
        
            conn.Close();
            return lessonSignalDto;
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
