using KOMTEK.KundeInnsyn.Common.DataObjects;
using KOMTEK.KundeInnsyn.Common.Exceptions.CustomExceptions;
using Konscious.Security.Cryptography;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KOMTEK.KundeInnsyn.Common.Services
{
    public class Cryptography : ICryptography
    {
        public bool CalculationRunning { get; set; } = false;
        public event EventHandler<HashEventArgs> HashCalcualtionComplete;
        public event EventHandler<HashVerificationEventArgs> HashVerificationComplete;

        protected virtual void AlertCalculationComplete(UserHash user)
        {
            CalculationRunning = false;
            HashCalcualtionComplete?.Invoke(this, new HashEventArgs(user));
        }

        protected virtual void AlertHashVerificationComplete(bool result)
        {
            CalculationRunning = false;
            HashVerificationComplete?.Invoke(this, new HashVerificationEventArgs(result));
        }

        /// <summary>
        /// Generates cryptographic salt as byte array
        /// </summary>
        /// <returns></returns>
        public byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Calculates Argon_2 hash for user input and salt
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public byte[] HashInput(string inputString, byte[] salt)
        {
            CalculationRunning = true;
            byte[] hashedBytes;
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(inputString)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 4; // Number of threads to utilize
                argon2.Iterations = 4;
                argon2.MemorySize = 256 * 256; // Set to a defualt of 1024^2 memory size... 
                hashedBytes = argon2.GetBytes(16);
            };
            return hashedBytes;
        }

        /// <summary>
        /// Verifyes the given password vs stored hash and salt.
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="salt"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool VerifyHash(string inputString, byte[] salt, byte[] hash)
        {
            var newHash = HashInput(inputString, salt);
            var result = hash.SequenceEqual(newHash);
            return result; // Can also be set as async Task or void.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="hashedUser"></param>
        /// <returns></returns>
        public bool VerifyHash(string inputString, UserHash hashedUser)
        {
            var newHash = HashInput(inputString, hashedUser.Salt);
            var result = hashedUser.Hash.SequenceEqual(newHash);
            return result; // Can also be set as async Task or void.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="salt"></param>
        /// <param name="hash"></param>
        public void VerifyHashEvent(string inputString, byte[] salt, byte[] hash)
        {
            var newHash = HashInput(inputString, salt);
            var result = hash.SequenceEqual(newHash);
            AlertHashVerificationComplete(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="hashedUser"></param>
        public void VerifyHashEvent(string inputString, UserHash hashedUser)
        {
            var newHash = HashInput(inputString, hashedUser.Salt);
            var result = hashedUser.Hash.SequenceEqual(newHash);
            AlertHashVerificationComplete(result);
        }


        /// <summary>
        /// Creates dataset to store userlogin
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public void CreateHashAndSalt(string username, string password)
        {
            var salt = CreateSalt();
            var hash = HashInput(password, salt);
            var user = new UserHash
            {
                Salt = salt,
                Username = username,
                Hash = hash
            };
            AlertCalculationComplete(user);
        }

        public byte[] Encrypt(string inputString)
        {
            throw new NotImplementedException();
        }

        public byte[] Decrypt(string inputString, byte[] key)
        {
            throw new NotImplementedException();
        }

        public byte[] RandomBytes(int length, bool secure = false)
        {
            if (length <= 0) throw new InvalidArgumentException("Inavalid input argument for parameter 'length'. Must the non zero and positive");
            if (length < 24) throw new InvalidArgumentException($"Given parameter 'length' of '{length}' is too short! Use a minimum 24bit key lenhgth.");
            var buffer = new byte[length];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }        
    }
}
