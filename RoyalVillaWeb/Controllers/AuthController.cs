using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Diagnostics;

namespace RoyalVillaWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var response = await _authService.LoginAsync<ApiResponse<LoginResponseDTO>>(loginRequestDTO);
                if (response != null && response.Sucess && response.Data != null)
                {
                    LoginResponseDTO model = response.Data;
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistrationRequestDTO 
            {
                Email=string.Empty,
                Name=string.Empty,
                Password=string.Empty,
                Role="Customer"
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                ApiResponse<UserDTO> response = await _authService.RegisterAsync<ApiResponse<UserDTO>>(registrationRequestDTO);
                if (response != null && response.Sucess && response.Data != null)
                {
                    TempData["success"] = "Registration successfull! Please login with your credentials";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Registration failed.Please try again.";
                    return View(registrationRequestDTO);
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View(registrationRequestDTO);
        }
        public IActionResult AccessDenied()
        {
            return View(); 
        }
        public async Task<IActionResult> Logout()
        {
            return View();
        }


    }
}
