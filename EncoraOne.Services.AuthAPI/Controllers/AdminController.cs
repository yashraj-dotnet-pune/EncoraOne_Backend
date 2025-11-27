using System;
using System.Threading.Tasks;
using EncoraOne.Grievance.API.DTOs;
using EncoraOne.Grievance.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncoraOne.Grievance.API.Controllers
{
    [Authorize(Roles = "Admin")] // Ensures only Admins can access methods in this controller
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService; // You need to create this service

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: api/Admin/user/{email}
        // Endpoint for searching/retrieving user details by email
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var userDto = await _adminService.GetUserByEmailAsync(email);
                if (userDto == null)
                {
                    return NotFound(new { message = $"User with email '{email}' not found." });
                }
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while fetching user details.", details = ex.Message });
            }
        }

        // PUT: api/Admin/user
        // Endpoint for updating user details (name, role, job title, password)
        [HttpPut("user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateDto)
        {
            try
            {
                // This DTO will need to handle nullable fields (like password)
                await _adminService.UpdateUserAsync(updateDto);
                return NoContent(); // 204 No Content is standard for a successful PUT/Update
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while updating the user.", details = ex.Message });
            }
        }

        // DELETE: api/Admin/user/{id}
        // Endpoint for deleting a user by ID
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                bool result = await _adminService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"User with ID {id} not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while deleting the user.", details = ex.Message });
            }
        }
    }
}