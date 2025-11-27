using EncoraOne.Grievance.API.DTOs;
using EncoraOne.Grievance.API.Models;
using EncoraOne.Grievance.API.Data;
using EncoraOne.Grievance.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Grievance.API.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public AdminService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // --- GET USER BY EMAIL ---
        public async Task<object?> GetUserByEmailAsync(string email)
        {
            // The use of _context.Set<User>() is correct for TPT inheritance.
            var user = await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return null;
            }

            var userDto = new
            {
                user.Id,
                user.FullName,
                user.Email,
                Role = (int)user.Role,
                user.IsActive,

                // 💡 REFINEMENT: Ensure you handle cases where jobTitle/DepartmentId might 
                // be null, especially if the user is an Admin (which is a Manager in your seed data).
                // The current pattern matching is correct.
                JobTitle = user is Employee employee ? employee.JobTitle :
                           user is Manager manager1 ? manager1.JobTitle : null,

                DepartmentId = user is Manager manager2 ? manager2.DepartmentId : (int?)null
            };

            return userDto;
        }

        // --- UPDATE USER ---
        public async Task UpdateUserAsync(UpdateUserDto updateDto)
        {
            var userToUpdate = await _context.Set<User>()
           .FirstOrDefaultAsync(u => u.Id == updateDto.Id);

            if (userToUpdate == null)
            {
                throw new ArgumentException($"User with ID {updateDto.Id} not found.");
            }

            // --- Core Updates: Only apply changes if the DTO provides a value ---

            // 2. Update common base properties (if provided)
            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
            {
                userToUpdate.FullName = updateDto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
            {
                userToUpdate.Email = updateDto.Email;
            }

            if (updateDto.IsActive.HasValue)
            {
                userToUpdate.IsActive = updateDto.IsActive.Value;
            }

            // 3. Handle Password update (if provided)
            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                // Use the authentication service to hash the new password
                userToUpdate.PasswordHash = (string)_authService.HashPassword(updateDto.Password);
            }

            // 4. Handle Role and Department ID/JobTitle updates based on DTO and current type

            // --- A. Handle Role Change ---
            if (updateDto.Role.HasValue)
            {
                var newRole = (UserRole)updateDto.Role.Value;
                userToUpdate.Role = newRole; // Update the role on the base entity

                // If changing to Manager, ensure DepartmentId is set
                if (newRole == UserRole.Manager)
                {
                    if (userToUpdate is Manager manager)
                    {
                        // Update DepartmentId if a value is provided in the DTO
                        if (updateDto.DepartmentId.HasValue)
                        {
                            manager.DepartmentId = updateDto.DepartmentId.Value;
                        }
                        // Validation should prevent null DepartmentId if required for Manager.
                    }
                }
                else // Switching away from Manager (e.g., to Employee or Admin)
                {
                    // Clear Manager-specific property (DepartmentId)
                    if (userToUpdate is Manager manager)
                    {
                        manager.DepartmentId = 0; // Assuming 0 or another default value
                    }
                }
            }

            // --- B. Handle JobTitle Update (for Employee or Manager) ---
            if (!string.IsNullOrWhiteSpace(updateDto.JobTitle))
            {
                if (userToUpdate is Employee employee)
                {
                    employee.JobTitle = updateDto.JobTitle;
                }
                else if (userToUpdate is Manager manager)
                {
                    manager.JobTitle = updateDto.JobTitle;
                }
                // Admin is not a derived type here, so we only handle Employee/Manager
            }

            // --- C. Handle DepartmentId Update (if Role was NOT changed, but property was provided) ---
            // This handles updates to an existing Manager's DepartmentId without a role change
            if (!updateDto.Role.HasValue && userToUpdate is Manager existingManager && updateDto.DepartmentId.HasValue)
            {
                existingManager.DepartmentId = updateDto.DepartmentId.Value;
            }

            // 5. Save changes
            await _context.SaveChangesAsync();
        }

        // --- DELETE USER ---
        public async Task<bool> DeleteUserAsync(int id)
        {
            var userToDelete = await _context.Set<User>().FindAsync(id);

            if (userToDelete == null)
            {
                return false;
            }

            _context.Set<User>().Remove(userToDelete);
            // This correctly removes the entity from the base table and cascade deletes 
            // from the derived table in TPT.
            await _context.SaveChangesAsync();

            return true;
        }
    }
}