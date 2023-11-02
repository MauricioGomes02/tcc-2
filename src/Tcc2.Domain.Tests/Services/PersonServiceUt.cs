using Moq;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Interfaces.Infrastructure.Services;
using Tcc2.Domain.Interfaces.Services;
using Tcc2.Domain.Models.Pagination;
using Tcc2.Domain.Services;

namespace Tcc2.Domain.Tests.Services;

public class PersonServiceUt
{
    private readonly Mock<IPersonRepository> _personRepository;
    private readonly Mock<IGeographicCoordinateService> _geographicCoordinateService;
    private readonly Mock<IGeographicProximityService> _geographicProximityService;
    private readonly Mock<IFunctionalProximityService> _functionalProximityService;
    private readonly PersonService _sut;
    private readonly Person _validPerson;

    public PersonServiceUt()
    {
        _geographicCoordinateService = new Mock<IGeographicCoordinateService>();
        _geographicProximityService = new Mock<IGeographicProximityService>();
        _functionalProximityService = new Mock<IFunctionalProximityService>();
        _personRepository = new Mock<IPersonRepository>();
        _sut = new PersonService(
            _geographicCoordinateService.Object,
            _geographicProximityService.Object,
            _functionalProximityService.Object,
            _personRepository.Object);

        var address = new CompositeAddress(
           "Brasil",
           "Rio Grande do Sul",
           "Canoas",
           "Centro",
           "Avenida Victor Barreto",
           2288,
           "92010000");
        _validPerson = new Person("Maurício", address);
    }

    [Fact]
    public async Task GivenThatCreatePerson_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Arrange
        var person = new Person();

