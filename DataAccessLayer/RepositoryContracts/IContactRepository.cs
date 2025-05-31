using DataAccessLayer.Entities;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts;

public interface IContactRepository
{
    Task AddContactMessageAsync(ContactMessage contactMessage);
}