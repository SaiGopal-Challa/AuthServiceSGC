using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/UpdateAccount")]
    public class UpdateAccountController : ControllerBase
    {

        //Update account controller
        [HttpPost]
        [Route("UpdateAccount")]
        public async Task<ActionResult<UpdateAccountResponse>> UpdateAccountAsync(UpdateAccountRequest updateAccountRequest)
        {
            UpdateAccountResponse updateAccountResponse = new UpdateAccountResponse();

            return Ok(updateAccountResponse);
        }

        //Delete account controller
        [HttpPost]
        [Route("DeleteAccount")]
        public async Task<ActionResult<UpdateAccountResponse>> DeleteAccount(DeleteAccountRequest deleteAccountRequest)
        {
            UpdateAccountResponse updateAccountResponse = new UpdateAccountResponse();

            return Ok(updateAccountResponse);
        }
    }
}
