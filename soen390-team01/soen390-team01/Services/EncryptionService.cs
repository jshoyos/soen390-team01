#region Header

// Author: Tommy Andrews
// File: EncryptionService.cs
// Project: soen390-team01
// Created: 02/23/2021
// 

#endregion

using System;
using System.IO;
using System.Security.Cryptography;

namespace soen390_team01.Services
{
    public class EncryptionService
    {
        #region fields

        private readonly byte[] _key;

        #endregion

        public EncryptionService(string key)
        {
            _key = Convert.FromBase64String(key);
        }

        public string Encrypt(string text, byte[] iv)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException();
            }

            byte[] edata;

            using (var r = Rijndael.Create())
            {
                r.Key = _key;
                r.IV = iv;

                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, r.CreateEncryptor(r.Key, r.IV), CryptoStreamMode.Write);
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(text);
                }

                edata = ms.ToArray();
            }

            return Convert.ToBase64String(edata);
        }

        public string Decrypt(string cText, byte[] iv)
        {
            if (cText == null || cText.Length <= 0)
            {
                throw new ArgumentNullException();
            }

            string dData;
            var cTextBytes = Convert.FromBase64String(cText);

            using var r = Rijndael.Create();
            r.Key = _key;
            r.IV = iv;

            using var ms = new MemoryStream(cTextBytes);
            using var cs = new CryptoStream(ms, r.CreateDecryptor(r.Key, r.IV), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            dData = sr.ReadToEnd();

            return dData;
        }
    }
}