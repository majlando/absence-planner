# Tech stack requirements

## Systems Development
### Tests
- Unit tests
- Integration tests
- E2E tests

### GitHub
- GitHub Projects
- GitHub Issues
- Branching
- Pull Requests
- GitHub Actions

## Programming

The solution must be a distributed application consisting of 2 independent applications

#### React client application
- React Router
- Typescript
- swagger-typescript-api 

### .NET Web API
- Relational database using Linq2db
- Server side data validation
- Use some variant of OpenAPI / Swagger to provide living documentation for your API. (NSwag or other equivalent library)
- You must use GUIDs (and not numeric ID's like 1,2,3 ...)

## Security

#### Deployment
- The whole system should be able to run in a Docker container
#### Authentication & Deployment
- The system must have users
- They need to be able to authenticate
- Passwords are handled safely
- The system must have appropiate authorization polices determing who can access what.
#### Secrets
- No secrets in the git repository.

##  README
#### The README should at least consist of
- Security policies for who can access what (authorization)
- Current state of the project
- Testing strategy (TDD / TL)