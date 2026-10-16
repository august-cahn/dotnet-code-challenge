# Implementation Plan

## Goal
Complete both tasks in the repository's .NET 6 employee API and verify and document the behavior. Target effort: approximately 1-2 hours.

## 1. ReportingStructure (about 30 minutes)

- Add a `ReportingStructure` model with `employee` and `numberOfReports` properties.
- Extend the employee service to produce a reporting structure for an employee ID. Count all direct reports and their descendants from the employee data; compute the result on demand and do not persist it.
- Add a GET endpoint, proposed as `GET /api/employee/{id}/reporting-structure`, following the existing controller/service pattern.
- Return `404 Not Found` when the requested employee does not exist. Ensure traversal cannot loop forever if reporting data contains a cycle; settle how missing referenced employees are handled and document that behavior.

### Tests

- Add integration tests alongside the existing employee API tests for the seeded hierarchy: John Lennon has 4 total reports and Ringo Starr has 2, proving indirect reports are included.
- Verify a leaf employee such as Paul McCartney returns a count of 0 and that the response includes the requested employee.
- Verify an unknown employee ID returns `404 Not Found`.
- If cycle and missing-child behavior is implemented as specified above, add focused tests for those cases so traversal terminates and the chosen missing-child behavior stays explicit.

## 2. Compensation (about 35 minutes)

- Add a `Compensation` model with `employee`, `salary`, and `effectiveDate`.
- Add compensation storage to the EF Core context and extend repository/service interfaces and implementations to create and query compensation using the existing in-memory persistence setup.
- Add endpoints, proposed as `POST /api/compensation` to create and `GET /api/compensation/{employeeId}` to read.
- Define duplicate-create behavior and the response for an employee with no compensation before implementing; make the behavior deterministic and cover it with tests.
- On create, require the compensation to reference an existing employee. Return `404 Not Found` if that employee ID does not resolve; do not persist a compensation with a null or dangling employee. Return appropriate client errors for malformed or missing required fields.

### Tests

- Add integration tests that POST a compensation and then GET it by employee ID; verify salary, effective date, and employee data in the response.
- Verify the chosen duplicate-create behavior and that an employee without compensation receives the documented not-found response.
- Verify an unknown employee ID on create returns `404 Not Found` and does not create a compensation record.
- Verify malformed or missing required request data returns the expected client error and does not create a readable compensation record.
- Keep tests independent: use unique employee IDs or clean test data so compensation state from one test cannot affect another.

## 3. Verification and Documentation (about 20 minutes)

- [x] Run `dotnet test CodeChallenge.sln` and resolve failures related to the implementation.
- [x] Start the application and smoke-test both tasks' endpoints using the seeded data and a compensation create/read request.
- [x] Add concise code documentation for the report-counting approach and compensation persistence decisions. Update the README with both new endpoints, request payloads, and representative responses/status codes.
- [x] Review the final diff and confirm generated .NET output remains ignored by Git.

### Verification Record (2026-10-16)

- `dotnet test CodeChallenge.sln`: 15 passed, 0 failed.
- Live HTTP smoke checks: compensation POST returned `201 Created`; GET returned the saved salary and employee; John Lennon's ReportingStructure returned 4 reports.
- `git diff --check` and editor diagnostics passed. `bin/` and `obj/` outputs match ignore rules; seed data and app settings remain trackable.
- The build emits `NETSDK1138` because the project targets out-of-support .NET 6.

## 4. Transcript and Final Review (about 10 minutes)

- [ ] Download the AI conversation transcript at the end of the work and include it with the submission as the record of AI usage. No separate in-repository activity log is needed.
- [x] Review the implementation, documentation, tests, and repository status for submission readiness. Public GitHub setup and publication are already handled.

## Completion Checklist

- [x] ReportingStructure endpoint returns the employee and correct recursive report count.
- [x] Compensation create/read endpoints persist and return the requested data.
- [x] Integration tests cover success, empty/invalid cases, and not-found behavior.
- [x] `dotnet test CodeChallenge.sln` and endpoint smoke checks pass.
- [x] Code and API documentation are updated.
- [ ] Download and include the AI conversation transcript for submission.
- [x] Generated files are ignored and the completed work is ready for review.
