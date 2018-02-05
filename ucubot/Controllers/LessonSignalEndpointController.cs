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

                var myCommand = new MySqlCommand("SELECT * FROM lessonSignal", myConnection);
                var dataAdapter = new MySqlDataAdapter(myCommand);

                dataAdapter.Fill(dataTable);

                // Iterate over DataTable rows and convert each into a LessonSignalDto object.


                foreach (DataRow row in dataTable.Rows)
                {
                    var currDto = new LessonSignalDto();
                    currDto.Timestamp = (DateTime) row["Timestamp"];
                    currDto.Type = (LessonSignalType) row["SignalType"];
                    currDto.UserId = (string) row["UserId"];


                    obj.Add(currDto);
                }

            }

//                (d) Return all LessonSignalDto objects stored in any IEnumerable
//            container: List, array, iterator etc.
//                (e) Do not forget to close your connection.
            return obj;
        }

        [HttpGet]
        public LessonSignalDto ShowSignal(long id)
        {
            //TODO: replace with database query
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

                var myCommand = new MySqlCommand("SELECT * FROM lessonSignal WHERE Id=" + id, myConnection);
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
            var userId = message.UserId;
            var signalType = message.Text.ConvertSlackMessageToSignalType();

            //TODO: add code to store above values


            return Accepted();
        }

        [HttpDelete]
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
                }

                var myCommand = new MySqlCommand("DELETE FROM lessonSignal WHERE Id=" + id, myConnection);
                return Accepted();
            }
        }
    }
}
