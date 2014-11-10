using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class OpenCircuitException : ApplicationException
    {
        public OpenCircuitException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
