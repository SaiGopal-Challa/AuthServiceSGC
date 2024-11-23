using AuthServiceSGC.API.Models.Requests;
using AuthServiceSGC.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSGC.API.Controllers
{
    [ApiController]
    [Route("api/UpdateAccount")]
    public class UpdateAccountController
    {

        //Update account controller
        [HttpPost]
        [Route("UpdateAccount")]
        public async Task<ActionResult<UpdateAccountResponse>> UpdateAccountAsync(UpdateAccountRequest updateAccountRequest)
        {
            UpdateAccountResponse updateAccountResponse = new UpdateAccountResponse();

            return updateAccountResponse;
        }

        //Delete account controller

    }
}
