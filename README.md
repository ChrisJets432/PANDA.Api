## PANDA.Api

### Introduction

This project was created for the spec of (Patient Appointment Network Data Application)[https://github.com/airelogic/tech-test-portal/blob/main/Patient-Appointment-Backend/README.md]. As requested I timeboxed this project and limited it to around five hours of coding and then around a hour of documentation.

It was produced in #C .Net Core Web API. Was created with using a MSSql database, but was coded with the ability to connect to a PostgresSql database also. For querying with data sources it was produced using Dapper and ability to use any ADO.Net standard connector. I did think on using EntityFramework with it being a modern alternative, but with having limited time to produce the project it was something I had not worked with for a few years.

In terms of unit tests I used NUnit and FluentAssertions. Playwright would of been a good alternative.

### Setting Up

I recommend using a local installation of Sql Server on windows or a Docker container if on Linux/MacOS, I have attached a dockerfile to setup a instance if required.
You will then be able to run the ./Scripts/Creation.sql file once connected.
One thing to note that the create scripts where created for a MS Sql Server.

If needed there is a docker file to initialise the web app for to access the endpoints. Additionally you can find the published version of the api documentation produced by SwaggerUI & published to Postman (here)[https://documenter.getpostman.com/view/42747312/2sAYdhLWDm].
You could alternatively (only on windows) and have IIS installed on your machine host the application via a IIS application instance.
Otherwise, you can just pull the file down and compile via Visual Studio or Jetbrains Rider.

There is a appsettings.json file attached with the document that allows you to change the database settings if your db is a different name, user or password.

### Consuming the Api

All responses uses a object of 'OperationResultsT', this will always return with the following fields:
- "success" is a boolean if it was successful or not
- 'statusCode' is a number value that mimics a standard Http Status Code.
- 'errors' which is a list of error messages, this will always set the 'success' to false if any.
- 'warnings' which is a list of warning but the operation could of still been successful.
- 'infos' which is a list of messages from the server, usually to confirm the process had completed successfully.

There is a optional property, 'data', which will contain the property that has been created or updated. This is handy as it will return the created identifier for appointments.

The CRUD endpoints use the standard GET, POST, PUT and DELETE request methods to get, add, update and delete properties.
Currently there is basic validation like is it a valid postcode structure, is NHS number checksum valid, does properties exist and do foreign key references exist.

### Checklist

[Y] Document how to get the API running\
This

[Y] Document how to interact with the API\
This and/or Postman documentation.

[Y] It should be possible to add patients to and remove them from the PANDA.\
Endpoints added.

[Y] It should be possible to check and update patient details in the PANDA.\
Endpoint & Services added.

[Y] It should be possible to add new appointments to the PANDA, and check and update appointment details.\
Endpoint & Services added.

[Y] The PANDA may need to be restarted for maintenance, and the data should be persisted.\
Database integration added.

[Y] The PANDA backend should communicate with the frontend via some sort of HTTP API.\
Endpoints added.

[Y] The PANDA API does not need to handle authentication because it is used within a trusted environment.\
Ignored authentication.

[Y] Errors should be reported to the user.\
Returns errors if something went wrong.

[N] Appointments can be cancelled, but cancelled appointments cannot be reinstated.\
Ran out of time to add the endpoint, but wouldn't be hard.

[Y] Appointments should be considered 'missed' if they are not set to 'attended' by the end of the appointment.\
Added to service logic.

[Y] Ensure that all NHS numbers are checksum validated.\
Added within in extensions.

[Y] Ensure that all postcodes can be coerced into the correct format.\
Added within in extensions.

[Y] The client has been burned by vendor lock-in in the past, and prefers working with smaller frameworks.\
Third-party extensions used: Dapper, FluentAssertions, NUnit, Microsoft.SqlClient, Npgsql.

[Y] The client is interested in branching out into foreign markets, it would be useful if error messages could be localised.\
Added a localisation .resx file, additionally added threading which is attached to a basic authentication handler where a claim is set from the https request header which allows for changing the current culture. Added out the box support for French (fr) but haven't translated any values yet.

[Y]The client would like to ensure that patient names can be represented correctly, in line with GDPR.\
Data is stored correctly within the database to allow for the corresponding unicode.

### Personal Considerations
1. Store "Departments" & "Clinicians" into their own database tables and store their identifiers against foreign records. This can lead on to reporting on metrics & uses, could later develop a dashboard/reporting service for charts, tables, csv downloads etc.

### Ideas for the 'Additional Considerations'
- "The client highly values automated tests, particularly those which ensure their business logic is implemented correctly."\
Started introducing NUnits but ran out of time to continue developing endpoint and service tests.
- "The client is in negotiation with several database vendors, and is interested in being database-agnostic if possible."\
As long as the database is a Sql-Based and ADO.net adapter it should be currently supported.
- "The client is somewhat concerned that missed appointments waste significant amounts of clinicians' time, and is interested in tracking the impact this has over time on a per-clinician and per-department basis."\
See Personal Condsiderations 1.
- "The PANDA currently doesn't contain much data about clinicians, but will eventually track data about the specific organisations they currently work for and where they work from."\
See Personal Condsiderations 1.