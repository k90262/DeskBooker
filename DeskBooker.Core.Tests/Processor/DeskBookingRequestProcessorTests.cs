using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessorTests
{
    private readonly DeskBookingRequestProcessor _processor;
    private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
    private readonly Mock<IDeskRepository> _deskRepositoryMock;
    private readonly DeskBookingRequest _request;
    private readonly List<Desk> _availableDesks;

    public DeskBookingRequestProcessorTests()
    {
        _request = new DeskBookingRequest
        {
            FirstName = "Bill",
            LastName = "Ho",
            Email = "bill.ho@test.org",
            Date = new DateTime(2024, 04, 20)
        };

        _availableDesks = new List<Desk> { new Desk { Id = 7} };
        _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
        _deskRepositoryMock = new Mock<IDeskRepository>();
        _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date))
            .Returns(_availableDesks);
        
        _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
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
        Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId);
    }

    [Fact]
    public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
    {
        // Arrange
        _availableDesks.Clear();
        
        // Act
        DeskBookingResult result = _processor.BookDesk(_request);
        
        // Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);

    }

    [Theory]
    [InlineData(DeskBookingResultCode.Success, true)]
    [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
    public void ShouldReturnExpectedResultCode(
        DeskBookingResultCode expectedResultCode, bool isDeskAvailable)
    {
        // Arrange
        if (!isDeskAvailable)
        {
            _availableDesks.Clear();
        }
        
        // Act
        DeskBookingResult result = _processor.BookDesk(_request);
        
        // Assert
        Assert.Equal(expectedResultCode, result.Code);
    }
    
    [Theory]
    [InlineData(5, true)]
    [InlineData(null, false)]
    public void ShouldReturnExpectedDeskBookingId(
        int? expectedDeskBookingId, bool isDeskAvailable)
    {
        // Arrange
        if (!isDeskAvailable)
        {
            _availableDesks.Clear();
        }
        else
        {
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking =>
                {
                    deskBooking.Id = expectedDeskBookingId.Value;
                });
        }

        // Act
        DeskBookingResult result = _processor.BookDesk(_request);
        
        // Assert
        Assert.Equal(expectedDeskBookingId, result.DeskBookingId);
    }
}