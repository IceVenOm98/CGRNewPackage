using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CgrNewPackage
{

    using Terrasoft.Core.Factories;

    [DefaultBinding(typeof(ICgrSmsSender))]
    class CgrSmsSender : ICgrSmsSender
    {
        public int SendSms(string messageAndNumber)
        {
            string pattern = @"^[а-яА-Яa-zA-Z0-9.!?]*;\+[0-9]*$";
            if (Regex.IsMatch(messageAndNumber, pattern, RegexOptions.IgnoreCase))
            {
                string[] messageData = messageAndNumber.Split(";".ToCharArray()[0]);
                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = "AC4fab17a83dc129b26ecd7155cc0473c0";   //Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                string authToken = "54a5f45603f499e34e3c56f7dbc84925";      //Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: messageData[0],
                    from: new Twilio.Types.PhoneNumber("+18144202870"),
                    to: new Twilio.Types.PhoneNumber(messageData[1])
                );
                string str = message.Sid;
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
