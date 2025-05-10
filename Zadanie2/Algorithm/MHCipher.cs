using System.Text;

namespace Algorithm
{
    public class MHCipher
    {
        private SimpleKeyGenerator keyGen;
        private long[] privateKey { get; set; }
        private long[] publicKey { get; set; }

        public MHCipher(SimpleKeyGenerator keyGen)
        {
            this.keyGen = keyGen;
            generateKeys();
        }

        public void generateKeys()
        {
            privateKey = keyGen.generateDefaultPrivateKey();
            publicKey = keyGen.generatePublicKey(privateKey);
        }

        public void generatePublicKey()
        {
            publicKey = keyGen.generatePublicKey(privateKey);
        }

        public long[] Encrypt(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new Exception("Message is empty"); 

            long[] output = new long[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                output[i] = calculateEncryptedChar(message[i]);
            }

            return output;
        }

        public String decrypt(long[] cipher)
        {
            StringBuilder stringBuilder = new StringBuilder();
            long multiInverse = calculateMultiplierModuloInverse();

            foreach (long element in cipher)
            {
                long encryptedChar = (multiInverse * element) % keyGen.modulus;
                stringBuilder.Append(decryptChar(encryptedChar));
            }

            return stringBuilder.ToString();
        }

        public char decryptChar(long encryptedChar)
        {
            int MSB = 0x80;
            int outputChar = 0;
            for (int i = 7; i >= 0 && encryptedChar > 0; i--)
            {
                if (encryptedChar >= privateKey[i])
                {
                    encryptedChar -= privateKey[i];
                    outputChar += MSB;
                }
                MSB /= 2;
            }
            return (char)outputChar;
        }

        public long calculateMultiplierModuloInverse()
        {
            long[] response = extendedEuclid(keyGen.multiplier, keyGen.modulus);
            return response[0];
        }

        public long[] extendedEuclid(long c, long d) {
            if (d == 0)
            {
                return new long[] {1,0};
            }
            long[] response = extendedEuclid(d, c % d);
            long a = response[1];
            long b = response[0] - (c / d) * response[1];
            return new long[] {a,b};
        }

        public long calculateEncryptedChar(char x)
        {
            long output = 0;
            int val = x;
            for (int i = 0; i < 8; i++)
            {
                if ((val & 1) == 1)
                {
                    output += publicKey[i];
                }
                val >>= 1;
            }
            return output;
        }

    }
}
