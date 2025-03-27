using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data
{
    public class DataSeed
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public DataSeed(DataContext context, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CreateRoles();
            await CreateUsers();
            await CreateClients();
        }

        public async Task CreateRoles()
        {
            var seedRoles = new List<string> { "Admin", "PT Informática", "Global Eletrik", "Eficaz" };
            foreach (var role in seedRoles)
            {
                if (!await _roleRepository.ExistsAsync(role))
                {
                    await _roleRepository.CreateAsync(role);
                }
            }
        }

        public async Task CreateUsers()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || users.Count <= 1)
            {
                var seedUsers = new List<(string name, string email, IEnumerable<string> roles)>
                {
                    ("Admin", "admin@mail", new List<string> { "Admin", "PT Informática", "Global Eletrik", "Eficaz" }),
                    ("Employee", "employee@mail", new List<string> { "PT Informática" }),
                    ("Employee 2", "employee2@mail", new List<string> { "Global Eletrik", "Eficaz" })
                };

                foreach (var (name, email, roles) in seedUsers)
                {
                    foreach (var role in roles)
                    {
                        if (!await _roleRepository.ExistsAsync(role))
                        {
                            throw new InvalidOperationException($"Could not add seed user to role.");
                        }
                    }

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

                        if (await _userRepository.CreateAsync(user, "123456") != IdentityResult.Success)
                        {
                            throw new InvalidOperationException($"Could not create seed user.");
                        }

                        await _userRepository.AddToRolesAsync(user, roles);
                        foreach (var role in roles)
                        {
                            if (!await _userRepository.IsInRoleAsync(user, role))
                            {
                                throw new InvalidOperationException($"Could not add seed user to role.");
                            }
                        }

                        var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                        await _userRepository.ConfirmEmailAsync(user, token);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateClients()
        {
            if (!await _context.Clients.AnyAsync())
            {
                var seedClients = new List<Client>
                {
                    new Client { Name = "Client 1", ContactPerson = "Person 1", Email = "client@mail", Phone = "987654321", Tax = "123456789" },
                    new Client { Name = "Client 2", ContactPerson = "Person 2", Email = "client2@mail", Phone = "987654321", Tax = "123456789" }
                };

                _context.Clients.AddRange(seedClients);
                await _context.SaveChangesAsync();
            }
        }
    }
}
