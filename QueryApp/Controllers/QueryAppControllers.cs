using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using QueryApp.Models;

namespace QueryApp.Controllers
{
    public class DataController : Controller
    {
        private readonly ILogger<DataController> _logger;
        private readonly string _connectionString;

        public DataController(ILogger<DataController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("WorldDbContext");
        }

        public IActionResult ShowData()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Dapper query to retrieve country languages between two populations
                    var sql = @"
                        SELECT * 
                        FROM countrylanguage 
                        WHERE Percentage BETWEEN @MinPercentage AND @MaxPercentage";

                    _logger.LogInformation($"Executing SQL query: {sql}");

                    var result = connection.Query<CountryLanguage>(sql, new { MinPercentage = 3.0, MaxPercentage = 10.0 });

                    _logger.LogInformation($"Query result count: {result?.Count()}");

                    // Pass the result to the view
                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");

                // Handle the error, e.g., return an error view or message
                return View("Error");
            }
        }
    }
}
