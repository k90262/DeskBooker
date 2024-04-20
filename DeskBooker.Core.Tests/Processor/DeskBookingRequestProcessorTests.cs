using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessorTests
{
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
        
        var processor = new DeskBookingRequestProcessor();

        // Act
        DeskBookingResult result = processor.BookDesk(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Date, result.Date);
    }
}