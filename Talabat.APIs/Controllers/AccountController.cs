﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LogIn(LogInDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null)
                return Unauthorized(new ApiResponse(401));

            var logInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!logInResult.Succeeded)
                return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = "This will be token"
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber
            };

            var registerResult = await _userManager.CreateAsync(user, model.Password);
            if (!registerResult.Succeeded)
                return BadRequest(new ApiResponse(400));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = "This will be token"
            });
        }
    }
}
