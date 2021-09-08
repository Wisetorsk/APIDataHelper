using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOMTEK.KundeInnsyn.Common.Services
{
    public static class TokenHandler
    {
        public static bool ValidateToken(XSRFtoken incomingToken, XSRFtoken localToken)
        {
            bool tokenState = false;
            if (incomingToken.TokenID == localToken.TokenID && 
                localToken.IsValid && 
                incomingToken.IsValid &&
                incomingToken.TimeoutTime < DateTime.Now)
            {
               tokenState = localToken.TokenBytes.SequenceEqual(incomingToken.TokenBytes);
                if (incomingToken.SingleUse)
                {
                    localToken.Invalidate();
                    incomingToken.Invalidate();
                }

            }
            return tokenState;
        }

        public static bool ValidateToken(byte[] incomingToken, XSRFtoken localToken)
        {
            bool tokenState = false;
            if (localToken.IsValid &&
                localToken.TimeoutTime < DateTime.Now)
            {
                tokenState = localToken.TokenBytes.SequenceEqual(incomingToken);
                if (localToken.SingleUse)
                {
                    localToken.Invalidate();
                }
            }
            return tokenState;
        }

        public static XSRFtoken Generate(int timeout, bool singleUse)
        {
            return new XSRFtoken(timeout, singleUse);
        }
    }
}
