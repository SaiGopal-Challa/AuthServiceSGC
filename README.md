**Login / Register Microservice**

Providing both API as service and WebUI as service.

More details on services provided:

**Service Type 1**: Login based on UID & password. [ Both API & UI option ]

**Service Type 2**: Login based on UID & password + CAPTCHA. [ UI option ] 

**Service Type 3**: Login based on UID & password + CAPTCHA + OTP (mobile/email/both). [ UI option ]
  
  Sub-types:
    OTP only on mobile.
    OTP only on email.
    OTP on both email and mobile.

**Service Type 4**: Login based on UID & password + OTP (mobile/email/both). [ Both API & UI option ]
  
  Sub-types:
    OTP only on mobile.
    OTP only on email.
    OTP on both email and mobile.

Using JWT to provide token, and also using same to provide session details.
Added BlackListing functionality to black list tokens immediatly post logout.
Using Redis to faster validations ( @conditions ). 
Using Custom built RateLimiting to control traffic .

Built using: .Net Core, Razor Pages, PostgreSQL, Redis, Bootstrap
