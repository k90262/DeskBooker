using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Moq;
using Xunit;

namespace DeskBooker.Web.Pages
{
  public class BookDeskModelTests
  {

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ShouldCallBookDeskMethodOfProcessorIfModleIsValid(
      int expectedBookDeskCalls, bool isModelValid)
    {
      // Arrange
      var processorMock = new Mock<IDeskBookingRequestProcessor>();
      var bookDeskModel = new BookDeskModel(processorMock.Object)
      {
        DeskBookingRequest = new DeskBookingRequest()
      };
      if (!isModelValid)
      {
        bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
      }

      // Act
      bookDeskModel.OnPost();
      
      // Assert
      processorMock.Verify(x => x.BookDesk(bookDeskModel.DeskBookingRequest),
        Times.Exactly(expectedBookDeskCalls));
    }

    [Fact]
    public void ShouldAddModelErrorIfNoDeskIsAvailable()
    {
      // Arrange
      var processorMock = new Mock<IDeskBookingRequestProcessor>();
      var bookDeskModel = new BookDeskModel(processorMock.Object)
      {
        DeskBookingRequest = new DeskBookingRequest()
      };
      processorMock.Setup(x => x.BookDesk(bookDeskModel.DeskBookingRequest))
        .Returns(new DeskBookingResult
        {
          Code = DeskBookingResultCode.NoDeskAvailable
        });
      
      // Act
      bookDeskModel.OnPost();
      
      // Assert
      var modelStateEntry = Assert.Contains("DeskBookingRequest.Date", bookDeskModel.ModelState);
      var modelError = Assert.Single(modelStateEntry.Errors);
      Assert.Equal("No desk available for selected date", modelError.ErrorMessage);
    }
  }
}
