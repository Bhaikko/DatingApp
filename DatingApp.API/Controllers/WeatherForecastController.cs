using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    // [ApiController]
    // [Route("[controller]")] // http://localhost:5000/[controller]
    // public class WeatherForecastController : ControllerBase
    // {
    //     private static readonly string[] Summaries = new[]
    //     {
    //         "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //     };

    //     private readonly ILogger<WeatherForecastController> _logger;

    //     public WeatherForecastController(ILogger<WeatherForecastController> logger)
    //     {
    //         _logger = logger;
    //     }

    //     [HttpGet]
    //     public IEnumerable<WeatherForecast> Get()
    //     {
    //         var rng = new Random();
    //         return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //         {
    //             Date = DateTime.Now.AddDays(index),
    //             TemperatureC = rng.Next(-20, 55),
    //             Summary = Summaries[rng.Next(Summaries.Length)]
    //         })
    //         .ToArray();
    //     }
    // }

    // Naming convention works like this
    // THe  word before the Controller when specifing the class becomes the route 
    // for eg, values + Controller make the class name hence values is the route
    // /api/values
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase 
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context) {
            this._context = context;
        }

        // This code is synchronous
        // [HttpGet]
        // public IActionResult GetValues () {
        //     // To retrieve values from database and values table
        //     var values = this._context.Values.ToList(); // this is synchronous, the thread will be blocked
            
        //     return Ok(values);
        // }
        
        [HttpGet]
        public async Task<IActionResult> GetValues () {
            // To retrieve values from database and values table
            var values = await this._context.Values.ToListAsync(); // this is asynchronous, the thread will not be blocked
            
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue (int id) {
            var value = await this._context.Values.FirstOrDefaultAsync(x => x.Id == id);

            return Ok(value);
        }
    }
}
