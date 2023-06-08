using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            this._authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            //hacer una peticion login async
            APIResponse response = await _authService.LoginAsync<APIResponse>(loginRequestDTO);
            if(response != null && response.IsSuccess)
            {
                LoginResponseDTO loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDTO>
                    (Convert.ToString(response.Result));

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                identity.AddClaim(new Claim(ClaimTypes.Name, loginResponseDTO.User.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, loginResponseDTO.User.Role));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(StaticDetails.SessionToken, loginResponseDTO.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError",response.ErrorMessages.FirstOrDefault());
                return View(loginRequestDTO);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            APIResponse result = await _authService.RegisterAsync<APIResponse>(registrationRequestDTO);
            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(StaticDetails.SessionToken , "");
            return RedirectToAction("Index","Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
