using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using Queries.Persistence;

namespace Queries
{
    internal class Program
    {
        private static void Main()
        {
            PlutoContext context = new PlutoContext();

            // LINQ Syntax
            var query =
                from c in context.Courses
                where c.Author.Id == 1
                orderby c.Level descending, c.Name
                select c;

            // Projection
            var query0 =
                from c in context.Courses
                where c.Author.Id == 1
                orderby c.Level descending, c.Name
                select new { c.Name, Author = c.Author.Name };

            foreach (var course in query0)
            {
                Console.WriteLine(course.Name);
            }

            // Grouping
            var query1 =
                from c in context.Courses
                group c by c.Level into g
                select g;

            foreach (var group in query1)
            {
                Console.WriteLine(group.Key);

                foreach (var course in group)
                    Console.WriteLine("\t{0}", course.Name);
            }

            foreach (var group in query1)
            {
                Console.WriteLine("{0} ({1})", group.Key, group.Count());
            }

            // Joining - Inner Join
            var query2 =
                from c in context.Courses
                join a in context.Authors on c.AuthorId equals a.Id
                select new { c.Name };

            // Joining - Group Join

            var query3 =
                from a in context.Authors
                join c in context.Courses on a.Id equals c.AuthorId into g
                select new { AuthorName = a.Name, Courses = g.Count() };

            foreach (var x in query3)
                Console.WriteLine("{0} ({1})", x.AuthorName, x.Courses);

            // Joining - Cross Join

            var query4 =
                from a in context.Authors
                from c in context.Courses
                select new { AuthorName = a.Name, CourseName = c.Name };

            foreach (var x in query4)
                Console.WriteLine("{0} - {1}", x.AuthorName, x.CourseName);

            // LINQ Extension methods

            // Restrication
            context.Courses.Where(c => c.Level == 1);

            // Oredering
            context.Courses.Where(c => c.Level == 1)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Level);

            context.Courses.Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name)
                .ThenByDescending(c => c.Level);

