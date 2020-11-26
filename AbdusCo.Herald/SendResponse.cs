using System.Collections.Generic;
using System.Linq;

namespace AbdusCo.Herald
{
    public class SendResponse
    {
        public List<string> Errors = new List<string>();
        public bool IsSuccessful => !Errors.Any();

        private SendResponse()
        {
        }

        public static SendResponse Success => new SendResponse();
        public static SendResponse Fail(params string[] messages) => new SendResponse {Errors = messages.ToList()};
    }
}