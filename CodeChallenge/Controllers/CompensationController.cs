using System;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger<CompensationController> _logger;
        private readonly ICompensationService _compensationService;
        private readonly IEmployeeService _employeeService;

        public CompensationController(
            ILogger<CompensationController> logger,
            ICompensationService compensationService,
            IEmployeeService employeeService)
        {
            _logger = logger;
            _compensationService = compensationService;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Compensation compensation)
        {
            var employeeId = compensation?.Employee?.EmployeeId;
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return BadRequest();
            }

            // Resolve the submitted ID to a stored employee instead of trusting client-supplied employee data.
            var employee = _employeeService.GetById(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            var createdCompensation = _compensationService.Create(compensation, employee);
            if (createdCompensation == null)
            {
                return Conflict();
            }

            _logger.LogDebug("Created compensation for employee '{EmployeeId}'", employeeId);
            return CreatedAtAction(nameof(GetByEmployeeId), new { employeeId }, createdCompensation);
        }

        [HttpGet("{employeeId}")]
        public IActionResult GetByEmployeeId(string employeeId)
        {
            _logger.LogDebug("Received compensation request for employee '{EmployeeId}'", employeeId);

            var compensation = _compensationService.GetByEmployeeId(employeeId);
            if (compensation == null)
            {
                return NotFound();
            }

            return Ok(compensation);
        }
    }
}