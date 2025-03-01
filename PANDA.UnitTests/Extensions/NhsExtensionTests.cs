using FluentAssertions;
using PANDA.Common.Extensions;

namespace PANDA.UnitTests.Extensions;

[TestFixture]
public class NhsExtentionTests
{
    [Test]
    public void TryParse_ValidNhsNumber_ReturnsTrueAndCleansNumber()
    {
        // Arrange
        string input = "943 476 5919";

        // Act
        bool result = NhsExtensions.TryParse(input, out var cleanedNhsNumber);

        // Assert
        result.Should().BeTrue();
        cleanedNhsNumber.Should().Be("9434765919");
    }

    [Test]
    public void TryParse_InvalidNhsNumber_ReturnsFalseAndNullCleanedNumber()
    {
        // Arrange
        string input = "1234567890";

        // Act
        bool result = NhsExtensions.TryParse(input, out var cleanedNhsNumber);

        // Assert
        result.Should().BeFalse();
        cleanedNhsNumber.Should().BeNull();
    }

    [TestCase("9434765919", true, "9434765919")]
    [TestCase("943 476 5919", true, "9434765919")]
    [TestCase("abcdefghij", false, null)]
    [TestCase("943476591", false, null)]
    [TestCase("94347659199", false, null)]
    [TestCase("1234567890", false, null)]
    [TestCase(null, false, null)]
    [TestCase(" ", false, null)]
    public void TryParse_VariousCases(string? input, bool expectedIsValid, string? expectedCleanedNumber)
    {
        // Act
        bool result = NhsExtensions.TryParse(input, out var cleanedNhsNumber);

        // Assert
        result.Should().Be(expectedIsValid);
        cleanedNhsNumber.Should().Be(expectedCleanedNumber);
    }
}