using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts
{
	public interface ICustomBraceletRepository
	{
		Task<CustomBracelet?> GetCustomBraceletByIdAsync(Guid? customBraceletId);
		Task<bool> DeleteCustomBracelet(Guid customBraceletId);
	}
}
