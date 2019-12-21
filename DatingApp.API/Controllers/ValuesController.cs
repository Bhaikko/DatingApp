using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.API.Controllers
{
    // [Authorize] // This is added such that all the requests inside this Controller must be a valid Request
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
        
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetValues () {
            // To retrieve values from database and values table
            var values = await this._context.Values.ToListAsync(); // this is asynchronous, the thread will not be blocked
            
            return Ok(values);
        }

        [Authorize(Roles = "Member")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue (int id) {
            var value = await this._context.Values.FirstOrDefaultAsync(x => x.Id == id);

            return Ok(value);
        }
    }
}
