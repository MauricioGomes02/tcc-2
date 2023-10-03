using Tcc2.Domain.Entities;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(TccContext context) : base(context)
    {
    }
}
