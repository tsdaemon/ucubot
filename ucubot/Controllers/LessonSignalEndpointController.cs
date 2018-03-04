﻿using System;
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
            // TODO: add query to get all signals
            var query = "SELECT * FROM lesson_signal";
            var conn = new MySqlConnection(connectionString);        
            var cmd = new MySqlCommand(query, conn);
            conn.Open();

            // create data adapter
            var da = new MySqlDataAdapter(cmd);
            var DataTable;
            // this will query your database and return the result to your datatable
            da.Fill(DataTable);
            conn.Close();
            
            //initialize list of objects
            
            foreach(DataRow row in DataTable.Rows)
            {
                var obj = new LessonSignalDto
                {
                    Id = (int)row["Id"],
                    UserId = (string)row["UserId"],
                    Type = (LessonSignalType)row["SignalType"],
                    Timestamp = (Datetime)row["Timestamp"]
                };
                // add object to list
            }
            
            /**/
            da.Dispose();
            
            // return list of objects
            return new LessonSignalDto[0];
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            // TODO: add query to get a signal by the given id
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            // TODO: add insert command to store signal
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            return Accepted();
        }
    }
}
