using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVillas_API.Data;
using RoyalVillas_API.Models.DTO;
using RoyalVillas_API.Services;

namespace RoyalVillas_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService=authService;

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register([FromBody]RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                if (registrationRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registration data is required"));
                }
                if (await _authService.IsEmailExistAsync(registrationRequestDTO.Email))
                {
                    return Conflict(ApiResponse<object>.Conflict("User with Email '{registrationRequestDTO.Email}' already exists"));
                }
                var user = await _authService.RegisterAsync(registrationRequestDTO);
                if (user == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Failed to register user"));
                }


                //auth service
                var response = ApiResponse<UserDTO>.CreateAt(user,
                    "User created successfully");

                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "$An error occured while creating villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LoginRequestDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login data is required"));
                }
                
                var loginResponse = await _authService.LoginAsync(loginRequestDTO);
                if (loginResponse == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Failed to login user"));
                }


                //auth service
                var response = ApiResponse<LoginResponseDTO>.Ok(loginResponse,
                    "Login successfull");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "$An error occured during login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
