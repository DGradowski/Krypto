//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

using System;
using System.Text;

namespace Algorithm
{
    public class MHCipher
    {
        private SimpleKeyGenerator keyGen;
        private long[] privateKey;
        private long[] publicKey;
        private int blockSize;

        public MHCipher(SimpleKeyGenerator keyGen, long[] privateKey)
        {
            this.keyGen = keyGen;
            this.privateKey = privateKey;
            this.publicKey = keyGen.generatePublicKey(privateKey);
            this.blockSize = privateKey.Length;
        }

        public string Encrypt(string message)
        {
            string binary = ConvertToBinary(message);
            while (binary.Length % blockSize != 0) binary += "0";

            StringBuilder cipher = new StringBuilder();
            for (int i = 0; i < binary.Length; i += blockSize)
            {
                long total = 0;
                for (int j = 0; j < blockSize; j++)
                    total += (binary[i + j] == '1' ? 1 : 0) * publicKey[j];

                if (cipher.Length > 0) cipher.Append(",");
                cipher.Append(total.ToString());
            }
            return cipher.ToString();
        }

        public string Decrypt(string cipher)
        {
            // Usuwamy wszystkie spacje przed przetwarzaniem
            string cipherWithoutSpaces = cipher.Replace(" ", "");
            int partLength = publicKey.Length.ToString().Length;
            StringBuilder bits = new StringBuilder();
            long inverse = calculateMultiplierModuloInverse();

            for (int i = 0; i < cipherWithoutSpaces.Length; i += partLength)
            {
                string part = cipherWithoutSpaces.Substring(i, Math.Min(partLength, cipherWithoutSpaces.Length - i));
                long c = long.Parse(part);
                long value = (c * inverse) % keyGen.modulus;
                bits.Append(DecryptBits(value));
            }
            return ConvertFromBinary(bits.ToString());
        }

        private string ConvertToBinary(string message)
        {
            StringBuilder binary = new StringBuilder();
            foreach (char c in message)
                binary.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            return binary.ToString();
        }

        private string ConvertFromBinary(string bits)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i + 8 <= bits.Length; i += 8)
            {
                text.Append((char)Convert.ToInt32(bits.Substring(i, 8), 2));
            }
            return text.ToString();
        }

        private string DecryptBits(long value)
        {
            char[] bits = new char[blockSize];
            for (int i = blockSize - 1; i >= 0; i--)
            {
                bits[i] = value >= privateKey[i] ? '1' : '0';
                if (bits[i] == '1') value -= privateKey[i];
            }
            return new string(bits);
        }

        private long calculateMultiplierModuloInverse()
        {
            long[] result = extendedEuclid(keyGen.multiplier, keyGen.modulus);
            return (result[0] % keyGen.modulus + keyGen.modulus) % keyGen.modulus;
        }

        private long[] extendedEuclid(long a, long b)
        {
            if (b == 0) return new long[] { 1, 0 };
            long[] vals = extendedEuclid(b, a % b);
            return new long[] { vals[1], vals[0] - (a / b) * vals[1] };
        }
    }
}