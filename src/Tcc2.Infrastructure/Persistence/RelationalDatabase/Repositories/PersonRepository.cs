using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Pagination;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly TccContext _context;

    public PersonRepository(TccContext context)
    {
        _context = context;
    }

    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken)
    {
        var storedPerson = await _context.AddAsync(person, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return storedPerson.Entity;
    }

    public async Task<Paginated<Person>> GetAsync(CancellationToken cancellationToken, int pageIndex, int pageSize)
    {
        var totalItems = await _context.People
            .CountAsync(cancellationToken).ConfigureAwait(false);

        var storedPeople = await _context.People
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Paginated<Person>(pageIndex, pageSize, totalItems, storedPeople);
    }

    public Task<Person?> GetAsync(long id, CancellationToken cancellationToken)
    {
        return _context.People.Include(x => x.Address).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Paginated<Person>> GetAsync(
        Expression<Func<Person, bool>> predicate,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalItems = await _context.People
            .Where(predicate)
            .CountAsync(cancellationToken).ConfigureAwait(false);

        var storedPeople = await _context.People
            .Where(predicate)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Paginated<Person>(pageIndex, pageSize, totalItems, storedPeople);
    }
}
