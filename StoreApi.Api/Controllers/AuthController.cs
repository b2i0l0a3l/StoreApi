using StoreSystem.Application.Contract.Auth.Req;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.Token.req;
using StoreSystem.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Auth;
using StoreSystem.Core.Interfaces;
using StoreSystem.Application.Contract.Auth.Login.req;

namespace BookingApi.Api.Controllers
    {
        [ApiController]
        [Route("api/Auth")]
        public partial class AuthController : ControllerBase
        {
            private readonly IAuth _authService;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IEmailSender _emailSender;
            private readonly IToken _tokenService;

            public AuthController(IAuth authService, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IToken tokenService)
            {
                _authService = authService;
                _userManager = userManager;
                _emailSender = emailSender;
                _tokenService = tokenService;
            }

            [HttpPost("Register")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Register([FromBody] SingUp model) =>
                Ok(await _authService.Register(model));

            [HttpPost("Login")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Login([FromBody] LoginModel model) =>
                Ok(await _authService.Login(model));

        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] TokenReq model) =>
            Ok(await _authService.Refresh(model));
                    
         [HttpPost("google-login")]
            public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginReq model)
        => Ok(await _authService.Login(model));
            
            [HttpGet("ConfirmEmail")]
            public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null) return NotFound();

                var incomingToken = token
                                        .Replace("-", "+")
                                        .Replace("_", "/");
                switch (incomingToken.Length % 4)
                {
                    case 2: incomingToken += "=="; break;
                    case 3: incomingToken += "="; break;
                }

                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(incomingToken));
                var result = await _userManager.ConfirmEmailAsync(user, decoded);

                            if (!result.Succeeded) return BadRequest(result.Errors);
                                
                                return Ok(new { message = "Email confirmed" });
                }
    }
}