using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly OuroborosContext _context;

    public ContactRepository(OuroborosContext context)
    {
        _context = context;
    }

    public async Task AddContactMessageAsync(ContactMessage contactMessage)
    {
        await _context.ContactMessages.AddAsync(contactMessage);
        await _context.SaveChangesAsync();
    }
}