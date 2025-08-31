The premise of this application is simple, after applying to a job you can add the resumé and or cover letter for reference and track response times and get statistics.


This could easily be solved by a Back- and front-end solution with SQLite, however I wish to challenge myself and to reach more subjects, frameworks and methods for the experience. 


Currently containing:
- [x] Docker-compose orchestration with secrets for important parts.
- [x] Local MSSQL & PostgreSQL servers  (PostgreSQL is what I have worked with the least, so I chose that for login and learned OpenIddict doesn't support it. But I kept both for the experience.)
- [x] OpenIDConnect & OAuth2 with OpenIddict in AuthorizationServer.
- [x] Login/user creation with encryption, routed through the Authorization server, so that the Login functionality remains un-exposed.\
      (Tested through Postman)
- [ ] Blazor front-end:
- [ ] User creation/Login
- [ ] Upload PDF files to server
- [ ] Handling of applications stored with a User relation
- [ ] Preview applications in the app
- [ ] Show statistics such as how many has replied, response times
