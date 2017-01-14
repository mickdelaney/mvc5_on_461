using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Messages
{
    public class TraceMessage
    {
        public string Message { get; protected set; }

        protected TraceMessage() { }
        public TraceMessage(string message)
        {
            Message = message;
        }
    }
}
