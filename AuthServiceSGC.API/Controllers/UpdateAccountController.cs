﻿using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using AuthServiceSGC.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/UpdateAccount")]
    public class UpdateAccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public UpdateAccountController(IUserService userService)
        {
            _userService = userService;
        }

        //Update account controller
        [HttpPost]
        [Route("UpdateAccount")]
        public async Task<ActionResult<UpdateAccountResponse>> UpdateAccountAsync(UpdateAccountRequest updateAccountRequest)
        {
            UpdateAccountResponse updateAccountResponse = new UpdateAccountResponse();
            // Create a DTO object from the UpdateAccountRequest
            // Call the UpdateAccountAsync method from the UserService 
            return Ok(updateAccountResponse);
        }

        //Delete account controller
        [HttpPost]
        [Route("DeleteAccount")]
        public async Task<ActionResult<UpdateAccountResponse>> DeleteAccount(DeleteAccountRequest deleteAccountRequest)
        {
            UpdateAccountResponse updateAccountResponse = new UpdateAccountResponse();
            // Create a DTO object from the DeleteAccountRequest
            // Call the DeleteAccountAsync method from the UserService
            return Ok(updateAccountResponse);
        }
    }
}
