using Exatek.Application.Interfaces;
using Exatek.Domain.Dtos;
using Exatek.Domain.Entity;
using Exatek.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<object>> CreateAccountAsync(CreateAccountRequest request)
        {
            try
            {
                // Check if customer already exists
                var existingCustomer = await _unitOfWork.Customers.GetByIcNumberAsync(request.IcNumber);

                if (existingCustomer != null)
                {
                    // Customer exists - migration flow
                    return ApiResponse<object>.SuccessResponse(new
                    {
                        ic_number = existingCustomer.ICNumber,
                        name = existingCustomer.CustomerName,
                        phone = existingCustomer.MobileNumber,
                        email = existingCustomer.Email,
                        is_phone_confirmed = existingCustomer.IsPhoneConfirmed,
                        is_email_confirmed = existingCustomer.IsEmailConfirmed,
                        is_terms_accepted = existingCustomer.IsTermsAccepted
                    }, "Account found. Please complete verification process.");
                }

                // Create new customer
                var customer = new Customer
                {
                    ICNumber = request.IcNumber,
                    CustomerName = request.Name,
                    MobileNumber = request.Phone,
                    Email = request.Email
                };

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                var customerToken = new CustomerToken
                {
                    CustomerId = customer.Id,
                    Token = GenerateToken(),
                    ExpiredAt = DateTime.Now.AddMinutes(2),
                    Channel= "phone_verification"
                };
                await _unitOfWork.CustomerTokens.AddAsync(customerToken);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<object>.SuccessResponse(new
                {
                    ic_number = customer.ICNumber,
                    name = customer.CustomerName,
                    phone = customer.MobileNumber,
                    email = customer.Email,
                    is_phone_confirmed = customer.IsPhoneConfirmed,
                    is_email_confirmed = customer.IsEmailConfirmed,
                    is_terms_accepted = customer.IsTermsAccepted
                }, "Account created successfully. Please verify your phone number.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error creating account: {ex.Message}");
            }
        }

        public async Task<ApiResponse<object>> VerifyPhoneAsync(string icNumber, VerifyPhoneRequest request)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIcNumberAsync(icNumber);

                if (customer == null)
                {
                    return ApiResponse<object>.ErrorResponse("Customer not found");
                }

                // Find valid phone verification token
                var token = await _unitOfWork.CustomerTokens
                    .GetValidTokenAsync(customer.Id, "phone_verification", token: request.Token);

                if (token == null)
                {
                    return ApiResponse<object>.ErrorResponse("Invalid or expired verification code");
                }

                // Mark phone as confirmed and revoke token
                customer.IsPhoneConfirmed = true;
                customer.UpdatedAt = DateTime.Now;
                token.IsRevoked = true;

                await _unitOfWork.CustomerTokens.UpdateAsync(token);
                await _unitOfWork.Customers.UpdateAsync(customer);
                var customerToken = new CustomerToken
                {
                    CustomerId = customer.Id,
                    Token = GenerateToken(),
                    ExpiredAt = DateTime.Now.AddMinutes(2),
                    Channel = "email_verification"
                };
                await _unitOfWork.CustomerTokens.AddAsync(customerToken);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<object>.SuccessResponse(new
                {
                    ic_number = customer.ICNumber,
                    is_phone_confirmed = customer.IsPhoneConfirmed,
                    next_step = "email_verification"
                }, "Phone verified successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error verifying phone: {ex.Message}");
            }
        }

        public async Task<ApiResponse<object>> VerifyEmailAsync(string icNumber, VerifyEmailRequest request)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIcNumberAsync(icNumber);

                if (customer == null)
                {
                    return ApiResponse<object>.ErrorResponse("Customer not found");
                }

                // Find valid email verification token
                var token = await _unitOfWork.CustomerTokens
                    .GetValidTokenAsync(customer.Id, "email_verification", token: request.Token);

                if (token == null)
                {
                    return ApiResponse<object>.ErrorResponse("Invalid or expired verification token");
                }

                // Mark email as confirmed and revoke token
                customer.IsEmailConfirmed = true;
                customer.UpdatedAt = DateTime.Now;
                token.IsRevoked = true;

                await _unitOfWork.CustomerTokens.UpdateAsync(token);
                await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<object>.SuccessResponse(new
                {
                    ic_number = customer.ICNumber,
                    is_email_confirmed = customer.IsEmailConfirmed,
                    next_step = "accept_terms"
                }, "Email verified successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error verifying email: {ex.Message}");
            }
        }

        public async Task<ApiResponse<object>> AcceptTermsAsync(string icNumber)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIcNumberAsync(icNumber);

                if (customer == null)
                {
                    return ApiResponse<object>.ErrorResponse("Customer not found");
                }

                if (!customer.IsPhoneConfirmed || !customer.IsEmailConfirmed)
                {
                    return ApiResponse<object>.ErrorResponse("Please complete phone and email verification first");
                }

                customer.IsTermsAccepted = true;
                customer.UpdatedAt = DateTime.Now;

                await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<object>.SuccessResponse(new
                {
                    ic_number = customer.ICNumber,
                    is_terms_accepted = customer.IsTermsAccepted,
                    registration_complete = true
                }, "Terms accepted. Registration completed successfully!");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error accepting terms: {ex.Message}");
            }
        }

        public async Task<ApiResponse<object>> GetCustomerStatusAsync(string icNumber)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIcNumberAsync(icNumber);

                if (customer == null)
                {
                    return ApiResponse<object>.ErrorResponse("Customer not found");
                }

                string nextStep = "complete";
                if (!customer.IsPhoneConfirmed)
                    nextStep = "phone_verification";
                else if (!customer.IsEmailConfirmed)
                    nextStep = "email_verification";
                else if (!customer.IsTermsAccepted)
                    nextStep = "accept_terms";

                return ApiResponse<object>.SuccessResponse(new
                {
                    ic_number = customer.ICNumber,
                    name = customer.CustomerName,
                    phone = customer.MobileNumber,
                    email = customer.Email,
                    is_phone_confirmed = customer.IsPhoneConfirmed,
                    is_email_confirmed = customer.IsEmailConfirmed,
                    is_terms_accepted = customer.IsTermsAccepted,
                    next_step = nextStep
                }, "Customer status retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error retrieving status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<object>> SendVerificationAsync(string icNumber, SendVerificationRequest request)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIcNumberAsync(icNumber);

                if (customer == null)
                {
                    return ApiResponse<object>.ErrorResponse("Customer not found");
                }

                if (request.Channel.ToLower() == "email")
                {
                    await _unitOfWork.CustomerTokens
                        .RevokeTokensByCustomerAndChannelAsync(customer.Id, "email_verification");

                    var emailToken = GenerateToken(); 

                    var newToken = new CustomerToken
                    {
                        CustomerId = customer.Id,
                        Channel = "email_verification",
                        Token = emailToken,
                        ExpiredAt = DateTime.Now.AddMinutes(2)
                    };

                    await _unitOfWork.CustomerTokens.AddAsync(newToken);
                    await _unitOfWork.SaveChangesAsync();


                    return ApiResponse<object>.SuccessResponse(new
                    {
                        channel = "email",
                        expires_in = 120
                    }, "Verification link sent to your email address");
                }

                return ApiResponse<object>.ErrorResponse("Invalid channel. Use 'email' for email verification.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error sending verification: {ex.Message}");
            }
        }

        public async Task<ApiResponse<object>> SetPinAsync(string icNumber, SetPinRequest request)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIcNumberAsync(icNumber);

                if (customer == null)
                {
                    return ApiResponse<object>.ErrorResponse("Customer not found");
                }

                var customerPin = new CustomerPin
                {
                    CustomerId = customer.Id,
                    PIN = request.Pin,
                };

                await _unitOfWork.CustomerPins.AddAsync(customerPin);
                await _unitOfWork.SaveChangesAsync();;

                return ApiResponse<object>.SuccessResponse(new
                {
                    ic_number = customer.ICNumber,
                    pin_set = true,
                }, "PIN has been set successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse($"Error setting PIN: {ex.Message}");
            }
        }

        public string GenerateToken()
        {
            Random random = new Random();
            int token = random.Next(0, 10000);
            return token.ToString("D4");
        }
    }
}
