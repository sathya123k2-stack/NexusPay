using AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AuthenticationService.Services.AuthenticationService _authenticationService;

    public UserController(
        AuthenticationService.Services.AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _authenticationService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _authenticationService.GetById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Add(User user)
    {
        var created = await _authenticationService.Add(user);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
    var user = await _authenticationService.Login(
        request.Username,
        request.Password);

    if (user == null)
        return Unauthorized("Invalid username or password.");

    var response = new LoginResponse
    {
        Id = user.Id,
        CustomerId = user.CustomerId,
        Username = user.Username,
        Role = user.Role,
        Message = "Login successful"
    };

    return Ok(response);
    }

}