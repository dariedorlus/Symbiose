
using Symbiose.Mail.Data;
using Symbiose.Mail.Models;

namespace Symbiose.Mail.Repositories
{
    public interface IEmailRepository : IRepository<Email> { }
    public class EmailRepository: RepositoryBase<Email>, IEmailRepository
    {
        public EmailRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}