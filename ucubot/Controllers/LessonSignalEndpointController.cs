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
            var cnn = new MySqlConnection(connectionString);
            try
            {
                cnn.Open();
                Console.WriteLine("Connection Open ! ");

                var dataTable = "SELECT * FROM lesson_signal";
                var cmd = new MySqlCommand(dataTable, cnn);
                var rdr = cmd.ExecuteReader();                
                
                while (rdr.Read()) 
                {
                    Console.WriteLine(rdr.GetInt32(0) + ": " 
                                                      + rdr.GetString(1));
                }
                
                cnn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not open connection ! ");
            }
            //TODO: replace with database query
            return new LessonSignalDto[0];
        }
        
        [HttpGet]
        public LessonSignalDto ShowSignal(long id)
        {   
            //TODO: replace with database query
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();

            //TODO: add code to store above values
            
            return Accepted();
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add code to delete a record with the given id
            return Accepted();
        }
    }
}
