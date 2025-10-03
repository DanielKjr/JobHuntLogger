The premise of this application is simple, after applying to a job you can add the resumé and or cover letter for reference and track response times and get statistics.


This could easily be solved by a Back- and front-end solution with SQLite, however I wish to spend my time while seeking a job productively, and applying more advanced subjects. 

Because this is a public repo I can't be keeping my information in the appsettings, so for that purpose I use an excluded folder with a json file, that supplies all the projects with their configurations.

After weeks of Blazor issues, that turned out to be a local installation or template corruption, I can finally actually work on this.

Currently containing:
- [x] Docker-compose orchestration with secrets for important parts.
- [x] Local MSSQL & PostgreSQL servers  (PostgreSQL is what I have worked with the least, so I chose that for login and learned OpenIddict doesn't support it. But I kept both for the experience.)
- [x] ~~OpenIDConnect & OAuth2 with OpenIddict in AuthorizationServer.~~
- [x] ~~Login/user creation with encryption, routed through the Authorization server, so that the Login functionality remains un-exposed.~~\
- [x] Login using Windows account or Microsoft login
- [x] Caching to ensure user session is carried over, instead of logging in every time.       
- [x] OpenIDConnect, OAuth2 through Entra





Blazor front-end:
- [ ] Upload PDF files to server
- [ ] Preview applications in the app
- [ ] Show statistics such as how many has replied, response times
- [x] Anti forgery (is on by default so easy check) 

ASP.NET Back-end:
- [ ] Use the Graph(entra) user Id as relation to applications.
- [ ] Store applications in EF database.
- [ ] Encrypt
- [x] Access token validation with Entra specified scope
