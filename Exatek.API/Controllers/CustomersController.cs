using Exatek.Application.Interfaces;
using Exatek.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exatek.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input data"));
            }

            var result = await _customerService.CreateAccountAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{icNumber}/verify-phone")]
        public async Task<IActionResult> VerifyPhone(string icNumber, [FromBody] VerifyPhoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input data"));
            }

            var result = await _customerService.VerifyPhoneAsync(icNumber, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{icNumber}/verify-email")]
        public async Task<IActionResult> VerifyEmail(string icNumber, [FromBody] VerifyEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input data"));
            }

            var result = await _customerService.VerifyEmailAsync(icNumber, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{icNumber}/accept-terms")]
        public async Task<IActionResult> AcceptTerms(string icNumber)
        {
            var result = await _customerService.AcceptTermsAsync(icNumber);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{icNumber}/status")]
        public async Task<IActionResult> GetStatus(string icNumber)
        {
            var result = await _customerService.GetCustomerStatusAsync(icNumber);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("{icNumber}/send-verification")]
        public async Task<IActionResult> SendVerification(string icNumber, [FromBody] SendVerificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input data"));
            }

            var result = await _customerService.SendVerificationAsync(icNumber, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{icNumber}/set-pin")]
        public async Task<IActionResult> SetPin(string icNumber, [FromBody] SetPinRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input data"));
            }

            var result = await _customerService.SetPinAsync(icNumber, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
