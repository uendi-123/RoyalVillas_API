using Microsoft.AspNetCore.Mvc;
using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{

    public class AuthService : BaseService, IAuthService
    {
        private readonly string _villaUrl;
        private const string APIEndpoint = "/api/auth";
        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {

        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = APIEndpoint+"/login",
                
            });
        }
        public Task<T> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDTO,
                Url = APIEndpoint+"/register",
            });
        }
    }
}
