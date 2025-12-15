#  Vehicle repair plan management

This project provides a Plan → Procedure → User assignment workflow using ASP.NET Core, EF Core, and a React frontend.
It supports creating plans, adding procedures, assigning multiple users to each procedure, and reliably retrieving grouped data for UI display.

This README provides:
- Features
- Tech stack
- API endpoints and it use
- Steps to run the app
  
---

## Features
- Create and manage Plans
- Add / remove Procedures from a Plan
- Assign multiple Users to a Procedure
- Unassign individual users or clear all assignments
- Displaying the data in UI

  ---
  
## Tech stack

- Backend
  - ASP.NET Core Web API
  - Entity Framework Core
  - SQLite
  - xUnit + FluentAssertions + Moq
- UI
  - React
  - ReactSelect
  - Fetch API
  
---

## Tech stack

- .NET Core (ASP.NET Core Web API)
- ReactJS (frontend)
- MS-SQL (database)

---

## API points
- GET /api/PlanProcedure/GetByPlan/{planId} - API used in UI to display data for each plan
- POST /api//plan - to create plan
- POST /api/plan/AddProcedureToPlan - add procedure to plan
- POST /api/PlanProcedure/assign-user - assign user to the procedure
- POST /api/PlanProcedure/unassign-user - un assign user from the prodecure
- POST /api/PlanProcedure/clear-users - clear all the user from the procedure
- POST /api/PlanProcedure/remove-procedure - remove the procedurer from the plan

Note: Swagger will be available in https://localhost:10011/index.html
---

## Steps to run the APP

1. Run the update-database command to make sure DB is up to date.
2. Run the API and d make it opens the swagger URL properly in https://localhost:10011/index.html
3. Run the npm i, if pacakges are not installed.
4. Run the react app using npm start and make sure all the procedures are loaded properly when start button is clicked.
5. 
---
