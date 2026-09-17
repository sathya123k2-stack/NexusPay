using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Models;

namespace TransactionService.Services;

public class TransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetAll()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction?> GetById(int id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task<Transaction> Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }
}
