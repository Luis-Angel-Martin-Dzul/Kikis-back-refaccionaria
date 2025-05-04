using System.Text;

namespace Kikis_back_refaccionaria.Infrastructure.Utilities {
    public class Util {


        public static string Generator(int length) {

            string validChars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for(int i = 0; i < length; i++) {
                int index = random.Next(validChars.Length);

                password.Append(validChars[index]);
            }

            return password.ToString();
        }

    }
}
