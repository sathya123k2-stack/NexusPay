using AccountService.Data;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Services;

public class AccountService
{
    private readonly ApplicationDbContext _context;

    public AccountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Account>> GetAll()
    {
        return await _context.Accounts.ToListAsync();
    }

    public async Task<Account?> GetById(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> Add(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }
}
