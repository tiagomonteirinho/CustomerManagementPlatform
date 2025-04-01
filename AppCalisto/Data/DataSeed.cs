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
            await CreateOrders();
        }

        public async Task CreateRoles()
        {
            var seedRoles = new List<string> { "Admin", "Back-office", "Front-office" };
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
                    ("Admin", "admin@mail", new List<string> { "Admin", "Back-office", "Front-office" }),
                    ("Employee", "employee@mail", new List<string> { "Back-office", "Front-office" }),
                    ("Employee 2", "employee2@mail", new List<string> { "Back-office" })
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
                    new Client { Name = "Client 1", ContactPerson = "Person 1", Email = "client@mail", Phone = "111111111", Tax = "111111111" },
                    new Client { Name = "Client 2", ContactPerson = "Person 2", Email = "client2@mail", Phone = "222222222", Tax = "222222222" }
                };

                _context.Clients.AddRange(seedClients.AsEnumerable().Reverse());
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateOrders()
        {
            if (!await _context.Orders.AnyAsync())
            {
                var seedOrders = new List<Order>
                {
                    new Order { Number = "111", Type = "Maintenance", Location = "Client 1's Office", Description = "Systems maintenance.", Status = "Ongoing", ClientId = 1 },
                    new Order { Number = "222", Type = "Software Development", Location = "Client 2's Office", Description = "Application development.", Status = "Ongoing", ClientId = 2 }
                };

                foreach (var order in seedOrders)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == order.ClientId);
                    if (client == null)
                    {
                        throw new InvalidOperationException($"Could not add seed order to client.");
                    }

                    order.Client = client;
                }

                _context.Orders.AddRange(seedOrders.AsEnumerable().Reverse());
                await _context.SaveChangesAsync();
            }
        }
    }
}
