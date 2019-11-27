using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Queries.Core.Repositories;

namespace Queries.Persistence.Repositories
{
    public class CourseRespository : Repository<Course>, ICourseRepository
    {
        public CourseRespository(PlutoContext context)
            :base(context)
        {
        }

        public IEnumerable<Course> GetCoursesWithAuthors(int pageIndex, int pageSize = 10)
        {
            return PlutoContext.Courses
                .Include(c => c.Author)
                .OrderBy(c => c.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IEnumerable<Course> GetTopSellingCourses(int count)
        {
            return PlutoContext.Courses.OrderByDescending(c => c.FullPrice).Take(count).ToList();
        }

        public PlutoContext PlutoContext
        {
            get { return dbContext as PlutoContext; }
        }
    }
}
