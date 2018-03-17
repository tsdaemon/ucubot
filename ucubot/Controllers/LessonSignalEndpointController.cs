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
            var conn  = new MySqlConnection(connectionString);
            conn.Open();
            var query = new MySqlCommand("SELECT * FROM lesson_signal", conn);
            var datatable = new DataTable();
            var adapter = new MySqlAdapter(query);
            try
            {
                conn.Open();
                adapter.Fill(datatable);
                conn.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            var Signals = new List<LessonSignalDto>();
            foreach (DataRow row in datatable.Rows){
            	var signalDto = new LessonSignalDto
            	{
            		Timestamp = (DateTime) row["timestamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    user_id = (string) row["user_id"]
            	};
            	Signals.Add(signalDto);
            }
            return Signals;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn  = new MySqlConnection(connectionString);
            var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", conn);
            command.Parameters.AddWithValue("@id", id);
            var adapter = new MySqlDataAdapter(command);
            
            var datatable = new DataTable();
            try
            {
                conn.Open();
                adapter.Fill(datatable);
                conn.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
                
            adapter.Fill(datatable, "lesson_signal");
            if (datatable.Rows.Count < 1)
            {
                return null;
            }

            var row = datatable.Rows[0];
            var signalDto = new LessonSignalDto
            {
             	Timestamp = (DateTime) row["timestamp"],
                Type = (LessonSignalType) row["signal_type"],
                UserId = (string) row["user_id"]
            };
            return signalDto;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn  = new MySqlConnection(connectionString);
            var CommandText ="INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType)";
            var command = new MySqlCommand(CommandText, conn);
            command.Parameters.AddWithValue("@user_id", userId);
            command.Parameters.AddWithValue("@signal_type", signalType);
            try
            {
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var conn  = new MySqlConnection(connectionString);
            var command = conn.CreateCommand();
            command.CommandText ="DELETE FROM lesson_signal WHERE ID = @id;";
            command.Parameters.AddWithValue("@id", id);
            try
            {
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);                
            }
            return Accepted();
        }
    }
}
