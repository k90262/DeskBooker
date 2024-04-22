using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessorTests
{
    private readonly DeskBookingRequestProcessor _processor;

    public DeskBookingRequestProcessorTests()
    {
        _processor = new DeskBookingRequestProcessor();
    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        // Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Bill",
            LastName = "Ho",
            Email = "bill.ho@test.org",
            Date = new DateTime(2024, 04, 20)
        };

        // Act
        DeskBookingResult result = _processor.BookDesk(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Date, result.Date);
    }

    [Fact]
    public void ShouldThrowExceptionIfRequestIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

        Assert.Equal("request", exception.ParamName);
    }
}