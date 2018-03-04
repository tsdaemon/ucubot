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
        
        public LessonSignalEndpointController(IConfiguration configuration){
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals(){
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var query = "SELECT * FROM lesson_signal";
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(query, conn);
            
            conn.Open();
            
            var adapter = new MySqlDataAdapter(cmd);
     
            adapter.Fill(dataTable);

            var list = new List<LessonSignalDto>();
            
            foreach (DataRow row in dataTable.Rows){
                list.Add(new LessonSignalDto{
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["time_stamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                });
            }
            
            conn.Close();
            
            return list;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id){
            // TODO: add query to get a signal by the given id
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message){
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id){
            //TODO: add delete command to remove signal
            return Accepted();
        }
    }
}
