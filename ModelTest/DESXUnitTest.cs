//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

using Xunit;
using Model;
using Newtonsoft.Json.Bson;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DESXTest
{
    public class DESUnitTest
    {
        [Fact]
        public void EncryptionDecryptionTest()
        {
            // Arrange: Oryginalna wiadomość musi mieć 8 znaków (64 bity)
            DESX desx = new DESX();
            string originalMessage = "abcdefgh";

            // Ustawienie wiadomości jako tekst
            desx.setMsg(originalMessage);
            desx.prepareMsgPackages();

            // Act: szyfrowanie
            desx.run(true);
            byte[] encryptedMsg = desx.getMsg();

            // Aby odszyfrowywać, ustawiamy wiadomość bezpośrednio jako tablicę bajtów,
            // dzięki czemu nie dochodzi do utraty danych przy konwersji.
            desx.setMsg(encryptedMsg); // Korzystamy z przeciążenia przyjmującego byte[]
            desx.prepareMsgPackages();

            // Act: deszyfrowanie
            desx.run(false);
            byte[] decryptedMsg = desx.getMsg();

            // Assert: porównujemy bajtowo oryginalną wiadomość z odszyfrowanym wynikiem
            byte[] expectedDecryptedMsg = Encoding.ASCII.GetBytes(originalMessage);
            Assert.Equal(expectedDecryptedMsg, decryptedMsg);
        }
    }
}