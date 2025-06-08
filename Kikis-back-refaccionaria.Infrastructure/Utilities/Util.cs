using Kikis_back_refaccionaria.Core.Request;
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
        public static string SaveBase64File(ProductREQ request, string relativePath) {
            try {
                if(string.IsNullOrEmpty(request.imgB64)) return String.Empty;

                // Limpia el path relativo para evitar errores en Path.Combine
                relativePath = relativePath.TrimStart('/', '\\');

                var extension = Path.GetExtension(request.imgName);
                var newFileName = $"{request.Barcode}{extension}";

                var base64Data = request.imgB64;
                if(base64Data.Contains(","))
                    base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);

                var fileBytes = Convert.FromBase64String(base64Data);

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                if(!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                var filePath = Path.Combine(fullPath, newFileName);
                File.WriteAllBytes(filePath, fileBytes);

                // Para devolver una URL relativa usable en frontend
                return Path.Combine(relativePath, newFileName).Replace("\\", "/");
            }
            catch(Exception ex) {
                throw new Exception("Error al guardar archivo base64: " + ex.Message);
            }
        }

    }
}
