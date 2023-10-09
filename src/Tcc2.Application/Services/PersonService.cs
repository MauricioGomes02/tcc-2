using System.Linq;
using Tcc2.Application.Exceptions;
using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Outputs;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Pagination;

namespace Tcc2.Application.Services;
public class PersonService : IPersonService
{
    private readonly Domain.Interfaces.Services.IPersonService _personService;
    private readonly IPersonValidationService _personValidatorService;
    private readonly IActivityValidationService _activityValidationService;

    public PersonService(
        Domain.Interfaces.Services.IPersonService personService,
        IPersonValidationService personValidatorService,
        IActivityValidationService activityValidationService)
    {
        _personService = personService;
        _personValidatorService = personValidatorService;
        _activityValidationService = activityValidationService;
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
            peopleOutput.ToList(),
            totalPages: storedPaginatedDomainPeople.TotalPages);
    }

    public async Task<PersonCompleteOutput> GetAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException($"The argument {nameof(id)} cannot be less than or equal to zero");
        }

        var storedDomainPerson = await _personService.GetAsync(id, cancellationToken).ConfigureAwait(false);
        return CompleteConvert(storedDomainPerson);
    }

    #endregion

    #region Address

    public async Task<AddressCompleteOutput> GetAddressAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException($"The argument {nameof(id)} cannot be less than or equal to zero");
        }

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
        ActivityInput? activityInput,
        CancellationToken cancellationToken)
    {
        var validation = _activityValidationService.Validate(activityInput);
        if (!validation.IsValid)
        {
            throw new ValidationModelException("The input model for adding a new activity is not valid", validation);
        }

        var domainActivity = Convert(activityInput!);
        var storedDomainActivity = await _personService
            .AddActivityAsync(id, domainActivity, cancellationToken)
            .ConfigureAwait(false);

        return CompleteConvert(storedDomainActivity);
    }

    public async Task<IReadOnlyCollection<ActivitySimpleOutput>> GetActivitiesAsync(
        long id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException($"The argument {nameof(id)} cannot be less than or equal to zero");
        }

        var activities = await _personService.GetActivitiesAsync(id, cancellationToken).ConfigureAwait(false);
        return activities.Select(SimpleConvert).ToList();
    }

    public async Task<ActivityCompleteOutput> GetActivityAsync(
        long id,
        long activityId,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException($"The argument {nameof(id)} cannot be less than or equal to zero");
        }

        if (activityId <= 0)
        {
            throw new ArgumentException($"The argument {nameof(activityId)} cannot be less than or equal to zero");
        }

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
        if (id <= 0)
        {
            throw new ArgumentException($"The argument {nameof(id)} cannot be less than or equal to zero");
        }

        if (activityId <= 0)
        {
            throw new ArgumentException($"The argument {nameof(activityId)} cannot be less than or equal to zero");
        }

        if (pageIndex < 0)
        {
            throw new ArgumentException($"The {nameof(pageIndex)} argument cannot be less than zero");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException($"The {nameof(pageSize)} argument cannot be less than or equal to zero");
        }

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
            activity.Description);

        foreach (var dayOfWeek in activity.DaysOfWeek)
        {
            convertedActivity.AddActivityDay(new ActivityDay(convertedActivity, new Day(dayOfWeek)));
        }

        return convertedActivity;
    }

    public static ActivitySimpleOutput SimpleConvert(Activity activity)
    {
        return new ActivitySimpleOutput
        {
            Id = (long)activity.Id!,
            State = activity.Address.State,
            City = activity.Address.City,
            Description = activity.Description
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
            DaysOfWeek = activity.ActivityDay.Select(x => (short)x.DayId!).ToList(),
            Start = activity.Start,
            End = activity.End
        };
    }

    #endregion
}
