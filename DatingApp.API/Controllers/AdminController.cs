using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Identity;
using DatingApp.API.Models;

namespace DatingApp.API.Controllers
{
    // this uses policy based authorization assigned in startup.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(DataContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            // Executing Sql query directly
            var usersList = await (
                from user in _context.Users 
                orderby user.UserName 
                select new {
                    ID = user.Id,
                    Username = user.UserName,
                    Roles = (
                        from userRole in user.UserRoles
                        join role in _context.Roles 
                        on userRole.RoleId equals role.Id
                        select role.Name 
                    ).ToList()
                }
            ).ToListAsync();
            return Ok(usersList);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosForModeration() 
        {
            return Ok("Admins or Moderators can see this");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;

            // null collesing operator
            selectedRoles = selectedRoles ?? new string[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) {
                return BadRequest("Failed To Add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) {
                return BadRequest("Failed to remove roles");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}