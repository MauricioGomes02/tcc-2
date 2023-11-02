namespace Tcc2.Domain.Tests.Entities;

public class AddressUt
{
    [Fact]
    public void GivenThatCreateAnAddress_WhenNotToEnterDataAndValidate_ShouldThrowException()
    {
        // Arrange
        var sut = new Person();

        // Act
        var act = () => sut.Validate();

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    [Theory]
    [InlineData("", "", "", "", "", default(int), "")]
    [InlineData(null, null, null, null, null, - 1, null)]
    public void GivenThatCreateAnAddress_WhenInsertingInvalidData_ShouldThrowException(
        string? country,
        string? state,
        string? city,
        string? neighborhood,
        string? street,
        int number,
        string? postalCode)
    {
        // Act
        var act = () => new Address(country!, state!, city!, neighborhood!, street!, number, postalCode!);

        // Assert
        Assert.Throws<ValidationException>(act);
    }
}
