using Tcc2.Application.Exceptions;
using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Outputs;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Pagination;

namespace Tcc2.Application.Services;
public class PersonService : IPersonService
{
    private readonly Domain.Interfaces.Services.IPersonService _personService;
    private readonly IPersonValidationService _personValidatorService;

    public PersonService(
        Domain.Interfaces.Services.IPersonService personService,
        IPersonValidationService personValidatorService)
    {
        _personService = personService;
        _personValidatorService = personValidatorService;
    }

    #region Person

    public async Task<PersonCompleteOutput> AddAsync(PersonInput person, CancellationToken cancellationToken)
    {
        var validation = _personValidatorService.Validate(person);
        if (!validation.IsValid)
        {
            throw new ValidationModelException("The input model for adding a new person is not valid", validation);
        }

        var domainPerson = Convert(person);
        var storedDomainPerson = await _personService.AddAsync(domainPerson, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainPerson);
    }

    public async Task<Paginated<PersonSimpleOutput>> GetAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentException($"The {nameof(pageIndex)} argument cannot be less than zero");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException($"The {nameof(pageSize)} argument cannot be less than or equal to zero");
        }

        var storedPaginatedDomainPeople = await _personService
            .GetAsync(pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var storedDomainPeople = storedPaginatedDomainPeople.Items;
        var peopleOutput = storedDomainPeople.Select(SimpleConvert);
        return new Paginated<PersonSimpleOutput>(
            storedPaginatedDomainPeople.PageIndex,
            storedPaginatedDomainPeople.PageSize,
            storedPaginatedDomainPeople.TotalPages,
            peopleOutput.ToList());
    }

    public async Task<PersonCompleteOutput> GetAsync(long id, CancellationToken cancellationToken)
    {
        var storedDomainPerson = await GetDomainPersonAsync(id, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainPerson);
    }

    public async Task<Paginated<PersonSimpleOutput>> GetNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentException($"The {nameof(pageIndex)} argument cannot be less than zero");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException($"The {nameof(pageSize)} argument cannot be less than or equal to zero");
        }

        if (radius <= 0)
        {
            throw new ArgumentException(
                $"The {nameof(radius)} argument cannot be less than or equal to zero");
        }

        var storedPaginatedDomainPeople = await _personService
            .GetNearbyPeopleAsync(id, radius, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var peopleOutput = storedPaginatedDomainPeople.Items.Select(SimpleConvert);
        return new Paginated<PersonSimpleOutput>(
            storedPaginatedDomainPeople.PageIndex,
            storedPaginatedDomainPeople.PageSize,
            storedPaginatedDomainPeople.TotalPages,
            peopleOutput.ToList());
    }

    #endregion

    #region Address

    public async Task<AddressCompleteOutput> GetAddressAsync(long id, CancellationToken cancellationToken)
    {
        var storedDomainPerson = await GetDomainPersonAsync(id, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainPerson.Address);
    }

    #endregion

    private Task<Person> GetDomainPersonAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException($"The argument {nameof(id)} cannot be less than or equal to zero");
        }

        return _personService.GetAsync(id, cancellationToken);
    }

    #region Convert Person

    private static Person Convert(PersonInput person)
    {
        return new Person(person.Name, Convert(person.Address));
    }

    private static PersonSimpleOutput SimpleConvert(Person person)
    {
        return new PersonSimpleOutput
        {
            Id = (long)person.Id!,
            Name = person.Name
        };
    }

    private static PersonCompleteOutput CompleteConvert(Person person)
    {
        return new PersonCompleteOutput
        {
            Id = (long)person.Id!,
            Name = person.Name,
            Address = SimpleConvert(person.Address)
        };
    }

    #endregion

    #region Convert Address

    private static Address Convert(AddressInput address)
    {
        return new Address(
            address.Country,
            address.State,
            address.City,
            address.Neighborhood,
            address.Street,
            address.Number,
            address.PostalCode);
    }

    private static AddressSimpleOutput SimpleConvert(Address address)
    {
        return new AddressSimpleOutput
        {
            Id = (long)address.Id!,
            State = address.State,
            City = address.City
        };
    }

    private static AddressCompleteOutput CompleteConvert(Address address)
    {
        return new AddressCompleteOutput
        {
            Id = (long)address.Id!,
            Country = address.Country,
            State = address.State,
            City = address.City,
            Neighborhood = address.Neighborhood,
            Street = address.Street,
            Number = address.Number,
            PostalCode = address.PostalCode,
            Latitude = address.GeographicCoordinate!.Latitude,
            Longitude = address.GeographicCoordinate!.Longitude
        };
    }

    #endregion
}
