using Moq;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Models.Pagination;
using Tcc2.Domain.Services;

namespace Tcc2.Domain.Tests.Services;

public class FunctionalProximityServiceUt
{
    private readonly Mock<IPersonRepository> _repository;
    private readonly FunctionalProximityService _sut;

    public FunctionalProximityServiceUt()
    {
        _repository = new Mock<IPersonRepository>();
        _sut = new FunctionalProximityService(_repository.Object);
    }

    [Fact]
    public async Task GivenThatGetPaginatedNearbyPeople_WhenInsertingValidData_ShouldReturnPaginatedNearbyPeople()
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

        var activity = new Activity(
           address,
           new TimeSpan(0, 0, 0),
           new TimeSpan(1, 0, 0),
           new List<short> { 6, 0 },
           "Estudar");

        var people = new List<Person> { new Person("Maurício", address) };

        var paginatedPeople = new Paginated<Person>(pageIndex, pageSize, people, totalItems: 1);
        _repository
            .Setup(x => x.GetPaginatedAsync(It.IsAny<Criteria<Person, Person>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedPeople);

        // Act
        var storedPaginatedPeople = await _sut.GetAsync(
            activity,
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
