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
            
            var Connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            Connection.Open();
            var sqlexpressionAdapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", Connection);
            var data = new DataSet();
            sqlexpressionAdapter.Fill(data,"lesson_signal");

            foreach (DataRow row in data.Tables[0].Rows)
            {
                var signalDto = new LessonSignalDto
                {
                    Id = (int) row["id"], Timestamp = (DateTime) row["timestamp"],
                    Type = (LessonSignalType) Convert.ToInt32(row["signal_type"]),
                    UserId = (string) row["user_id"]
                };
               
                yield return signalDto;
            }
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var Connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            Connection.Open();
            var dataset = new DataSet();

            var cmd = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", Connection);
            cmd.Parameters.Add(new MySqlParameter("id", id));
            var dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(dataset, "lesson_signal");
            
            using (var result = dataset.Tables[0])
            {
                if (result.Rows.Count < 1) { return null; }
                var LsSignalDto = new LessonSignalDto
                    {
                        Id = (int) result.Rows[0]["id"], Timestamp = (DateTime) result.Rows[0]["timestamp"],
                        Type = (LessonSignalType) Convert.ToInt32(result.Rows[0]["signal_type"]),
                        UserId = (string) result.Rows[0]["user_id"]
                    };
                return LsSignalDto;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            var Connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            Connection.Open();

            var sqlQuery =
                new MySqlCommand("INSERT INTO lesson_signal (signal_type, user_id) VALUES (@signalType, @userId)",
                    Connection);
            sqlQuery.Parameters.Add(new MySqlParameter("signalType", signalType));
            sqlQuery.Parameters.Add(new MySqlParameter("userId", userId));
            sqlQuery.ExecuteNonQuery();
            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var Connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase"));
            Connection.Open();
            var sqlQuery = new MySqlCommand("DELETE FROM lesson_signal WHERE id = @id", Connection);
            sqlQuery.Parameters.Add(new MySqlParameter("id", id));
            sqlQuery.ExecuteNonQuery();
            return Accepted();
        }
    }
}