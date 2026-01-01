namespace MusicStore.UnitTests;

public class IntegerAdditionTests
{
    [Fact]
    public void Add_OnePlusTwo_ShouldReturnThree()
    {
        // Arrange
        int firstNumber = 1;
        int secondNumber = 2;

        // Act
        var sum = firstNumber + secondNumber;

        // Assert
        Assert.Equal(3, sum);
    }

    [Theory]
    [InlineData(1, 2, 3)]   // Caso básico
    [InlineData(-1, -2, -3)] // Suma de negativos
    [InlineData(0, 0, 0)]   // Suma de ceros
    [InlineData(-5, 5, 0)]  // Suma que da cero
    [InlineData(int.MaxValue, 0, int.MaxValue)] // Límite superior
    [InlineData(int.MinValue, 0, int.MinValue)] // Límite inferior
    public void Add_TwoIntegers_ShouldReturnCorrectSum(int firstNumber, int secondNumber, int expected)
    {
        // Act
        var sum = firstNumber + secondNumber;

        // Assert
        Assert.Equal(expected, sum);
    }
}