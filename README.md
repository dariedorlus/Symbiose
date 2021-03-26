# The Awesome Mail Api

### How to install

1. Download and install the .Net 5.0 SDK for your OS following the instructions here (https://dotnet.microsoft.com/download)
2. Clone this repo (https://github.com/dariedorlus/Symbiose.git)
3. Open a terminal window where the project is cloned  
    a. Navigate to Symbiose-Mail  
    b. Run the command `dotnet watch run`  
    c. Alternatively you can run test with the command `dotnet test`

The application should launch on (https://localhost:5001/swagger/).  
The api endpoints are located https://localhost:5001/api/Emails.

- **Note:**  
    Api keys for Mailgun and Sendgrid, need to be added the `Symbiose-mail/appsettings.json`. The `Domain` is also needed for Mailgun.

        {
            ...
            "Mailgun": {
                "ApiKey": "[Insert-Key-Here]",
                "Domain": "[Insert-domain-here]",
                "Enabled": true 
                ...
            },
            "Sendgrid": {
                "ApiKey": "[Insert-Key-Here]",
                "ApiBaseUrl": "https://api.sendgrid.com/v3/mail/"
                ...
            }
        }

- Maillgun is `enabled` by default. To use sendgrid, set the `Enabled` property of the  Mailgun setting to false (as seen above).  
Alternatively, the `V2` branch uses both mail delivery systems automatically.


### Language, Framework, and Libraries

- **Language:**  
    The language I chose for this exercise is C# (version 9.0). I primarily went with C# for familiarity. Being object oriented, it allowed me to showcase some object oriented principles as specified by the requirements.  

- **Framework:**  
    I went to .Net 5.0 as a framework because it boasts the latest security, and higher performance than some of its peers. Part of the reason I chose C# is because of the level of maturity of the fleet  of .Net frameworks. A lot of the heavy lifting in creating api is already done, making producing code much faster.

- **Libraries:**  
    1. EntityFrameWork Core (InMemoryDatabase) - I thought it would be a good idea to persist the email post requests. Should some of the mail delivery services be down or an email not sent for any reason, the service would always try again later. Moreover, it would be good to store some telemetry to understand how the api is being used.  

    2. Swashbuckle - Because swagger is awesome, this auto generates swagger docs with swag.

    3. AspNet TestHost - To be able to run system integration test

    4. FakeItEasy and FluentAssertion to mock and make tests read like a sentence. The tests then become a living document.  

### Tradeoffs, Improvements, and more

- **Tradeoffs:**  
    1. For serializing DTO into json, my goto lib is NewtonSoft.Json (Json.Net). It's such a convenient and powerful lib that microsoft used to ship it with .NET Core up until the current version. While Json.Net is convenient, the .NET team created a lighter and faster serializer. Though I lost some of the features, and I had to do a little manual work, the performance is 2x which can be crucial at scale.  

    2. I use a service and repository layer. It would have been much faster given the scope of the project to let the controller access data context and send these emails directly. Having everything segregated that much longer to develop, but makes it much better to maintain and change things down the line.  

- **Improvements:**  
    1. I would add both authentication and rate limits on the system. Depending on the load this service is expected to handle, I would have the service return 203 Accepted (as opposed to 201) with location discovery (currently implemented). That way users can check the status of their post.

    2. Persist all post requests, and create a cron job to send the ones that failed.

    3. Allow users to specify multiple recipients

    4. For extreme load, make this system event-driven. The api endpoint would then raise an event and return right away.

    5. Retry mechanism for when mailgun and/or sendgrid  per min/sec rate is reached. Currently in the V2 Branch sendgrid will be used if mailgun fails.

- **And More:**  
    The test coverage of the system is currently at 77%. I would love to bring it to 77.2% ;)
