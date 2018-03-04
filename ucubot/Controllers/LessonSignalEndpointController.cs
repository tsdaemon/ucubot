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
            var con = new MySqlConnection(connectionString);
            var com = new MySqlCommand("SELECT * FROM lesson_signal", con);
            var datTab = new DataTable();
            
            try
            {
                con.Open();
                var datAd = new MySqlDataAdapter(com);
                datAd.Fill(datTab);
                datAd.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
          
            foreach (DataRow row in datTab.Rows)
            {
                var lesSigDto = new LessonSignalDto
                {
                    Id = (int) row["id"],
                    Timestamp = (DateTime) row["timestamp"],
                    Type = (LessonSignalType) row["signal_type"],
                    UserId = (string) row["user_id"]
                };
                yield return lesSigDto;
            }
            con.Close();
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var con = new MySqlConnection(connectionString);
            var com = new MySqlCommand("SELECT * FROM lesson_signal WHERE id=@id", con);
            com.Parameters.AddWithValue("@id", id);
            var datTab = new DataTable();
            
            try
            {   
                con.Open();
                var datAd = new MySqlDataAdapter(com);
                datAd.Fill(datTab);
                datAd.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (datTab.Rows.Count <= 0) return null;
            var row = datTab.Rows[0];
            con.Close();
            return new LessonSignalDto
            {
                Id = (int) row["id"],
                Timestamp = (DateTime) row["timestamp"],
                Type = (LessonSignalType) row["signal_type"],
                UserId = (string) row["user_id"]
            };

        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var curTime = string.Format("{0:HH:mm:ss tt}", DateTime.Now).ToString();
            var userId = message.user_id;
            var signalType = (int)message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var con = new MySqlConnection(connectionString);
            var com = new MySqlCommand("INSERT INTO lesson_signal (timestamp, signal_type, user_id) VALUES (@timestamp," +
                                       " @signal_type, @user_id);", con);
            com.Parameters.AddWithValue("@timestamp", DateTime.Now);
            com.Parameters.AddWithValue("@signal_type", signalType);
            com.Parameters.AddWithValue("@user_id", userId);
            try
            {   
                con.Open();
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            con.Close();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var con = new MySqlConnection(connectionString);
            var com = new MySqlCommand("DELETE FROM lesson_signal WHERE id=@id", con);
            com.Parameters.AddWithValue("@id", id);

            try
            {   
                con.Open();
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            con.Close();
            return Accepted();
        }
    }
}
