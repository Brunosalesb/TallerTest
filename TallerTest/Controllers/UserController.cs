using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TallerTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("/{username}")]
        public IActionResult Welcome(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required.");

            try
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    const string query = "SELECT name FROM Users WHERE username = @username";
                    
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.NVarChar) { Value = username });
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return NotFound("User not found.");
                        
                            var name = reader["name"].ToString();
                            return Ok($"Hello, {name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}