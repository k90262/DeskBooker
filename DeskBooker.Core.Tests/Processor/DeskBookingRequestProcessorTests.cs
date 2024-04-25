using DeskBooker.Core.Domain;
using Moq;

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

    [Fact]
    public void ShouldSaveDeskBooking()
    {
        // Arrange
        DeskBooking savedDeskBooking = null;
        Mock<IDeskBookingRepository> deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
        deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
            .Callback<DeskBooking>(deskBooking =>
            {
                savedDeskBooking = deskBooking;
            });
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
        deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
        Assert.NotNull(savedDeskBooking);
        Assert.Equal(request.FirstName, savedDeskBooking.FirstName);
        Assert.Equal(request.LastName, savedDeskBooking.LastName);
        Assert.Equal(request.Date, savedDeskBooking.Date);
        Assert.Equal(request.Email, savedDeskBooking.Email);
    }
}

public interface IDeskBookingRepository
{
    void Save(DeskBooking deskBooking);
}

public class DeskBooking
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime Date { get; set; }
}