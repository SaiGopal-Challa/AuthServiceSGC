using AuthServiceSGC.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Services
{
    public class OTPService: IOTPService
    {
        public OTPService() { }

        // send otp via mobile, call send SMSService

        // send otp via email, call send EmailService

        //call OTPRepository to save the otp


    }
}
