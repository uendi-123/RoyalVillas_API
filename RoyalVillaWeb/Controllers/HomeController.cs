using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Diagnostics;

namespace RoyalVillaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public HomeController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaDTO> villaList = new();
            try {
                var response = await _villaService.GetAllAsync<ApiResponse<List<VillaDTO>>>("");
                if (response != null && response.Sucess && response.Data!=null) 
                { 
                    villaList = response.Data;
                }

            }
            catch (Exception ex) {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View(villaList);
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
