using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Outputs;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Exceptions;
using Tcc2.Domain.Models.Pagination;
using Tcc2.Domain.Models.Validation;

namespace Tcc2.Application.Services;
public class PersonService : IPersonService
{
    private readonly Domain.Interfaces.Services.IPersonService _personService;

    public PersonService(Domain.Interfaces.Services.IPersonService personService)
    {
        _personService = personService;
    }

    #region Person

    public async Task<PersonCompleteOutput> AddAsync(PersonInput person, CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();

        if (person is null)
        {
            validationResult.Add(
                nameof(person),
                nameof(PersonService),
                "Cannot be null");
        }

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var domainPerson = Convert(person!);
        var storedDomainPerson = await _personService.AddAsync(domainPerson, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainPerson);
    }

    public async Task<Paginated<PersonSimpleOutput>> GetAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var storedPaginatedDomainPeople = await _personService
            .GetAsync(pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var storedDomainPeople = storedPaginatedDomainPeople.Items;
        var peopleOutput = storedDomainPeople.Select(SimpleConvert);
        return new Paginated<PersonSimpleOutput>(
            storedPaginatedDomainPeople.PageIndex,
            storedPaginatedDomainPeople.PageSize,
            peopleOutput.ToList(),
            totalPages: storedPaginatedDomainPeople.TotalPages);
    }

    public async Task<PersonCompleteOutput> GetAsync(long id, CancellationToken cancellationToken)
    {
        var storedDomainPerson = await _personService.GetAsync(id, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainPerson);
    }

    #endregion

    #region Address

    public async Task<AddressCompleteOutput> GetAddressAsync(long id, CancellationToken cancellationToken)
    {
        var storedDomainAddress = await _personService.GetAddressAsync(id, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainAddress);
    }

    public async Task<Paginated<PersonSimpleOutput>> GetGeographicallyNearbyPeopleAsync(
        long id,
        double radius,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var storedPaginatedDomainPeople = await _personService
            .GetGeographicallyNearbyPeopleAsync(id, radius, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var peopleOutput = storedPaginatedDomainPeople.Items.Select(SimpleConvert);
        return new Paginated<PersonSimpleOutput>(
            storedPaginatedDomainPeople.PageIndex,
            storedPaginatedDomainPeople.PageSize,
            peopleOutput.ToList(),
            totalPages: storedPaginatedDomainPeople.TotalPages);
    }

    #endregion

    #region Activities

    public async Task<ActivityCompleteOutput> AddActivityAsync(
        long id,
        ActivityInput? activity,
        CancellationToken cancellationToken)
    {
        var validationResult = new ValidationResult();

        if (activity is null)
        {
            validationResult.Add(
                nameof(activity),
                nameof(PersonService),
                "Cannot be null");
        }

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Invalid input", validationResult);
        }

        var domainActivity = Convert(activity!);
        var storedDomainActivity = await _personService
            .AddActivityAsync(id, domainActivity, cancellationToken)
            .ConfigureAwait(false);

        return CompleteConvert(storedDomainActivity);
    }

    public async Task<IReadOnlyCollection<ActivitySimpleOutput>> GetActivitiesAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var activities = await _personService.GetActivitiesAsync(id, cancellationToken).ConfigureAwait(false);
        return activities.Select(SimpleConvert).ToList();
    }

    public async Task<ActivityCompleteOutput> GetActivityAsync(
        long id,
        long activityId,
        CancellationToken cancellationToken)
    {
        var activity = await _personService.GetActivityAsync(id, activityId, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(activity);
    }

    public async Task<Paginated<PersonSimpleOutput>> GetFunctionallyNearbyPeopleAsync(
        long id,
        long activityId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var storedPaginatedDomainPeople = await _personService
            .GetFunctionallyNearbyPeopleAsync(id, activityId, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var peopleOutput = storedPaginatedDomainPeople.Items.Select(SimpleConvert);
        return new Paginated<PersonSimpleOutput>(
            storedPaginatedDomainPeople.PageIndex,
            storedPaginatedDomainPeople.PageSize,
            peopleOutput.ToList(),
            totalPages: storedPaginatedDomainPeople.TotalPages);
    }

    #endregion

    #region Convert Person

    private static Person Convert(PersonInput person)
    {
        return new Person(person.Name, ConvertToComposite(person.Address));
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
            Address = SimpleConvert(person.Address),
            Activities = person.Activities.Select(SimpleConvert)
        };
    }

    #endregion

    #region Convert Address

    private static CompositeAddress ConvertToComposite(AddressInput address)
    {
        return new CompositeAddress(
            address.Country,
            address.State,
            address.City,
            address.Neighborhood,
            address.Street,
            address.Number,
            address.PostalCode);
    }

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

    private static AddressSimpleOutput SimpleConvert(CompositeAddress address)
    {
        return new AddressSimpleOutput
        {
            Id = (long)address.Id!,
            State = address.State,
            City = address.City
        };
    }

    private static AddressCompleteOutput CompleteConvert(CompositeAddress address)
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

    #region Convert Activities

    private static Activity Convert(ActivityInput activity)
    {
        var convertedActivity = new Activity(
            Convert(activity.Address),
            activity.Start,
            activity.End,
            activity.DaysOfWeek,
            activity.Description);

        return convertedActivity;
    }

    public static ActivitySimpleOutput SimpleConvert(Activity activity)
    {
        return new ActivitySimpleOutput
        {
            Id = (long)activity.Id!,
            State = activity.Address.State,
            City = activity.Address.City,
            Description = activity.Description,
            DaysOfWeek = activity.ActivityDay.Select(x => x.Day.Id).ToList(),
            Start = activity.Start,
            End = activity.End,
        };
    }

    public static ActivityCompleteOutput CompleteConvert(Activity activity)
    {
        return new ActivityCompleteOutput
        {
            Id = (long)activity.Id!,
            State = activity.Address.State,
            City = activity.Address.City,
            Description = activity.Description,
            Country = activity.Address.Country,
            Street = activity.Address.Street,
            Number = activity.Address.Number,
            PostalCode = activity.Address.PostalCode,
            DaysOfWeek = activity.ActivityDay.Select(x => x.DayId).ToList(),
            Start = activity.Start,
            End = activity.End
        };
    }

    #endregion
}
