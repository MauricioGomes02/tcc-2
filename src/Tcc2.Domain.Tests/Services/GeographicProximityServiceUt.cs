using Moq;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Models.Pagination;
using Tcc2.Domain.Services;

namespace Tcc2.Domain.Tests.Services;

public class GeographicProximityServiceUt
{
    private readonly Mock<IPersonRepository> _repository;
    private readonly GeographicProximityService _sut;

    public GeographicProximityServiceUt()
    {
        _repository = new Mock<IPersonRepository>();
        _sut = new GeographicProximityService(_repository.Object);
    }

    [Fact]
    public async Task GivenThatGetPaginatedNearbyPeople_WhenInsertingInvalidData_ShouldThrowException()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 20;

        var address = new CompositeAddress(
           "Brasil",
           "Rio Grande do Sul",
           "Canoas",
           "Centro",
           "Avenida Victor Barreto",
           2288,
           "92010000");

        // Act
        var act = () => _sut.GetAsync(
           address,
           5,
           pageIndex,
           pageSize,
           CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task GivenThatGetPaginatedNearbyPeople_WhenInsertingValidData_ShouldReturnPaginatedNearbyPeople()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 20;

        var latitude = -29.917270584965024;
        var longitude = -51.181264857578256;
        var geographicCoordinate = new GeographicCoordinate(latitude, longitude);

        var address = new CompositeAddress(
           "Brasil",
           "Rio Grande do Sul",
           "Canoas",
           "Centro",
           "Avenida Victor Barreto",
           2288,
           "92010000",
           geographicCoordinate);

        var people = new List<Person> { new Person("Maurício", address) };

        var paginatedPeople = new Paginated<Person>(pageIndex, pageSize, people, totalItems: 1);
        _repository
            .Setup(x => x.GetPaginatedAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedPeople);

        // Act
        var storedPaginatedPeople = await _sut.GetAsync(
            address,
            5,
            pageIndex,
            pageSize,
            CancellationToken.None);

        // Assert
        Assert.NotNull(storedPaginatedPeople);
        Assert.Same(storedPaginatedPeople, paginatedPeople);

        _repository.Verify(
            x => x.GetPaginatedAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
