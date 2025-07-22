using Microsoft.AspNetCore.Mvc;

namespace GMS.WebUI.Controllers.Guests
{
    public class ReservationController : Controller
    {
        public async Task<IActionResult> GuestReservation()
        {
            return View();
        }
    }
}
