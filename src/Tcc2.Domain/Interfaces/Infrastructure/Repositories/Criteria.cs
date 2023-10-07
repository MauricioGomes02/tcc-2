using System.Linq.Expressions;
using Tcc2.Domain.Entities;

namespace Tcc2.Domain.Interfaces.Infrastructure.Repositories;

public class Criteria<TAggregateRoot, TResult> where TAggregateRoot : IAggregateRoot
{
    public Criteria(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Expression<Func<TAggregateRoot, TResult>> projection,
        int? pageIndex = null,
        int? pageSize = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(projection);

        Predicate = predicate;
        Projection = projection;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public Expression<Func<TAggregateRoot, bool>> Predicate { get; private set; }
    public Expression<Func<TAggregateRoot, TResult>> Projection { get; private set; }
    public int? PageIndex { get; private set; }
    public int? PageSize { get; private set; }
}
