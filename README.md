README 
======
The solution contains 

Data project that 
1. defines a Repository Layer (EntityFramwork Context)
2. a Service Layer (application business logic)

XUnit Test project that
1. Defines a set of unit tests to validate the operation of the service layer 
2. Requires a reference to the SMS.Data project (dependency) - in particular the Service class and 
   models

MVC Web project that
1. Defines the user interface of our application - containing the ability to
    a. manage students (list, view, edit)
    b. manage student modules (add, remove) 
    c. add student tickets
    d. manage open tickets and provide a ticket response, closing the ticket
2. Handles general UX considerations 
    a. general validation when creating/modifying data
	b. provides breadcrumbs and alerts
3. Adds Cookie based authentication / authorization
    a. users can register or login 
    b. controllers are protected via authorization attributes 
    c. custom authorization tag helper used in razor views

