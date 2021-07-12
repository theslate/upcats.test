# What's UpCataas?
Just doing this programmer, what's up with you?

UpCat is a stateless .Net 5.0 AspNet Core Web Api service that's primary job is to flip images upside down from cataas.com.

For API details, see the [Swagger generated docs](swagger.html) on the deployed service at /swagger or the checked in html.

# Choices made - Features
## Authentication
The API only supports bearer JWT for authorization and authentication. OAuth is available via Swagger. Swagger handles the OAuth to JWT flow. Basic auth not supported because it's not secure. Authentication methods based on Asp.Net sessions are not supported, because this a stateless service.

## Image caching
Every image generated is cached using the original images's signature as a key. On a cache hit, upcats serves the cached images. Further investigation is required to configure the cache optimally for this scenario.

## User history
The service keeps a history of images flipped by each users via a /users/me. This allows users to get a list of cache keys so that the images can be retrieved via the /images/{cachekey} api. History is limited to the last 10 images.

## User persistance
The user store is a simple in memory database. The user store interface can easily be reimplemented with a persistant database.

# Choices made - Architecture
## AspNet core for authorization and authentication
Easy support for JWT tokens.

## Custom UserManager instead of AspNet Core Identity middleware
AspNet Core identity is too heavyweight for this service, where we only track very little about the users.

# Choices made - Dependencies
## .Net 5.0 AspNet Core
It's the latest tech stack, so I used that.

## ImageMagic.Net
Not going to reinvent the image processing wheel. ImageMagic.Net is easy to use, loads almost any image format, and allows us to flip transformations.

## MSpec for unit tests
I love using MSpec. Unfortunately, it doesn't follow idiomatic C#, so it looks a little weird a first glance.

Did not write unit tests for controllers, as their logic is very simple.

## Swashbuck to generate swagger docs
Testing an endpoint can be a chore. Swagger documents the API, creates auth tokens, and executes the interface.
