using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public ReportingStructure GetReportingStructure(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return null;
            }

            var employee = _employeeRepository.GetAll().SingleOrDefault(item => item.EmployeeId == id);
            if (employee == null)
            {
                return null;
            }

            return new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = CountReports(employee)
            };
        }

        // An iterative traversal avoids recursion limits and counts each employee ID once.
        private static int CountReports(Employee employee)
        {
            var visitedEmployeeIds = new HashSet<string>(StringComparer.Ordinal);
            if (!String.IsNullOrEmpty(employee.EmployeeId))
            {
                visitedEmployeeIds.Add(employee.EmployeeId);
            }

            var employeesToVisit = new Stack<Employee>();
            if (employee.DirectReports != null)
            {
                foreach (var directReport in employee.DirectReports)
                {
                    employeesToVisit.Push(directReport);
                }
            }

            var numberOfReports = 0;
            while (employeesToVisit.Count > 0)
            {
                var report = employeesToVisit.Pop();
                if (report == null || String.IsNullOrEmpty(report.EmployeeId) || !visitedEmployeeIds.Add(report.EmployeeId))
                {
                    continue;
                }

                numberOfReports++;
                if (report.DirectReports == null)
                {
                    continue;
                }

                foreach (var directReport in report.DirectReports)
                {
                    employeesToVisit.Push(directReport);
                }
            }

            return numberOfReports;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
    }
}
