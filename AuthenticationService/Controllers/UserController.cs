using AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
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
    var response = await _authenticationService.Login(
        request.Username,
        request.Password);

    if (response == null)
        return Unauthorized("Invalid username or password.");

    return Ok(response);
    }
}