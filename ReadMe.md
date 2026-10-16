# Mindex Coding Challenge
## What's Provided
A simple [.Net 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) web application has been created and bootstrapped 
with data. The application contains information about all employees at a company. On application start-up, an in-memory 
database is bootstrapped with a serialized snapshot of the database. While the application runs, the data may be
accessed and mutated in the database without impacting the snapshot.

### How to Run
You can run this by executing `dotnet run` on the command line or in [Visual Studio Community Edition](https://www.visualstudio.com/downloads/).


### How to Use
The following endpoints are available to use:
```
* CREATE
    * HTTP Method: POST 
    * URL: localhost:8080/api/employee
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET 
    * URL: localhost:8080/api/employee/{id}
    * RESPONSE: Employee
* UPDATE
    * HTTP Method: PUT 
    * URL: localhost:8080/api/employee/{id}
    * PAYLOAD: Employee
    * RESPONSE: Employee
```
The Employee has a JSON schema of:
```json
{
  "type":"Employee",
  "properties": {
    "employeeId": {
      "type": "string"
    },
    "firstName": {
      "type": "string"
    },
    "lastName": {
          "type": "string"
    },
    "position": {
          "type": "string"
    },
    "department": {
          "type": "string"
    },
    "directReports": {
      "type": "array",
      "items" : "string"
    }
  }
}
```
For all endpoints that require an "id" in the URL, this is the "employeeId" field.

## What to Implement
Clone or download the repository, do not fork it.

### Task 1
Create a new type, ReportingStructure, that has two properties: employee and numberOfReports.

For the field "numberOfReports", this should equal the total number of reports under a given employee. The number of 
reports is determined to be the number of directReports for an employee and all of their direct reports. For example, 
given the following employee structure:
```
                    John Lennon
                /               \
         Paul McCartney         Ringo Starr
                               /        \
                          Pete Best     George Harrison
```
The numberOfReports for employee John Lennon (employeeId: 16a596ae-edd3-4847-99fe-c4518e82c86f) would be equal to 4. 

This new type should have a new REST endpoint created for it. This new endpoint should accept an employeeId and return 
the fully filled out ReportingStructure for the specified employeeId. The values should be computed on the fly and will 
not be persisted.

#### ReportingStructure Endpoint

* HTTP Method: GET
* URL: `localhost:8080/api/employee/{employeeId}/reporting-structure`
* RESPONSE: `ReportingStructure`, containing the requested `employee` and `numberOfReports` (the total number of direct and indirect reports)
* STATUS: `200 OK` when the employee exists; `404 Not Found` when the employeeId is unknown

### Task 2
Create a new type, Compensation. A Compensation has the following fields: employee, salary, and effectiveDate. Create 
two new Compensation REST endpoints. One to create and one to read by employeeId. These should persist and query the 
Compensation from the persistence layer.

#### Compensation Endpoints

* CREATE
  * HTTP Method: POST
  * URL: `localhost:8080/api/compensation`
  * PAYLOAD: `Compensation` with an existing employee, a positive salary, and an effective date
  * RESPONSE: Created `Compensation`
  * STATUS: `201 Created`; `400 Bad Request` for invalid data; `404 Not Found` when the employee does not exist; `409 Conflict` if that employee already has compensation

Example request:
```json
{
  "employee": {
    "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f"
  },
  "salary": 125000.50,
  "effectiveDate": "2026-10-16T00:00:00Z"
}
```

* READ
  * HTTP Method: GET
  * URL: `localhost:8080/api/compensation/{employeeId}`
  * RESPONSE: Persisted `Compensation` with its employee, salary, and effective date
  * STATUS: `200 OK`; `404 Not Found` when the employee has no compensation

Example response:
```json
{
  "employee": {
    "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f",
    "firstName": "John",
    "lastName": "Lennon",
    "position": "Development Manager",
    "department": "Engineering"
  },
  "salary": 125000.50,
  "effectiveDate": "2026-10-16T00:00:00Z"
}
```

## Delivery
Please upload your results to a publicly accessible Git repo. Free ones are provided by Github and Bitbucket.