using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/UsersAuth")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserRepository _user;
        private readonly APIResponse _apiResponse;

        public UsersController(UserRepository user, APIResponse apiResponse)
        {
            this._user = user;
            this._apiResponse = apiResponse;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _user.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = loginResponse;

            return Ok(_apiResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserNameUnique = _user.IsUnique(model.UserName);
            if (!ifUserNameUnique)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username already exists");
                return BadRequest(_apiResponse);
            }

            var user = await _user.Register(model);
            if(user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Error while registering");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }
    }
}
