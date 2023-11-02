namespace Tcc2.Domain.Tests.Entities;

public class ActivityUt
{
    [Fact]
    public void GivenThatCreateAnActivity_WhenNotToEnterDataAndValidate_ShouldThrowException()
    {
        // Arrange
        var sut = new Activity();

        // Act
        var act = () => sut.Validate();

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    [Theory]
    [MemberData(nameof(GetInvalidDatas))]
    public void GivenThatCreateAnActivity_WhenInsertingInvalidData_ShouldThrowException(
        Address? address,
        TimeSpan start,
        TimeSpan end,
        IEnumerable<short>? daysOfWeek,
        string? description)
    {
        // Act
        var act = () => new Activity(
            address!,
            start,
            end,
            daysOfWeek!,
            description!);

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    public static IEnumerable<object[]> GetInvalidDatas()
    {
        yield return new object[]
        {
            null!,
            TimeSpan.Zero.Add(new TimeSpan(-1)),
            TimeSpan.Zero.Add(new TimeSpan(-1)),
            null!,
            null!
        };

        yield return new object[]
        {
            new Address(),
            TimeSpan.Zero,
            TimeSpan.Zero.Add(new TimeSpan(1)),
            Enumerable.Empty<short>(),
            string.Empty
        };

        yield return new object[]
        {
            new Address(),
            TimeSpan.Zero,
            TimeSpan.Zero.Add(new TimeSpan(1)),
            new List<short> { 2, 2, 3 },
            string.Empty
        };
    }
}
