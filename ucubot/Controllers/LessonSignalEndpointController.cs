using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                //connecting to mysql
                conn.Open();
                string query = "SELECT * FROM lessons_signal;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                List<LessonSignalDto> result = new List<LessonSignalDto>();
                foreach(DataRow row in dt.Rows)
                {
                    result.Add(new LessonSignalDto
                    {
                        Id = (int) row["Id"],
                        user_id = (string) row["user_id"],
                        Type = (LessonSignalType) row["signal_type"],
                        Timestamp = Convert.ToDateTime(row["Timestemp"])
                    });
                    
                    
                }
                conn.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                return new LessonSignalDto[0];
            }
            
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                //connecting to mysql

                conn.Open();
                string query = "SELECT * FROM lesson_signal WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@id", id);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    return null;
                }

                var row = dt.Rows[0];
                return new LessonSignalDto
                {
                    Id = (int) row["Id"],
                    user_id = (string) row["user_id"],
                    Type = (LessonSignalType) row["signal_type"],
                    Timestamp = Convert.ToDateTime(row["Timestemp"])
                };





            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                return new LessonSignalDto();
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var user_id = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                //connecting to mysql
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText =
                    "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@user_id, @signal_type);";
                command.Parameters.AddRange(new[]
                {
                    new MySqlParameter("user_id", user_id),
                    new MySqlParameter("signal_type", signalType)
                });
                await command.ExecuteNonQueryAsync();
                
            }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                return NotFound();
            }
            
            return Accepted();
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                //connecting to mysql
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText =
                    "DELETE FROM lesson_signal WHERE ID = @id;";
                command.Parameters.Add(new MySqlParameter("id", id));
                await command.ExecuteNonQueryAsync();
                
                return Accepted();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                return NotFound();
            }
            
        }
        }
    }

