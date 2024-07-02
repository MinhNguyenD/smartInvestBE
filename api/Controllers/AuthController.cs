using api.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signinManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
    {
        _userManager = userManager;
        _signinManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newUser = new AppUser()
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };
            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (createUserResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
                if (roleResult.Succeeded)
                {
                    return Ok(new AuthUserDto()
                    {
                        Username = newUser.UserName,
                        Email = newUser.Email,
                        Token = _tokenService.GenerateToken(newUser)
                    });
                }
                else
                {
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                return StatusCode(500, createUserResult.Errors);
            }
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Signin([FromBody] SigninRequestDto signinRequestDto)
    {
        var user = await _userManager.FindByEmailAsync(signinRequestDto.Email);
        if (user == null)
        {
            return Unauthorized("Incorrect credentials. Email/password does not match");
        }
        var authResult = await _signinManager.CheckPasswordSignInAsync(user, signinRequestDto.Password, false);
        if (!authResult.Succeeded)
        {
            return Unauthorized("Incorrect credentials. Email/password does not match");
        }
        return Ok(new AuthUserDto
        {
            Username = user.UserName,
            Email = user.Email,
            Token = _tokenService.GenerateToken(user)
        });

    }
}
