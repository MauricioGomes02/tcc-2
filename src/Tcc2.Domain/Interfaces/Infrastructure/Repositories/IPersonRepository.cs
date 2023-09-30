using System.Linq.Expressions;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Pagination;

namespace Tcc2.Domain.Interfaces.Infrastructure.Repositories;

public interface IPersonRepository
{
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken);
    Task<Paginated<Person>> GetAsync(CancellationToken cancellationToken, int pageIndex, int pageSize);
    Task<Person?> GetAsync(long id, CancellationToken cancellationToken);
    Task<Paginated<Person>> GetAsync(
        Expression<Func<Person, bool>> predicate,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
}
