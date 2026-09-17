using AccountService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService.Services.AccountService _accountService;

    public AccountController(AccountService.Services.AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Account>>> GetAll()
    {
        var accounts = await _accountService.GetAll();
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetById(int id)
    {
        var account = await _accountService.GetById(id);

        if (account == null)
            return NotFound();

        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> Add(Account account)
    {
        var created = await _accountService.Add(account);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }
}
