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
            // Arrange
            DESX desx = new DESX();
            string originalMessage = "abcdefgh"; // Wiadomość musi mieć 8 znaków (64 bity)

            desx.setMsg(originalMessage);
            desx.prepareMsgPackages();

            // Act: szyfrowanie
            desx.run(true);
            byte[] encryptedMsg = desx.getMsg();

            // Aby przeprowadzić dezaszyfrowanie, przekonwertujemy wynik szyfrowania do stringa
            // (pamiętaj – to uproszczone rozwiązanie, ponieważ encryptedMsg może zawierać bajty
            // nieodpowiadające reprezentacji ASCII)
            string encryptedMsgAsString = Encoding.ASCII.GetString(encryptedMsg);
            desx.setMsg(encryptedMsgAsString);
            desx.prepareMsgPackages();

            // Act: deszyfrowanie
            desx.run(false);
            byte[] decryptedMsg = desx.getMsg();

            // Assert
            byte[] expectedDecryptedMsg = Encoding.ASCII.GetBytes(originalMessage);
        Assert.Equal(expectedDecryptedMsg, decryptedMsg);
        }
    }
}