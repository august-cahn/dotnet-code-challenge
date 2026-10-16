using CodeChallenge.Models;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation Create(Compensation compensation, Employee employee);
        Compensation GetByEmployeeId(string employeeId);
    }
}