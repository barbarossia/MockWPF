using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn.Common.ExceptionHandling
{
    /// <summary>
    /// Throw this exception for user-correctable error conditions, with a user-centric message. E.g. "Validation errors exist! Correct validation errors before compiling workflow."
    /// </summary>
    [Serializable]
    public class UserFacingException : Exception
    {
        public UserFacingException(string msg)
            : base(msg)
        {

        }

        public UserFacingException(string msgFmt, params string[] msgParams)
            : base(string.Format(msgFmt, msgParams))
        {

        }

        public UserFacingException(string msg, Exception inner)
            : base(msg, inner)
        {

        }
    }
}
