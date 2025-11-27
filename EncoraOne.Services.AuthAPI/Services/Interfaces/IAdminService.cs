using System.Threading.Tasks;
using EncoraOne.Grievance.API.DTOs;
using EncoraOne.Grievance.API.Models; // Assuming your User model is here

namespace EncoraOne.Grievance.API.Services.Interfaces
{
    public interface IAdminService
    {
        /// <summary>
        /// Retrieves user details by email, typically for pre-population in update/delete forms.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>A DTO containing user information, or null if not found.</returns>
        Task<object?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Updates the details of an existing user based on the provided DTO.
        /// </summary>
        /// <param name="updateDto">Data transfer object containing user ID and updated fields.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task UpdateUserAsync(UpdateUserDto updateDto);

        /// <summary>
        /// Deletes a user permanently from the system.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>True if the user was deleted; false if the user was not found.</returns>
        Task<bool> DeleteUserAsync(int id);
    }
}