using CustomerManagementPlatform.Data.Entities;
using CustomerManagementPlatform.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
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

            var user = await _userRepository.GetByEmailAsync("admin@mail");
            if (user == null)
            {
                user = new User
                {
                    FullName = "Admin",
                    Email = "admin@mail",
                    UserName = "admin@mail",
                };

                var result = await _userRepository.CreateAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not create seed user. Errors: {errors}");
                }

                var token = await _accountHelper.GenerateEmailConfirmationTokenAsync(user);
                await _accountHelper.ConfirmEmailAsync(user, token);
            }
        }
    }
}
