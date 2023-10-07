using Tcc2.Domain.Entities;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Interfaces.Infrastructure.Repositories;

public interface IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
    Task<TAggregateRoot> AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<TResult>> GetAsync<TResult>(
        Criteria<TAggregateRoot, TResult> criteria,
        CancellationToken cancellationToken);

    Task<Paginated<TResult>> GetPaginatedAsync<TResult>(
        Criteria<TAggregateRoot, TResult> criteria,
        CancellationToken cancellationToken);

    Task<TAggregateRoot> UpdateAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken);

    Task SaveAsync(CancellationToken cancellationToken);
}
