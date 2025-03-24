using AppCalisto.Data.Entities;
using AppCalisto.Data.Repositories;
using Microsoft.AspNetCore.Identity;
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

            var seedRoles = new List<string> { "Admin", "PT Informática", "Global Eletrik", "Eficaz" };
            foreach (var role in seedRoles)
            {
                if (!await _roleRepository.ExistsAsync(role))
                {
                    await _roleRepository.CreateAsync(role);
                }
            }

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

                    var user = await CreateUser(name, email, roles);
                    users.Add(user);
                }

                await _context.SaveChangesAsync();
            }

            var clients = _context.Clients.ToList();
            if (clients == null || !clients.Any())
            {
                var seedClients = new List<(string name, string email, string contactPerson, string phone, string tax)>
                {
                    ("Client 1", "Person 1", "client@mail", "987654321", "123456789"),
                    ("Client 2", "Person 2", "client2@mail", "987654321", "123456789")
                };

                foreach (var (name, email, contactPerson, phone, tax) in seedClients)
                {
                    var client = CreateClient(name, email, contactPerson, phone, tax);
                    clients.Add(client);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> CreateUser(string name, string email, IEnumerable<string> roles)
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

            return user;
        }

        private Client CreateClient(string name, string contactPerson, string email, string phone, string tax)
        {
            var client = new Client()
            {
                Name = name,
                ContactPerson = contactPerson,
                Email = email,
                Phone = phone,
                Tax = tax
            };

            _context.Clients.Add(client);
            return client;
        }
    }
}
