using Microsoft.EntityFrameworkCore;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Pagination;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Repositories;

public abstract class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
    protected readonly TccContext _context;

    protected Repository(TccContext context)
    {
        _context = context;
    }

    public async Task<TAggregateRoot> AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        var entryEntity = await _context.AddAsync(aggregateRoot, cancellationToken).ConfigureAwait(false);
        return entryEntity.Entity;
    }

    public async Task<IReadOnlyCollection<TResult>> GetAsync<TResult>(
        Criteria<TAggregateRoot, TResult> criteria,
        CancellationToken cancellationToken)
    {
        var entities = await _context
            .Set<TAggregateRoot>()
            .Where(criteria.Predicate)
            .Select(criteria.Projection)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entities;
    }

    public async Task<Paginated<TResult>> GetPaginatedAsync<TResult>(
        Criteria<TAggregateRoot, TResult> criteria,
        CancellationToken cancellationToken)
    {
        if (criteria.PageIndex is null)
        {
            throw new ArgumentException($"The {nameof(criteria.PageIndex)} parameter cannot be null");
        }

        if (criteria.PageSize is null)
        {
            throw new ArgumentException($"The {nameof(criteria.PageSize)} parameter cannot be null");
        }

        var pageIndex = (int)criteria.PageIndex;
        var pageSize = (int)criteria.PageSize;

        var totalItems = await _context
            .Set<TAggregateRoot>()
            .CountAsync(criteria.Predicate, cancellationToken).ConfigureAwait(false);

        var entities = await _context
            .Set<TAggregateRoot>()
            .Where(criteria.Predicate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(criteria.Projection)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Paginated<TResult>(pageIndex, pageSize, entities, totalItems: totalItems);
    }

    public async Task<TAggregateRoot> UpdateAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        var entityEntry = await Task.Run(
            () => _context.Set<TAggregateRoot>().Update(aggregateRoot),
            cancellationToken).ConfigureAwait(false);

        return entityEntry.Entity;
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
