﻿using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Vehicles2.Api.Data.Entities;
using Vehicles2.Api.Models;

namespace Vehicles2.Api.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<User> GetUserByIdAsync(string id);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> DeleteUserAsync(User user);

    }
}