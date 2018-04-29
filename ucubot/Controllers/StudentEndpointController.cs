using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class StudentEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet]
        public IEnumerable<Student> ShowStudent()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var command = "SELECT student.id AS Id, student.first_name AS FirstName, " +
                              "student.last_name AS LastName, student.user_id AS UserId FROM student;";
                List<Student> signals = conn.Query<Student>(command).ToList();
                return signals;
            }
        }

        [HttpGet("{id}")]
        public Student ShowStudent(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var command = "SELECT student.id AS Id, student.first_name AS FirstName, " +
                              "student.last_name AS LastName, student.user_id AS UserId FROM student WHERE student.id=@id;";
                Student student = conn.Query<Student>(command, new {id}).SingleOrDefault();
                if (student == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }

                return student;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(StudentDet info)
        {
            var firstName = info.first_name;
            var lastName = info.last_name;
            var userId = info.user_id;
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var countSql = "SELECT COUNT(*) FROM student WHERE user_id=@UserId";
                var num_stud = conn.ExecuteScalar<int>(countSql, new {userId});
                if (num_stud == 1)
                {
                    return StatusCode(409);
                }
                else
                {
                    var sqlQuery =
                        "INSERT INTO student(first_name, last_name, user_id) VALUES (@FirstName,@LastName,@UserId)";
                    conn.Execute(sqlQuery,
                        new
                        {
                            firstName,
                            lastName,
                            userId
                        });
                }
            }

            return Accepted();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStudent(Student student)
        {
            var firstName = student.FirstName;
            var lastName = student.LastName;
            var userId = student.UserId;
            var Id = student.Id;

            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var countSql = "SELECT COUNT(*) FROM student WHERE user_id=@UserId";
                var num_stud = conn.ExecuteScalar<int>(countSql, new {userId});
                if (num_stud == 1)
                {
                    return StatusCode(409);
                }
                else
                {
                    var sqlQuery =
                        "UPDATE student SET first_name=@FirstName, last_name=@LastName, user_id=@UserId WHERE id=@Id;";
                    conn.Execute(sqlQuery,
                        new
                        {
                            firstName,
                            lastName,
                            userId,
                            Id,
                        });
                }
            }

            return Accepted();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add delete command to remove signal
            var connectionString = _configuration.GetConnectionString("BotDatabase");


            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var countSql = "SELECT COUNT(*) FROM lesson_signal WHERE student_id=@Id";
                var num_sig = conn.ExecuteScalar<int>(countSql, new {id});
                if (num_sig > 1)
                {
                    return StatusCode(409);
                }
                else
                {
                    var sqlQuery = "DELETE FROM student WHERE id=@Id;";
                    conn.Execute(sqlQuery,
                        new {id});
                }
            }

            return Accepted();
        }
    }
}