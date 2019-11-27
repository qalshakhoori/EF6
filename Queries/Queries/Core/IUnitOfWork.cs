using Queries.Core.Repositories;
using System;

namespace Queries.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Courses { get; }
        IAuhtorRepostiry Auhtors { get; }

        int Complete();
    }
}