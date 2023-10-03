using Tcc2.Domain.Entities;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Interfaces.Infrastructure.Repositories;

public interface IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
    Task<TAggregateRoot> AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken);
    Task<TAggregateRoot> GetAsync(long id, CancellationToken cancellationToken);
    Task<Paginated<TResult>> GetAsync<TResult>(
        Criteria<TAggregateRoot, TResult> criteria,
        CancellationToken cancellationToken) where TResult : IEntity;
    Task SaveAsync(CancellationToken cancellationToken);
}
