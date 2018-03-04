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
        private string connectionString;
        private readonly MySqlConnection sqlConnection;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("BotDatabase");
            sqlConnection = new MySqlConnection(connectionString);
            
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
        
           OpenConnection();
            MySqlCommand command = new MySqlCommand("SELECT * FROM lesson_signal;", sqlConnection);
            MySqlDataAdapter query = new MySqlDataAdapter(command);
            DataSet data = new DataSet();
            query.Fill(data);

            foreach (DataRow row in data.Tables[0].Rows)
            {
                var signalDto = new LessonSignalDto();
                signalDto.Id = (int) row["id"];
                signalDto.Timestamp = (DateTime) row["timestamp_"];
                signalDto.Type = SignalTypeUtils.Decode(Convert.ToInt32(row["signal_type"]));
                signalDto.UserId = (string) row["user_id"];

                yield return signalDto;
            }

            CloseConnection();      
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
        
            OpenConnection();
            MySqlCommand command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id =@id;", sqlConnection);
            command.Parameters.AddWithValue("id", id);
            MySqlDataAdapter query = new MySqlDataAdapter(command);
            DataSet data = new DataSet();
            query.Fill(data);

            if (data.Tables[0].Rows.Count < 1)
                return null;
                
            var row = data.Tables[0].Rows[0];
            var signalDto = new LessonSignalDto
            {
                Id = (int) row["id"],
                Timestamp = (DateTime) row["timestamp_"],
                Type = SignalTypeUtils.Decode(Convert.ToInt32(row["signal_type"])),
                UserId = (string) row["user_id"]
            };
            CloseConnection(); 
            return signalDto;
            
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            OpenConnection();
            String userId = message.user_id;
            LessonSignalType  signalType;
            long number;
            bool result = long.TryParse(userId, out number);
            if (!result)
            {
                return Accepted();
            }
            try
            {
                signalType = message.text.ConvertSlackMessageToSignalType();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Accepted();
            }
            
            MySqlCommand command = new MySqlCommand("INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);",sqlConnection);
            command.Parameters.Add(new MySqlParameter("userId", userId));
            command.Parameters.Add(new MySqlParameter("signalType", signalType));
            command.ExecuteNonQueryAsync();
            
            CloseConnection();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            OpenConnection();
            if (id <= 0){
                return Accepted();
            }
            MySqlCommand command = new MySqlCommand("DELETE FROM lesson_signal WHERE ID = @id;", sqlConnection);
            command.Parameters.Add(new MySqlParameter("id", id));
            command.ExecuteNonQueryAsync();
            CloseConnection();
            return Accepted();
        }



        private void  OpenConnection()
        {
            sqlConnection.Open();
        }

        private void CloseConnection()
        {
            sqlConnection.Close();
        }
    }
}
