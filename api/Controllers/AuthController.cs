using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
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
                    return Ok(new NewUserDto()
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


}
