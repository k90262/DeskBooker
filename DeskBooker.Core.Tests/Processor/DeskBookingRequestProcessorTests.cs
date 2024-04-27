using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessorTests
{
    private readonly DeskBookingRequestProcessor _processor;
    private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
    private readonly DeskBookingRequest _request;

    public DeskBookingRequestProcessorTests()
    {
        _request = new DeskBookingRequest
        {
            FirstName = "Bill",
            LastName = "Ho",
            Email = "bill.ho@test.org",
            Date = new DateTime(2024, 04, 20)
        };
        _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
        _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object);
    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        // Arrange

        // Act
        DeskBookingResult result = _processor.BookDesk(_request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_request.FirstName, result.FirstName);
        Assert.Equal(_request.LastName, result.LastName);
        Assert.Equal(_request.Email, result.Email);
        Assert.Equal(_request.Date, result.Date);
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
        _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
            .Callback<DeskBooking>(deskBooking =>
            {
                savedDeskBooking = deskBooking;
            });
        
        // Act
        DeskBookingResult result = _processor.BookDesk(_request);
        
        // Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
        
        Assert.NotNull(savedDeskBooking);
        Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
        Assert.Equal(_request.LastName, savedDeskBooking.LastName);
        Assert.Equal(_request.Date, savedDeskBooking.Date);
        Assert.Equal(_request.Email, savedDeskBooking.Email);
    }
}