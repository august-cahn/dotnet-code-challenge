using CodeChallenge.Models;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;

        public CompensationService(ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
        }

        public Compensation Create(Compensation compensation, Employee employee)
        {
            // EmployeeId is the key, so reject a second compensation for the same employee.
            if (compensation == null || employee == null ||
                _compensationRepository.GetByEmployeeId(employee.EmployeeId) != null)
            {
                return null;
            }

            compensation.Employee = employee;
            compensation.EmployeeId = employee.EmployeeId;
            _compensationRepository.Add(compensation);
            _compensationRepository.SaveAsync().Wait();

            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return null;
            }

            return _compensationRepository.GetByEmployeeId(employeeId);
        }
    }
}