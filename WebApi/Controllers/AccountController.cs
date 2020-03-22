using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using WebApi.Entities;
using WebApi.Models.AccountViewModels;
using WebApi.Models;
using WebApi.HttpProcess;
using WebApi.Authorize;
using WebApi.DataAccess.Base;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;//
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        PermissionRequirement _requirement;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            //IEmailSender emailSender,
            ILogger<AccountController> logger,
            PermissionRequirement requirement
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            _logger = logger;
            //_userManager.UserValidator = new UserValidator<TUser>(UserManager) { AllowOnlyAlphanumericUserNames = false }
            //userManager.AllowOnlyAlphanumericUserNames = false
            _requirement = requirement;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<Response> Register(RegisterViewModel model)
        {
            Response res = new Response();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,//UserName
                    Email = model.Email,
                    //FirstName = model.FirstName,
                    //LastName = model.LastName
                };
                try {

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        //return Ok();
                        res.success = 1;
                        res.resTxt = "success register";

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
                catch (System.Exception ex)
                {
                    res.success = 0;
                    res.resTxt = "err register";
                    //return res;

                    //eError = TError.TError_DataBase_Exception;
                }
            }
            return res;
            // If we got this far, something failed, redisplay form
            //return BadRequest();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<Response> Login([FromBody] LoginViewModel model)
        {
            Response res = new Response();
            if (ModelState.IsValid)
            {
                    //var user = _userManager.FindByEmailAsync(model.Email);
                var user = _userManager.FindByNameAsync(model.UserName);
                var result = await _signInManager.CheckPasswordSignInAsync(user.Result, model.Password, lockoutOnFailure: false);

                try
                {
                    //await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
                    //await _signInManager.SignInAsync(user.Result, isPersistent: true);

                }
                catch (System.Exception ex)
                {
                }

                //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                //identity.AddClaim(new Claim(ClaimTypes.Sid, userName));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                //identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
                    var now = DateTime.UtcNow;
                if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        var claims = new Claim[]
                            {
                            //new Claim(ClaimTypes.Email, model.Email),
                            new Claim(ClaimTypes.Name, user.Result.UserName),
                            new Claim(ClaimTypes.Expiration, now.Add(_requirement.Expiration).ToString()),

                            //new Claim(ClaimTypes.Name, user.Result.ID.ToString()),
                            new Claim("Id", user.Id.ToString()),
                            //new Claim("FirstName", user.Result.FirstName),
                            //new Claim("LastName", user.Result.LastName),
                            new Claim("Name", user.Result.UserName),
                            };
                    //var key = new SymmetricSecurityKey();
                    //var audienceConfig = Configuration.GetSection("Audience");
                    //var symmetricKeyAsBase64 = audienceConfig["Secret"];
                    var keyByteArray = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
                    var signingKey = new SymmetricSecurityKey(keyByteArray);
                    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                    //var t = new JwtSecurityToken(
                    //    claims: claims);

                    var token = new JwtSecurityToken(
                            claims: claims,
                            issuer: _requirement.Issuer,
                            audience: _requirement.Audience,
                            signingCredentials: _requirement.SigningCredentials,
                            expires: now.Add(_requirement.Expiration)
                            );
                        res.success = 1;
                        res.resTxt = "success login";
                        res.token = new JwtSecurityTokenHandler().WriteToken(token);

                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    try
                    {
                        //await HttpContext.AuthenticateAsync();
                        //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                        //await _signInManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                        await _signInManager.SignInAsync(user.Result, isPersistent: true);

                    }
                    catch (System.Exception ex)
                    {
                        //eError = TError.TError_DataBase_Exception;
                    }

                    return res;
                        //return Ok(new
                        //{
                        //    token = new JwtSecurityTokenHandler().WriteToken(token)
                        //});
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
                    res.resTxt = "fail login";
                    res.success = 0;
                    return res;
              
                
            }
            //res.success = 0;
            return res;
            
            //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //return BadRequest("Could not verify username and password");
        }

        // GET: api/User
        [HttpGet]
        [Route("get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "user1", "user2" };
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