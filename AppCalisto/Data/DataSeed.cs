using AppCalisto.Data.Entities;
using AppCalisto.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppCalisto.Data
{
    public class DataSeed
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IAccountHelper _accountHelper;
        private readonly IRoleHelper _roleHelper;

        public DataSeed(DataContext context, IUserRepository userRepository, IAccountHelper accountHelper, IRoleHelper roleHelper)
        {
            _context = context;
            _userRepository = userRepository;
            _accountHelper = accountHelper;
            _roleHelper = roleHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await _roleHelper.EnsureCreatedRoleAsync("Admin");
            await _roleHelper.EnsureCreatedRoleAsync("Employee");

            var users = await _userRepository.GetAllAsync();
            if (users == null || users.Count <= 1)
            {
                var seedUsers = new List<(string name, string email, string role)>
                {
                    ("Admin", "admin@mail", "Admin"),
                    ("Employee", "employee@mail", "Employee"),
                    ("Employee 2", "employee2@mail", "Employee")
                };

                foreach (var (name, email, role) in seedUsers)
                {
                    var user = await CreateUser(name, email, role);
                    users.Add(user);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> CreateUser(string name, string email, string role)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                };

                var result = await _userRepository.CreateAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Could not create seed user.");
                }

                await _userRepository.AddToRoleAsync(user, role);
                if (!await _userRepository.IsInRoleAsync(user, role))
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
