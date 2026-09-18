using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Payment>> GetAll()
        => await _context.Payments.ToListAsync();

    public async Task<Payment?> GetById(int id)
        => await _context.Payments.FindAsync(id);

    public async Task<Payment> Add(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
}