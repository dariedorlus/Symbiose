using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symbiose.Mail.Data;
using Symbiose.Mail.Models;

namespace Symbiose.Mail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Emails1Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public Emails1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Emails1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Email>>> GetEmail()
        {
            return await _context.Emails.ToListAsync();
        }

        // GET: api/Emails1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Email>> GetEmail(string id)
        {
            var email = await _context.Emails.FindAsync(id);

            if (email == null)
            {
                return NotFound();
            }

            return email;
        }

        // PUT: api/Emails1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmail(string id, Email email)
        {
            if (id != email.Id)
            {
                return BadRequest();
            }

            _context.Entry(email).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Emails1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Email>> PostEmail(Email email)
        {
            _context.Emails.Add(email);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmailExists(email.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmail", new { id = email.Id }, email);
        }

        // DELETE: api/Emails1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmail(string id)
        {
            var email = await _context.Emails.FindAsync(id);
            if (email == null)
            {
                return NotFound();
            }

            _context.Emails.Remove(email);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmailExists(string id)
        {
            return _context.Emails.Any(e => e.Id == id);
        }
    }
}
