using KOMTEK.KundeInnsyn.Common.Exceptions.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace KOMTEK.KundeInnsyn.Common.Services
{
    public class XSRFtoken
    {
        private DateTimeWrapper DateTime { get; set; }
        public byte[] TokenBytes { get; private set; }
        public Guid TokenID { get; set; }
        public DateTime GeneratedTime { get; set; }
        private TimeSpan _timeout;
        public DateTime TimeoutTime => GeneratedTime.Add(_timeout);
        private bool _isValid;
        public bool IsValid 
        { 
            get 
            {
                if (TimeoutTime < DateTime.Now) return false;
                return _isValid;
            } private set 
            {
                _isValid = value;
            } 
        }
        public bool SingleUse { get; private set; }

        private byte[] GenerateToken(int length = 24)
        {
            if (length <= 0) throw new InvalidArgumentException("Inavalid input argument for parameter 'length'. Must the non zero and positive");
            if (length < 24) throw new InvalidArgumentException($"Given parameter 'length' of '{length}' is too short! Use a minimum 24bit key lenhgth.");
            var buffer = new byte[length];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        
        public XSRFtoken(int timeoutSeconds, bool singleUse = false, DateTimeWrapper dateTime = null)
        {
            DateTime = dateTime;
            if (timeoutSeconds <= 0) throw new InvalidArgumentException("Invalid input parameter 'timeoutSeconds', generated token must have a timeout date in the future.");
            TokenBytes = GenerateToken();
            TokenID = Guid.NewGuid();
            GeneratedTime = DateTime.Now;
            SingleUse = singleUse;
            _timeout = TimeSpan.FromSeconds(timeoutSeconds);
            IsValid = true;
        }

        public XSRFtoken(int timeoutSeconds, bool singleUse = false)
        {
            DateTime = new DateTimeWrapper();
            if (timeoutSeconds <= 0) throw new InvalidArgumentException("Invalid input parameter 'timeoutSeconds', generated token must have a timeout date in the future.");
            TokenBytes = GenerateToken();
            TokenID = Guid.NewGuid();
            GeneratedTime = DateTime.Now;
            SingleUse = singleUse;
            _timeout = TimeSpan.FromSeconds(timeoutSeconds);
            IsValid = true;
        }

        public void Invalidate() => IsValid = false;

    }
}
