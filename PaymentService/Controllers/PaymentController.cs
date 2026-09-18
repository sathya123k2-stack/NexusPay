using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;

namespace PaymentService.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService.Services.PaymentService _paymentService;

    public PaymentController(PaymentService.Services.PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Payment>>> GetAll()
    {
        var payments = await _paymentService.GetAll();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetById(int id)
    {
        var payment = await _paymentService.GetById(id);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<Payment>> Add(Payment payment)
    {
        var created = await _paymentService.Add(payment);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }
}