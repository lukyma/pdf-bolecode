using CreatePdf.Models;
using CreatePdf.Services;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Diagnostics;

namespace CreatePdf.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IViewRenderService _ViewRenderService { get; }

        public HomeController(ILogger<HomeController> logger, IViewRenderService viewRenderService)
        {
            _logger = logger;
            _ViewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Pdf([FromQuery]bool isBase64 = false)
        {
            var barCode = "34199933600000000011090000019862938570047000";
            var boleto = new Boleto { NumeroBoleto = barCode };
            var response = await _ViewRenderService.RenderToStringAsync("Home/Pdf", boleto);
            //Download the PDF document in the browser.
            //FileStreamResult fileStreamResult = new FileStreamResult(outputDocument, "application/pdf");
            //return File(teste, "application/pdf");
            var viewPdf = new ViewAsPdf(boleto);
            if (isBase64)
                return Ok(Convert.ToBase64String(await viewPdf.BuildFile(ControllerContext)));

            return viewPdf;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}