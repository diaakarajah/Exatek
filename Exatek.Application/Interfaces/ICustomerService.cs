using Exatek.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Application.Interfaces
{
    public interface ICustomerService : IService
    {
        Task<ApiResponse<object>> CreateAccountAsync(CreateAccountRequest request);
        Task<ApiResponse<object>> VerifyPhoneAsync(string icNumber, VerifyPhoneRequest request);
        Task<ApiResponse<object>> VerifyEmailAsync(string icNumber, VerifyEmailRequest request);
        Task<ApiResponse<object>> AcceptTermsAsync(string icNumber);
        Task<ApiResponse<object>> GetCustomerStatusAsync(string icNumber);
        Task<ApiResponse<object>> SendVerificationAsync(string icNumber, SendVerificationRequest request);
        Task<ApiResponse<object>> SetPinAsync(string icNumber, SetPinRequest request);
    }
}
