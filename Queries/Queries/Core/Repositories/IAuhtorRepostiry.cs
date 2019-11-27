namespace Queries.Core.Repositories
{
    public interface IAuhtorRepostiry : IRepository<Author>
    {
        Author GetAuthorWithCourses(int id);
    }
}