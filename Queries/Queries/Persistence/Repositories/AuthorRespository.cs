using Queries.Core.Repositories;
using System.Data.Entity;
using System.Linq;

namespace Queries.Persistence.Repositories
{
    public class AuthorRespository : Repository<Author>, IAuhtorRepostiry
    {
        public AuthorRespository(PlutoContext context) : base(context)
        {
        }

        public Author GetAuthorWithCourses(int id)
        {
            return PlutoContext.Authors.Include(a => a.Courses).SingleOrDefault(a => a.Id == id);
        }

        public PlutoContext PlutoContext
        {
            get { return dbContext as PlutoContext; }
        }
    }
}