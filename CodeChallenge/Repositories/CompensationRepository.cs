using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;

        public CompensationRepository(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            // Include the related employee so reads return the complete compensation response.
            return _employeeContext.Compensations
                .Include(compensation => compensation.Employee)
                .SingleOrDefault(compensation => compensation.EmployeeId == employeeId);
        }

        public Compensation Add(Compensation compensation)
        {
            return _employeeContext.Compensations.Add(compensation).Entity;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}