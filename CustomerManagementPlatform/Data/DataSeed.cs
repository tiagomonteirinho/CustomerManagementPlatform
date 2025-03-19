using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagementPlatform.Data
{
    public class DataSeed
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IAccountHelper _accountHelper;

        public DataSeed(DataContext context, IUserRepository userRepository, IAccountHelper accountHelper)
        {
            _context = context;
            _userRepository = userRepository;
            _accountHelper = accountHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await _accountHelper.EnsureCreatedRoleAsync("Admin");
            await _accountHelper.EnsureCreatedRoleAsync("Employee");
            await _accountHelper.EnsureCreatedRoleAsync("Customer");

            var users = await _userRepository.GetAllAsync();
            if (users == null || users.Count <= 1)
            {
                var seedUsers = new List<(string fullName, string email, string role)>
                {
                    ("Admin", "admin@mail", "Admin"),
                    ("Employee", "employee@mail", "Employee"),
                    ("Customer", "customer@mail", "Customer"),
                    ("Customer 2", "customer2@mail", "Customer"),
                };

                foreach (var (fullName, email, role) in seedUsers)
                {
                    var user = await CreateUser(fullName, email, role);
                    users.Add(user);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> CreateUser(string fullName, string email, string role)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                };

                var result = await _userRepository.CreateAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Could not create seed user.");
                }

                await _accountHelper.AddToRoleAsync(user, role);
                if (!await _accountHelper.IsInRoleAsync(user, role))
                {
                    throw new InvalidOperationException($"Could not add seed user to role.");
                }

                var token = await _accountHelper.GenerateEmailConfirmationTokenAsync(user);
                await _accountHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }
    }
}
