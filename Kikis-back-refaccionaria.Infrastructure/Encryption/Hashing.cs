using System.Security.Cryptography;
using System.Text;

namespace Kikis_back_refaccionaria.Infrastructure.Encryption {
    public class Hashing {

        public static string EncodeSHA256(string encode) {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[]? stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(encode));
            for(int i = 0; i < stream.Length; i++)
                sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