            //Projection
            context.Courses.Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name)
                .ThenByDescending(c => c.Level)
                .Select(c => new { CourseName = c.Name, AuthorName = c.Author.Name });

            var tags = context.Courses.Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name)
                .ThenByDescending(c => c.Level)
                .SelectMany(c => c.Tags);

            foreach (var t in tags)
                Console.WriteLine(t.Name);

            // Set Operators
            context.Courses.Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name)
                .ThenByDescending(c => c.Level)
                .SelectMany(c => c.Tags)
                .Distinct();

            // Grouping
            var groups = context.Courses.GroupBy(c => c.Level);

            foreach (var group in groups)
            {
                Console.WriteLine("Key: " + group.Key);

                foreach (var course in group)
                    Console.WriteLine("\t" + course.Name);
            }

            // Joining

            // Inner Join - use when there is no relationship between objects
            // but you need to join them
            context.Courses.Join(
                context.Authors,
                c => c.AuthorId,
                a => a.Id,
                (course, author) => new
                {
                    CourseName = course.Name,
                    AuthorName = author.Name
                });

            // Group Join - use for left join in sql between two tables and then use aggregate functions like count , or group by
            context.Authors.GroupJoin(
                context.Courses,
                a => a.Id,
                c => c.AuthorId,
                (author, courses) => new
                {
                    Author = author,
                    Courses = courses
                });

            // Cross Join - return every combination of two lists
            context.Authors.
                SelectMany(
                    a => context.Courses,
                    (author, course) => new
                    {
                        AuthorName = author.Name,
                        CourseName = course.Name
                    }
                );

            // Partitioning - return a page of records, ex : return 10 courses per page
            context.Courses.Skip(10).Take(10); // skip first ten and get next 10 only

            // Element Operators - return a single or first object
            context.Courses.First();
            context.Courses.OrderBy(c => c.Level).First();
            context.Courses.OrderBy(c => c.Level).FirstOrDefault();
            context.Courses.OrderBy(c => c.Level).FirstOrDefault( c => c.FullPrice > 100);
            context.Courses.Single(c => c.Id == 1); // will through exception if ID 1 not found
            context.Courses.SingleOrDefault(c => c.Id == 1); // will through exception if condition return many records

            // Quantifying
            context.Courses.All(c => c.FullPrice > 10); // returns true if all courses FullPrice is greater than 10
            context.Courses.Any(c => c.Level == 1); // return true if we have at lease one course level equals 1

            // Aggregating          
            context.Courses.Count(); // return courses count
            context.Courses.Count(c => c.Level == 1); // return courses count of level one
            context.Courses.Max(c => c.FullPrice); // returns max price
            context.Courses.Min(c => c.FullPrice); // return min price
            context.Courses.Average(c => c.FullPrice); // return average price



            // IQueryable vs IEnumerable
            IQueryable<Course> courses1 = context.Courses;
            var filtered = courses1.Where(c => c.Level == 1); // filter will be part of our query to database
            foreach (var course in courses1)
                Console.WriteLine(course.Name);

            IEnumerable<Course> courses2 = context.Courses;
            var filtered1 = courses1.Where(c => c.Level == 1); // filter will not be included in query to database, filter will applied to results in memory

            // Lazy Loading
            var course0 = context.Courses.Single(c => c.Id == 2); // will load course without it's tags

            foreach (var tag in course0.Tags) // tags will be fetched from database here
                Console.WriteLine(tag.Name);


            //// n + 1 - problem
            //var coursesLazy = context.Courses.ToList(); // courses will be fetched here

            //foreach (var course in coursesLazy)
            //    Console.WriteLine("{0} by {1}", course.Name, course.Author.Name); // for each Author a query to the database will be made to fetch author

            //// Eager Loading all courses and thier author's

            //var coursesEager = context.Courses.Include(c => c.Author).ToList(); // Courses and thier authors are loaded here

            //foreach (var course in coursesEager)
            //    Console.WriteLine("{0} by {1}", course.Name, course.Author.Name);

            //// Explicit Loading
            //var authorExplicit = context.Authors.Single(a => a.Id == 1);

            //// MSDN 
            //context.Entry(authorExplicit).Collection(a => a.Courses).Load();

            //// Better way
            //context.Courses.Where(c => c.AuthorId == authorExplicit.Id).Load();
            //context.Courses.Where(c => c.AuthorId == authorExplicit.Id && c.FullPrice == 0).Load(); // loaded free courses only

            //// Load free courses for all authors
            //var authors = context.Courses.ToList();
            //var authorIds = authors.Select(a => a.Id);
            //context.Courses.Where(c => authorIds.Contains(c.AuthorId) && c.FullPrice == 0).Load();


            // Adding new objects
            var courseToAdd = new Course
            {
                Name = "New Course",
                Description = "New Desciption",
                FullPrice = 19.95f,
                Level = 1,
                AuthorId = 1
            };

            context.Courses.Add(courseToAdd);
            context.SaveChanges();

            // When we have object outside of our context
            var authorFromOutside = new Author { Id = 1, Name = "Qassim Alshakhoori" };
            context.Authors.Attach(authorFromOutside);

            courseToAdd = new Course
            {
                Name = "New Course",
                Description = "New Desciption",
                FullPrice = 19.95f,
                Level = 1,
                Author = authorFromOutside
            };

            context.Courses.Add(courseToAdd);
            context.SaveChanges();

            // Updating existing objects
            var courseToUpdate = context.Courses.Find(4); // Single(c => c.Id ==4)
            courseToUpdate.Name = "C# MOICT";
            courseToUpdate.AuthorId = 2;
            context.SaveChanges();

            // Removing objects
            // With Cascade delete enabled
            var courseToDelete = context.Courses.Find(6);
            context.Courses.Remove(courseToDelete);
            context.SaveChanges();

            // With Cascade delete disabled
            var authorToDelete = context.Authors.Include(a => a.Courses).Single(a => a.Id == 2);
            context.Courses.RemoveRange(authorToDelete.Courses);
            context.Authors.Remove(authorToDelete);
            context.SaveChanges();

            // Repository pattern
            using (var unitOfWork = new UnitOfWork(new PlutoContext()))
            {
                var course = unitOfWork.Courses.Get(1);

                var courses = unitOfWork.Courses.GetCoursesWithAuthors(1, 4);

                var author = unitOfWork.Auhtors.GetAuthorWithCourses(1);
                unitOfWork.Courses.RemoveRange(author.Courses);
                unitOfWork.Auhtors.Remove(author);
                unitOfWork.Complete();
            }
            
            context.Dispose();
        }
    }
}