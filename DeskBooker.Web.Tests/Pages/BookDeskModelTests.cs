using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Moq;
using Xunit;

namespace DeskBooker.Web.Pages
{
  public class BookDeskModelTests
  {
    private readonly Mock<IDeskBookingRequestProcessor> _processorMock;
    private readonly BookDeskModel _bookDeskModel;
    private readonly DeskBookingResult _deskBookingResult;

    public BookDeskModelTests()
    {
      // Arrange
      _processorMock = new Mock<IDeskBookingRequestProcessor>();
      
      _bookDeskModel = new BookDeskModel(_processorMock.Object)
      {
        DeskBookingRequest = new DeskBookingRequest()
      };
      
      _deskBookingResult = new DeskBookingResult
      {
        Code = DeskBookingResultCode.Success
      };
      
      _processorMock.Setup(x => x.BookDesk(_bookDeskModel.DeskBookingRequest))
        .Returns(_deskBookingResult);
    }
    
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ShouldCallBookDeskMethodOfProcessorIfModleIsValid(
      int expectedBookDeskCalls, bool isModelValid)
    {
      // Arrange
      if (!isModelValid)
      {
        _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
      }

      // Act
      _bookDeskModel.OnPost();
      
      // Assert
      _processorMock.Verify(x => x.BookDesk(_bookDeskModel.DeskBookingRequest),
        Times.Exactly(expectedBookDeskCalls));
    }

    [Fact]
    public void ShouldAddModelErrorIfNoDeskIsAvailable()
    {
      // Arrange
      _deskBookingResult.Code = DeskBookingResultCode.NoDeskAvailable;
      
      // Act
      _bookDeskModel.OnPost();
      
      // Assert
      var modelStateEntry = Assert.Contains("DeskBookingRequest.Date", _bookDeskModel.ModelState);
      var modelError = Assert.Single(modelStateEntry.Errors);
      Assert.Equal("No desk available for selected date", modelError.ErrorMessage);
    }
    
    [Fact]
    public void ShouldNotAddModelErrorIfDeskIsAvailable()
    {
      // Arrange
      _deskBookingResult.Code = DeskBookingResultCode.Success;
      
      // Act
      _bookDeskModel.OnPost();
      
      // Assert
      Assert.DoesNotContain("DeskBookingRequest.Date", _bookDeskModel.ModelState);
    }
  }
}
