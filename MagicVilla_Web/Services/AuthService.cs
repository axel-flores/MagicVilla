using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _httpClient;
        private string villaUrl;

        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            this._httpClient = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = loginRequestDTO,
                ApiUrl = villaUrl + "/api/v1/UsersAuth/login"

            });
        }

        public Task<T> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = registrationRequestDTO,
                ApiUrl = villaUrl + "/api/v1/UsersAuth/register"
            });
        }
    }
}
