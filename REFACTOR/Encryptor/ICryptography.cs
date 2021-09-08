using KOMTEK.KundeInnsyn.Common.DataObjects;
using System;

namespace KOMTEK.KundeInnsyn.Common.Services
{
    public interface ICryptography
    {
        bool CalculationRunning { get; set; }

        event EventHandler<HashEventArgs> HashCalcualtionComplete;
        event EventHandler<HashVerificationEventArgs> HashVerificationComplete;

        void CreateHashAndSalt(string username, string password);
        byte[] CreateSalt();
        byte[] Decrypt(string inputString, byte[] key);
        byte[] Encrypt(string inputString);
        byte[] HashInput(string inputString, byte[] salt);
        bool VerifyHash(string inputString, byte[] salt, byte[] hash);
        bool VerifyHash(string inputString, UserHash hashedUser);
        void VerifyHashEvent(string inputString, byte[] salt, byte[] hash);
        void VerifyHashEvent(string inputString, UserHash hashedUser);
        byte[] RandomBytes(int length, bool secure);
        
    }
}