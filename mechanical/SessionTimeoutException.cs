using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace mechanical
{
    public class SessionTimeoutException : Exception
    {
        public SessionTimeoutException(string message) : base(message)
        {
        }
    }

}
