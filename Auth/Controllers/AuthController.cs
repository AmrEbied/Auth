using Auth.Handlers.Commands.ChangePassword.Dto;
using Auth.Handlers.Commands.ForgetPassword.Dto;
using Auth.Handlers.Commands.Login.Dto;
using Auth.Handlers.Commands.RefreshToken.Dto;
using Auth.Handlers.Commands.Register.Dto;
using Auth.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegisterResponseDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register([FromForm] RegisterDto model)
        {
            try
            {
                return Ok(await _mediator.Send(new Handlers.Commands.Register.Commend { Model = model }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponseDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromForm] LoginDto model)
        {
            try
            {
                return Ok(await _mediator.Send(new Handlers.Commands.Login.Commend { Model = model }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ForgetPassword")]
        [ProducesResponseType(typeof(AuthModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ForgetPassword([FromForm] ForgetPasswordDto model)
        {
            try
            {

                return Ok(await _mediator.Send(new Handlers.Commands.ForgetPassword.Commend { Model = model }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(AuthModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto model)
        {
            try
            {
                return Ok(await _mediator.Send(new Handlers.Commands.ChangePassword.Commend { Model = model }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("SendCode")]
        [ProducesResponseType(typeof(AuthModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendCode(string email)
        {
            try
            {
                return Ok(await _mediator.Send(new Handlers.Commands.SendCode.Commend { Email = email }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("RefreshToken")]
        [ProducesResponseType(typeof(RefreshTokenResponseDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            try
            {
                return Ok(await _mediator.Send(new Handlers.Commands.RefreshToken.Commend { Token = refreshToken }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("RevokeToken")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RevokeToken(string refreshToken)
        {
            try
            {
                return Ok(await _mediator.Send(new Handlers.Commands.RevokeToken.Commend { Token = refreshToken }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
