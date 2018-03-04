using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            DataTable dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get all signals
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            string select = "SELECT * FROM lesson_signals";
            var da = new MySqlDataAdapter(select, connection);
            da.Fill(dataTable);
            IEnumerable<LessonSignalDto> dat = new List<LessonSignalDto>(); 
            foreach(DataRow row in dataTable.Rows)
            {
                var datan = new LessonSignalDto
                {
                    Id = (int) row["Id"],
                    Timestamp = (DateTime) row["Timestamp"],
                    Type = (LessonSignalType)Convert.ToInt32(row["SignalType"]),
                    UserId = (string) row["UserId"]
                };
                dat.Append(datan);
            }
            connection.Close();
            da.Dispose();
            return dat;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            DataTable dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("id", id);
            var da = new MySqlDataAdapter(cmd);
            da.Fill(dataTable);
            if (dataTable.Rows.Count < 1)
            {
                return null;
            }
            var signal = new LessonSignalDto();
            foreach(DataRow row in dataTable.Rows)
            {
                signal = new LessonSignalDto
                {
                    Timestamp = (DateTime) row["Timestamp"],
                    Type = (LessonSignalType)Convert.ToInt32(row["SignalType"]),
                    UserId = (string) row["UserId"]
                };
            }
            connection.Close();
            da.Dispose();
            return signal;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                "INSERT INTO lesson_signal (UserId, SignalType) VALUES (@userId, @signalType);";
            cmd.Parameters.AddRange(new[]
            {
                new MySqlParameter("UserId", userId),
                new MySqlParameter("SignalType", signalType)
            });
            await cmd.ExecuteNonQueryAsync();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                "DELETE INTO lesson_signal WHERE id = @id;";
            cmd.Parameters.Add(new MySqlParameter("id", id));
            await cmd.ExecuteNonQueryAsync();
            return Accepted();
        }
    }
}
