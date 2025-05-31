using BusinessLogicLayer.Dtos.ContactDtos;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts;

public interface IContactService
{
    Task<(bool Success, string Message)> SendContactMessageAsync(ContactMessageCreateDto contactMessageDto);
}