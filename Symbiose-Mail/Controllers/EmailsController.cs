using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Symbiose.Mail.Models;
using Symbiose.Mail.Services;


namespace Symbiose.Mail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private IEmailService emailService;

        public EmailsController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        // GET: api/Emails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Email>>> Get()
        {
            var res = await emailService.GetAllEmails();
            return Ok(res);
        }

        // GET api/Emails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Email>> GetEmail(string id)
        {
            Email email = await emailService.GetEmailById(id);
            
            return Ok(email);
        }

        // POST api/Emails
        [HttpPost]
        public async Task<ActionResult<Email>> PostEmail([FromBody] Email email)
        {

            if (email == null)
            {
                return BadRequest("The post body is empty.");
            }
            
            //2 Process request async
            Email response = await emailService.ProcessEmail(email);

            if (response == null)
            {
                return BadRequest("Missing or incorrect required field provided.");
            }

            //Return rich hyper media with url discovery
            return CreatedAtAction(nameof(GetEmail), new { id = email.Id }, email);
        }

        // PUT api/Emails/5
        [NonAction]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Email value)
        {

        }

        [NonAction]
        // DELETE api/Emails/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
