using Microsoft.AspNetCore.Mvc;
using TransactionService.Models;

namespace TransactionService.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService.Services.TransactionService _transactionService;

    public TransactionController(TransactionService.Services.TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Transaction>>> GetAll()
    {
        var transactions = await _transactionService.GetAll();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetById(int id)
    {
        var transaction = await _transactionService.GetById(id);

        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> Add(Transaction transaction)
    {
        var created = await _transactionService.Add(transaction);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }
}
