using Microsoft.EntityFrameworkCore;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(TccContext context) : base(context)
    {
    }

    public async Task<IReadOnlyCollection<Day>> GetDaysAsync(CancellationToken cancellationToken)
    {
        var days = await _context.Days.ToListAsync(cancellationToken).ConfigureAwait(false);
        return days;
    }
}
