﻿using Blink.Models;
using Blink.Server.Models.DTO;
using Blink.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Blink.Server.Services.Implementations
{
    public class AccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountService> _logger;
        private readonly ITokenService _tokenService;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountService> logger,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<bool> RegisterAsync(RegisterDto model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} registered successfully.", model.Email);
                return true;
            }

            _logger.LogWarning("User registration failed for {Email}: {Errors}", model.Email, string.Join(", ", result.Errors));
            return false;
        }

        public async Task<UserDto> LoginAsync(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);

                    var token = _tokenService.CreateJWT(user);

                    _tokenService.SetJWT(token);

                    return new UserDto
                    {
                        Email = user.Email,
                        Id = user.Id
                    };
                }
            }

            _logger.LogWarning("Failed login attempt for {Email}.", model.Email);
            return null;
        }
    }
}
