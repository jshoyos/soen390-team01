using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

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
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException();

            byte[] edata;

            using (Rijndael r = Rijndael.Create())
            {
                r.Key = _key;
                r.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, r.CreateEncryptor(r.Key, r.IV), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                        edata = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(edata);
        }

        public string Decrypt(string cText, byte[] iv)
        {
            if (cText == null || cText.Length <= 0) throw new ArgumentNullException();

            string dData;
            byte[] cTextBytes = Convert.FromBase64String(cText);

            using (Rijndael r = Rijndael.Create())
            {
                r.Key = _key;
                r.IV = iv;

                using (MemoryStream ms = new MemoryStream(cTextBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, r.CreateDecryptor(r.Key, r.IV), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            dData = sr.ReadToEnd();
                        }
                    }
                }
            }

            return dData;
        }
    }
}
