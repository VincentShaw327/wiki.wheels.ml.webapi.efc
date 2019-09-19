using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using WebApi.Entities;
using WebApi.Models.AccountViewModels;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        //private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            //IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    return Ok();

                    // send email
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //_logger.LogInformation("User created a new account with password.");
                    //return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return BadRequest();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(model.Email);
                var result =
                    await _signInManager.CheckPasswordSignInAsync(user.Result, model.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim(ClaimTypes.Name, user.Result.UserName),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("FirstName", user.Result.FirstName),
                        new Claim("LastName", user.Result.LastName),
                    };
                    //var key = new SymmetricSecurityKey();
                    //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //var t = new JwtSecurityToken(
                    //    claims:claims)
                    var token = new JwtSecurityToken(
                        claims: claims,
                        issuer: "yourdomain.com",
                        audience: "yourdomain.com",
                        expires: DateTime.Now.AddMinutes(30)
                        //signingCredentials: creds
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    //return RedirectToAction(nameof(Lockout));
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return BadRequest("Invalid login attempt.");
                //    //return View(model);
                //}
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return BadRequest("Could not verify username and password");
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}