        // Act
        var act = () => _sut.AddAsync(person, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task GivenThatCreatePerson_WhenNotFindTheGeographicCoordinates_ShouldThrowException()
    {
        // Arrange
        _geographicCoordinateService
            .Setup(x => x.GetAsync(It.IsAny<CompositeAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as GeographicCoordinate);

        // Act
        var act = () => _sut.AddAsync(_validPerson, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValueObjectNotFoundException>(act);
        _geographicCoordinateService.Verify(
            x => x.GetAsync(_validPerson.Address, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatCreatePerson_WhenFindTheGeographicCoordinates_ShouldAdd()
    {
        // Arrange
        var latitude = -29.917270584965024;
        var longitude = -51.181264857578256;
        var geographicCoordinate = new GeographicCoordinate(latitude, longitude);

        _geographicCoordinateService
            .Setup(x => x.GetAsync(It.IsAny<CompositeAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(geographicCoordinate);

        _personRepository
            .Setup(x => x.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_validPerson);

        // Act
        var storedPerson = await _sut.AddAsync(_validPerson, CancellationToken.None);

        // Assert
        Assert.NotNull(storedPerson);
        Assert.Same(storedPerson, _validPerson);

        _geographicCoordinateService.Verify(
            x => x.GetAsync(_validPerson.Address, CancellationToken.None),
            Times.Once);

        _personRepository.Verify(x => x.AddAsync(_validPerson, CancellationToken.None), Times.Once);
        _personRepository.Verify(x => x.SaveAsync(CancellationToken.None), Times.Once);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(-1, -1)]
    public async Task GivenThatGetPaginatedPeople_WhenInsertingInvalidData_ShouldThrowException(
        int pageIndex,
        int pageSize)
    {
        // Act
        var act = () => _sut.GetAsync(pageIndex, pageSize, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task GivenThatGetPaginatedPeople_WhenInsertingValidData_ShouldReturnPaginatedPeople()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 20;

        var people = new List<Person> { _validPerson };
        var storedPaginatedPeople = new Paginated<Person>(pageIndex, pageSize, people, totalItems: 1);

        _personRepository
            .Setup(x => x.GetPaginatedAsync(It.IsAny<Criteria<Person, Person>>(), CancellationToken.None))
            .ReturnsAsync(storedPaginatedPeople);

        // Act
        var paginatedPeople = await _sut.GetAsync(pageIndex, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(paginatedPeople);
        Assert.NotEmpty(paginatedPeople.Items);

        _personRepository.Verify(
            x => x.GetPaginatedAsync(
                It.Is<Criteria<Person, Person>>(
                    x => x.PageIndex == pageIndex && x.PageSize == pageSize),
                    CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetPerson_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Act
        var act = () => _sut.GetAsync(-1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task GivenThatGetPerson_WhenNotFindPerson_ShouldThrowException()
    {
        // Arrange
        var people = new List<Person>();
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        // Act
        var act = () => _sut.GetAsync(1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(act);
        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetPerson_WhenFindPerson_ShouldReturnPerson()
    {
        // Arrange
        var people = new List<Person> { _validPerson };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        // Act
        var person = await _sut.GetAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(person);
        Assert.Same(person, _validPerson);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetAddress_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Act
        var act = () => _sut.GetAddressAsync(-1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task GivenThatGetAddress_WhenNotFindAddress_ShouldThrowException()
    {
        // Arrange
        var addresses = new List<CompositeAddress>();
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addresses);

        // Act
        var act = () => _sut.GetAddressAsync(1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(act);
        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetAddress_WhenFindAddress_ShouldReturnAddress()
    {
        // Arrange
        var addresses = new List<CompositeAddress> { _validPerson.Address };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addresses);

        // Act
        var address = await _sut.GetAddressAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(address);
        Assert.Same(address, _validPerson.Address);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatCreateActivity_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Arrange
        var activity = new Activity();

        // Act
        var act = () => _sut.AddActivityAsync(1, activity, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task GivenThatCreateActivity_WhenInsertingValidData_ShouldAdd()
    {
        // Arrange
        var people = new List<Person> { _validPerson };

        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        var activity = new Activity(
            _validPerson.Address,
            new TimeSpan(0, 0, 0),
            new TimeSpan(1, 0, 0),
            new List<short> { 6, 0 },
            "Estudar");

        // Act
        var storedActivity = await _sut.AddActivityAsync(1, activity, CancellationToken.None);

        // Assert
        Assert.Same(activity, storedActivity);
        Assert.NotEmpty(_validPerson.Activities);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _personRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _personRepository.Verify(
            x => x.SaveAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetActivites_WhenInsertingValidData_ShoulReturnActivities()
    {
        // Arrange
        var people = new List<Person> { _validPerson };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        // Act
        var activities = await _sut.GetActivitiesAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(activities);
        Assert.Equal(_validPerson.Activities.Count, activities.Count);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetActivity_WhenInsertingInvalidData_ShoulThrowException()
    {
        // Act
        var act = () => _sut.GetActivityAsync(-1, -1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenThatGetActivity_WhenNotFindActivity_ShoulThrowException()
    {
        // Arrange
        var activity = new Activity(
            _validPerson.Address,
            new TimeSpan(0, 0, 0),
            new TimeSpan(1, 0, 0),
            new List<short> { 6, 0 },
            "Estudar");
        _validPerson.Activities.Add(activity);

        var people = new List<Person> { _validPerson };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        // Act
        var act = () => _sut.GetActivityAsync(1, 1, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetActivity_WhenFindActivity_ShoulReturnActivity()
    {
        // Arrange
        var activity = new Activity(
            _validPerson.Address,
            new TimeSpan(0, 0, 0),
            new TimeSpan(1, 0, 0),
            new List<short> { 6, 0 },
            "Estudar",
            id: 1);
        _validPerson.Activities.Add(activity);

        var people = new List<Person> { _validPerson };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        // Act
        var storedActivity = await _sut.GetActivityAsync(1, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(storedActivity);
        Assert.Same(activity, storedActivity);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetGeographicallyNearbyPeople_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Act
        var act = () => _sut.GetGeographicallyNearbyPeopleAsync(-1, -1, -1, 0, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenThatGetGeographicallyNearbyPeople_WhenInsertingValidData_ShouldReturnNearbyPeople()
    {
        // Arrange
        var people = new List<CompositeAddress> { _validPerson.Address };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        var pageIndex = 0;
        var pageSize = 20;

        var paginatedPeople = new Paginated<Person>(pageIndex, pageSize, new List<Person>(), totalItems: 0);

        _geographicProximityService
            .Setup(x => x.GetAsync(
                It.IsAny<CompositeAddress>(),
                It.IsAny<double>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedPeople);

        // Act
        var nearbyPeople = await _sut.GetGeographicallyNearbyPeopleAsync(1, 6, pageIndex, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(nearbyPeople);
        Assert.Empty(nearbyPeople.Items);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _geographicProximityService.Verify(
            x => x.GetAsync(
                It.IsAny<CompositeAddress>(),
                It.IsAny<double>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenThatGetFunctionallyNearbyPeople_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Act
        var act = () => _sut.GetFunctionallyNearbyPeopleAsync(-1, -1, -1, 0, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, CompositeAddress>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenThatGetFunctionallyNearbyPeople_WhenInsertingValidData_ShouldReturnNearbyPeople()
    {
        // Arrange
        var activity = new Activity(
             _validPerson.Address,
             new TimeSpan(0, 0, 0),
             new TimeSpan(1, 0, 0),
             new List<short> { 6, 0 },
             "Estudar",
             id: 1);
        _validPerson.Activities.Add(activity);

        var people = new List<Person> { _validPerson };
        _personRepository
            .Setup(x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        var pageIndex = 0;
        var pageSize = 20;

        var paginatedPeople = new Paginated<Person>(pageIndex, pageSize, new List<Person>(), totalItems: 0);

        _functionalProximityService
            .Setup(x => x.GetAsync(
                It.IsAny<Activity>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedPeople);

        // Act
        var nearbyPeople = await _sut.GetFunctionallyNearbyPeopleAsync(1, 1, pageIndex, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(nearbyPeople);
        Assert.Empty(nearbyPeople.Items);

        _personRepository.Verify(
            x => x.GetAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _functionalProximityService.Verify(
            x => x.GetAsync(
                It.IsAny<Activity>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
