using FluentAssertions;
using PANDA.Common.Extensions;

namespace PANDA.UnitTests.Extensions;

[TestFixture]
public class PostcodeExtensionTests
{
    [TestCase("sw1a1aa", "SW1A 1AA")]
    [TestCase("SW1A1AA", "SW1A 1AA")]
    [TestCase("sw1a 1aa", "SW1A 1AA")]
    [TestCase("Sw1A-1AA", "SW1A 1AA")]
    [TestCase(" EC1A1BB ", "EC1A 1BB")]
    public void CoercePostcode_ValidPostcodes_AreCoercedCorrectly(string input, string expected)
    {
        var result = PostcodeExtensions.Coerce(input);
        result.Should().Be(expected);
    }

    [TestCase("")]
    [TestCase("12345")]
    [TestCase("ABCDE")]
    [TestCase("AA11AAAA")]  // Invalid format
    public void CoercePostcode_InvalidPostcodes_ThrowsArgumentException(string input)
    {
        Action act = () => PostcodeExtensions.Coerce(input);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Postcode Invalid Format");
    }

    [Test]
    public void CoercePostcode_NullPostcode_ThrowsArgumentException()
    {
        Action act = () => PostcodeExtensions.Coerce(null);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Postcode Invalid Format");
    }
}
