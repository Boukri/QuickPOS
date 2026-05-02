using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace QuickPOS.Data;

/// <summary>
/// Generic repository using Ardalis.Specification's built-in RepositoryBase.
/// </summary>
public class EfRepository<T> : RepositoryBase<T> where T : class
{
    public EfRepository(QuickPosDbContext dbContext) : base(dbContext)
    {
    }
}
