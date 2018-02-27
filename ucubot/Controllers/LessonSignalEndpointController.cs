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
        
        private DataTable _dataTable = new DataTable();

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var LessonSignalArray = new List<LessonSignalDto>();

            var conn = new MySqlConnection(connectionString);
            var query = "SELECT * FROM LessonSignal;";
            var cmd = new MySqlCommand(query);
            
            conn.Open();

            using (var adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(_dataTable);

                foreach (DataRow row in _dataTable.Rows)
                {
                    LessonSignalArray.Add(new LessonSignalDto
                    {
                        Id = (int) row["ID"],
                        Timestamp = (DateTime) row["Timestamp_"],
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = row["UserID"].ToString()
                    });
                }
            }

            conn.Close();
            
            return LessonSignalArray;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
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
