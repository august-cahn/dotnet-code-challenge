using System;
using System.Net;
using System.Net.Http;
using System.Text;
using CodeChallenge.Models;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by MSTest.")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_ThenGetByEmployeeId_ReturnsPersistedCompensation()
        {
            var employee = CreateEmployee();
            var effectiveDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);
            var compensation = new Compensation
            {
                Employee = new Employee { EmployeeId = employee.EmployeeId },
                Salary = 125000.50m,
                EffectiveDate = effectiveDate
            };

            var createResponse = PostCompensation(compensation);

            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
            var createdCompensation = createResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(employee.EmployeeId, createdCompensation.Employee.EmployeeId);
            Assert.AreEqual(compensation.Salary, createdCompensation.Salary);
            Assert.AreEqual(effectiveDate, createdCompensation.EffectiveDate.Value);

            var getResponse = _httpClient.GetAsync($"api/compensation/{employee.EmployeeId}").Result;

            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var persistedCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(employee.EmployeeId, persistedCompensation.Employee.EmployeeId);
            Assert.AreEqual(employee.FirstName, persistedCompensation.Employee.FirstName);
            Assert.AreEqual(compensation.Salary, persistedCompensation.Salary);
            Assert.AreEqual(effectiveDate, persistedCompensation.EffectiveDate.Value);
        }

        [TestMethod]
        public void GetByEmployeeId_WithoutCompensation_ReturnsNotFound()
        {
            var employee = CreateEmployee();

            var response = _httpClient.GetAsync($"api/compensation/{employee.EmployeeId}").Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_ForUnknownEmployee_ReturnsNotFoundAndDoesNotPersist()
        {
            var employeeId = Guid.NewGuid().ToString();
            var compensation = new Compensation
            {
                Employee = new Employee { EmployeeId = employeeId },
                Salary = 80000m,
                EffectiveDate = DateTime.UtcNow
            };

            var createResponse = PostCompensation(compensation);
            var getResponse = _httpClient.GetAsync($"api/compensation/{employeeId}").Result;

            Assert.AreEqual(HttpStatusCode.NotFound, createResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_ForEmployeeWithExistingCompensation_ReturnsConflict()
        {
            var employee = CreateEmployee();
            var compensation = new Compensation
            {
                Employee = new Employee { EmployeeId = employee.EmployeeId },
                Salary = 95000m,
                EffectiveDate = DateTime.UtcNow
            };

            var firstResponse = PostCompensation(compensation);
            var duplicateResponse = PostCompensation(compensation);

            Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_WithoutEmployee_ReturnsBadRequest()
        {
            var compensation = new Compensation
            {
                Salary = 95000m,
                EffectiveDate = DateTime.UtcNow
            };

            var response = PostCompensation(compensation);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_WithNonPositiveSalary_ReturnsBadRequest()
        {
            var employee = CreateEmployee();
            var compensation = new Compensation
            {
                Employee = new Employee { EmployeeId = employee.EmployeeId },
                Salary = 0m,
                EffectiveDate = DateTime.UtcNow
            };

            var response = PostCompensation(compensation);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_WithoutEffectiveDate_ReturnsBadRequest()
        {
            var employee = CreateEmployee();
            var compensation = new Compensation
            {
                Employee = new Employee { EmployeeId = employee.EmployeeId },
                Salary = 95000m
            };

            var response = PostCompensation(compensation);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private static Employee CreateEmployee()
        {
            // Each test uses a fresh employee so shared in-memory state cannot affect other cases.
            var employee = new Employee
            {
                FirstName = "Compensation",
                LastName = "Test",
                Department = "Engineering",
                Position = "Developer"
            };
            var requestContent = new JsonSerialization().ToJson(employee);
            var response = _httpClient.PostAsync("api/employee",
                new StringContent(requestContent, Encoding.UTF8, "application/json")).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            return response.DeserializeContent<Employee>();
        }

        private static HttpResponseMessage PostCompensation(Compensation compensation)
        {
            var requestContent = new JsonSerialization().ToJson(compensation);
            return _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json")).Result;
        }
    }
}