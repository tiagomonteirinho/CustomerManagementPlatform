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
            await CreateCompanies();
            await CreateServices();
            await CreateOrders();
        }

        public async Task CreateRoles()
        {
            var seedRoles = new List<string> { "Admin", "Back-office", "Technician" };
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
                    ("Admin", "admin@mail", new List<string> { "Admin", "Back-office", "Technician" }),
                    ("Admin 2", "admin2@mail", new List<string> { "Admin" }),
                    ("Employee", "employee@mail", new List<string> { "Back-office" }),
                    ("Employee 2", "employee2@mail", new List<string> { "Back-office" }),
                    ("Technician", "technician@mail", new List<string> { "Technician" })
                };

                foreach (var (name, email, roles) in seedUsers)
                {
                    foreach (var role in roles)
                    {
                        if (!await _roleRepository.ExistsAsync(role))
                        {
                            throw new InvalidOperationException($"Could not find seed role.");
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
                                throw new InvalidOperationException($"Seed user {user.Id} not related to seed role {role}.");
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

        public async Task CreateCompanies()
        {
            if (!await _context.Companies.AnyAsync())
            {
                var seedCompanies = new List<Company>
                {
                    new Company { Name = "Singela Vertente Unipessoal, Lda", Abbreviation = "SINGV" },
                    new Company { Name = "Company 2", Abbreviation = "COMP2" }
                };

                _context.Companies.AddRange(seedCompanies.AsEnumerable().Reverse());
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateServices()
        {
            if (!await _context.Services.AnyAsync())
            {
                var seedServices = new List<Service>
                {
                    new Service { Name = "PT Informática", Abbreviation = "PTINF", CompanyId = 1 },
                    new Service { Name = "Global Eletrik", Abbreviation = "ELETR", CompanyId = 1 },
                    new Service { Name = "Eficaz", Abbreviation = "EFICZ", CompanyId = 1 },
                    new Service { Name = "Singela Vertente Unipessoal, Lda", Abbreviation = "SINGV", CompanyId = 1 },
                    new Service { Name = "Company 2", Abbreviation = "COMP2", CompanyId = 2 },
                };

                foreach (var service in seedServices)
                {
                    var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == service.CompanyId);
                    if (company == null)
                    {
                        throw new InvalidOperationException($"Could not find seed company.");
                    }

                    service.Company = company;
                }

                _context.Services.AddRange(seedServices.AsEnumerable().Reverse());
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateOrders()
        {
            if (!await _context.Orders.AnyAsync())
            {
                var technician = await _userRepository.GetByEmailAsync("technician@mail");
                var seedOrders = new List<Order>
                {
                    new Order { Location = "Client 1's Office", Description = "Application development.", Status = "Ongoing", 
                        ClientId = 1, ServiceId = 1, TechnicianId = technician.Id },
                    new Order { Location = "Client 1's Office", Description = "Eletric stove repairing.", Status = "Ongoing",
                        ClientId = 1, ServiceId = 2, TechnicianId = technician.Id },
                    new Order { Location = "Client 2's Office", Description = "Server systems maintenance.", Status = "Ongoing",
                        ClientId = 2, ServiceId = 5, TechnicianId = technician.Id },
                };

                foreach (var order in seedOrders)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == order.ClientId);
                    if (client == null)
                    {
                        throw new InvalidOperationException($"Could not find seed client.");
                    }

                    var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == order.ServiceId);
                    if (service == null)
                    {
                        throw new InvalidOperationException($"Could not find seed service."); 
                    }
                }

                _context.Orders.AddRange(seedOrders.AsEnumerable().Reverse());
                await _context.SaveChangesAsync();
            }
        }
    }
}
