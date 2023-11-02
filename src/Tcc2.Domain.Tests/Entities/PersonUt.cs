namespace Tcc2.Domain.Tests.Entities;

public class PersonUt
{
    [Fact]
    public void GivenThatCreateAPerson_WhenNotToEnterDataAndValidate_ShouldThrowException()
    {
        // Arrange
        var sut = new Person();

        // Act
        var act = () => sut.Validate();

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    [Theory]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void GivenThatCreateAPerson_WhenInsertingInvalidData_ShouldThrowException(
        string? name,
        CompositeAddress? compositeAddress)
    {
        // Act
        var act = () => new Person(name!, compositeAddress!);

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void GivenThatUpdateTheAddress_WhenIsNull_ShouldThrowException()
    {
        // Arrange
        var address = new CompositeAddress(
            "Brasil", 
            "Rio Grande do Sul", 
            "Canoas", 
            "Centro",
            "Avenida Victor Barreto", 
            2288,
            "92010000");
        var sut = new Person("Maurício", address);

        var newAddress = null as CompositeAddress;

        // Act
        var act = () => sut.UpdateAddress(newAddress!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GivenThatCreateAPerson_WhenInsertingInvalidAddress_ShouldThrowException()
    {
        // Arrange
        var address = new CompositeAddress();

        // Act
        var act = () => new Person("Maurício", address);

        // Assert
        Assert.Throws<ValidationException>(act);
    }
}
