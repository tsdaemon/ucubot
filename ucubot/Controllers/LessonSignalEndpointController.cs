using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Rewrite.Internal.PatternSegments;
using Microsoft.CodeAnalysis.Diagnostics;
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
            // TODO: add query to get all signals
            return new LessonSignalDto[0];
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            LessonSignalDto lessonSignalDto = new LessonSignalDto();

            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select lesson_signal from lesson_signals where id='@id';",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            lessonSignalDto.Id = reader.GetInt32("id");
                            lessonSignalDto.Timestamp = reader.GetDateTime("Timestamp");
                            lessonSignalDto.UserId = reader.GetString("UserId");
                            lessonSignalDto.Type =
                                SignalTypeUtils.ConvertSlackMessageToSignalType(reader.GetString("signal_type"));

                        }

                    }
                }

            }



            return lessonSignalDto;

        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();


            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                connection.Open();
                String str =
                    "INSERT INTO lesson_signal (signalType, Timestamp, userId) VALUES (@signalType, @Date, @userId)";
                using (MySqlCommand cmd = new MySqlCommand(str, connection))
                {
                    cmd.Parameters.AddWithValue("@signalType", signalType);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    if (cmd.ExecuteNonQuery() == -1)
                    {
                        return Forbid();
                    }
                }

            }



            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {

            LessonSignalDto lessonSignalDto = new LessonSignalDto();

            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("delete  lesson_signal from lesson_signals where id='@id';",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    if (cmd.ExecuteNonQuery() == -1)
                    {
                        return Forbid();
                    }
                }


            }

            return Accepted();
        }
    }
}
    
