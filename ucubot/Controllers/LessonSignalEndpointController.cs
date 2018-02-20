using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
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
            var obj = new List<LessonSignalDto>();

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                // Write a query, which selects all data from the table lesson signal
                //                                                             and stores it in a DataTable object

                var myCommand = new MySqlCommand("SELECT * FROM lesson_signal", myConnection);
                var dataAdapter = new MySqlDataAdapter(myCommand);

                dataAdapter.GetFillParameters();
                
                if (dataAdapter.ToString() != null) { dataAdapter.Fill(dataTable);}

                // Iterate over DataTable rows and convert each into a LessonSignalDto object.


                foreach (DataRow row in dataTable.Rows)
                {
                    var currDto = new LessonSignalDto
                    {
                        Id = (int) row["Id"],
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = (string) row["UserId"]
                    };

                    obj.Add(currDto);
                }

            }

            return obj;
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var dataTable = new DataTable();
            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                // Write a query, which selects all data from the table lesson signal
                //                                                             and stores it in a DataTable object

                var myCommand = new MySqlCommand("SELECT * FROM lesson_signal WHERE Id=" + id, myConnection);
                var dataAdapter = new MySqlDataAdapter(myCommand);

                dataAdapter.Fill(dataTable);

                // Iterate over DataTable rows and convert each into a LessonSignalDto object.

                foreach (DataRow row in dataTable.Rows)
                {
                    var currDto = new LessonSignalDto
                    {
                        Timestamp = (DateTime) row["Timestamp"],
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = (string) row["UserId"]
                    };
                    return currDto;
                }

            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();


            //TODO: add code to store above values
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return BadRequest();
                }
                
                const string mysqlCmdString = "INSERT INTO lesson_signal (UserId, SignalType) VALUES (@param1, @param2)";
                var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                cmd.Parameters.Add("@param1", MySqlDbType.Text).Value = userId;
                cmd.Parameters.Add("@param2", MySqlDbType.Int32).Value = signalType;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add code to delete a record with the given id
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return BadRequest();
                }
                
                var myCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE Id=" + id + ";", myConnection);

                return Accepted();
            }
        }
    }
}
