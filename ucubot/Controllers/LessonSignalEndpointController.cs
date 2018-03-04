using System;
using System.Collections.Generic;
using System.Data;
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
            var dataTable = new DataTable();
            var connection = new MySqlConnection(connectionString);
            var command = new MySqlCommand("select * from lesson_signal",connection);
            connection.Open();
            var da = new MySqlDataAdapter(command);
            da.Fill(dataTable);
            connection.Close();
            da.Dispose();
            var ret = new LessonSignalDto[dataTable.Rows.Count];
            int i = 0;    
            foreach (DataRow row in dataTable.Rows)
            {
                ret[i] = new LessonSignalDto();
                ret[i].Id = (int)row["Id"];
                ret[i].Timestamp = (DateTime)row["DateTime"];
                ret[i].Type = (LessonSignalType) row["SignalType"];
                ret[i].UserId = (string) row["UserId"];
                i++;
            }
            return ret;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var dataTable = new DataTable();
            var connection = new MySqlConnection(connectionString);
            var command = new MySqlCommand($"select * from lesson_signal where id={id}",connection);
            connection.Open();
            var da = new MySqlDataAdapter(command);
            da.Fill(dataTable);
            connection.Close();
            da.Dispose();         
            if (dataTable.Rows.Count > 0)
            {
                var ret = new LessonSignalDto();
                DataRow row = dataTable.Rows[0];
                ret.Id = (int) row["Id"];
                ret.Timestamp = (DateTime) row["DateTime"];
                ret.Type = (LessonSignalType) row["SignalType"];
                ret.UserId = (string) row["UserId"];
                return ret;
            }
            else
            {
                return null;
            }
            
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            int signalType;
            try
            {
               signalType = (int) message.text.ConvertSlackMessageToSignalType();
            }
            catch (CanNotParseSlackCommandException e)
            {
                return BadRequest();
            }
            
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            var command = new MySqlCommand($"insert into lesson_signal (UserId, SignalType) values (@user_id, @signalType);",connection);
            command.Parameters.Add(new MySqlParameter(){ParameterName = "@user_id",Value = userId});
            command.Parameters.Add(new MySqlParameter(){ParameterName = "@signalType",Value = signalType});
            
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            var command = new MySqlCommand($"delete from lesson_signal where id={id}",connection);
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
            return Accepted();
        }
    }
}
