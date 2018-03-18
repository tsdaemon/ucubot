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
            DataTable dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get all signals
            var connection = new MySqlConnection(connectionString);
            var select = "SELECT * FROM lesson_signal";
            var cmd = new MySqlCommand(select, connection);
            var da = new MySqlDataAdapter(cmd);
            connection.Open();
            da.Fill(dataTable);
            connection.Close();
            var dat = new List<LessonSignalDto>();
            foreach(DataRow row in dataTable.Rows)
            {
                dat.Add(new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["time_stamp"],
                    Type = (LessonSignalType)Convert.ToInt32(row["signaltype"]),
                    UserId = (string) row["userid"]
                });
            }
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
            cmd.Parameters.AddWithValue("@id", id);
            var da = new MySqlDataAdapter(cmd);
            da.Fill(dataTable);
            if (dataTable.Rows.Count < 1)
            {
                return null;
            }
            var row = dataTable.Rows[0];
            var signal = new LessonSignalDto();
                signal = new LessonSignalDto
                {
                    Timestamp = (DateTime) row["time_stamp"],
                    Type = (LessonSignalType)Convert.ToInt32(row["signaltype"]),
                    UserId = (string) row["userid"]
                };

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
                "INSERT INTO lesson_signal (userid, signaltype) VALUES (@userid, @signaltype);";
            cmd.Parameters.AddRange(new[]
            {
                new MySqlParameter("userid", userId),
                new MySqlParameter("signaltype", signalType)
            });
            cmd.ExecuteNonQuery();
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
            cmd.Parameters.Add(new MySqlParameter("@id", id));
            cmd.ExecuteNonQuery();
            return Accepted();
          }
    }
}
