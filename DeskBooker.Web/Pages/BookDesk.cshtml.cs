using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeskBooker.Web.Pages
{
  public class BookDeskModel : PageModel
  {
    private readonly IDeskBookingRequestProcessor _processorMockObject;

    public BookDeskModel(IDeskBookingRequestProcessor processorMockObject)
    {
      _processorMockObject = processorMockObject;
    }

    [BindProperty]
    public DeskBookingRequest DeskBookingRequest { get; set; }

    public void OnPost()
    {
      
    }
  }
}