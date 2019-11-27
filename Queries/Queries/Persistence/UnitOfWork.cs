using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Queries.Core;
using Queries.Core.Repositories;
using Queries.Persistence.Repositories;

namespace Queries.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PlutoContext _context;
        public UnitOfWork(PlutoContext context)
        {
            _context = context;
            Courses = new CourseRespository(_context);
            Auhtors = new AuthorRespository(_context);
        }
        public ICourseRepository Courses { get; private set; }

        public IAuhtorRepostiry Auhtors { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
