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

            while (binary.Length % blockSize != 0)
                binary += "0";

            StringBuilder cipher = new StringBuilder();
            for (int i = 0; i < binary.Length; i += blockSize)
            {
                int total = 0;
                for (int j = 0; j < blockSize; j++)
                {
                    int bit = binary[i + j] == '1' ? 1 : 0;
                    total += bit * (int)publicKey[j];
                }

                if (cipher.Length > 0) cipher.Append(",");
                cipher.Append(total);
            }

            return cipher.ToString();
        }

        public string Decrypt(string cipher)
        {
            string[] parts = cipher.Split(',');
            StringBuilder bits = new StringBuilder();
            long inverse = calculateMultiplierModuloInverse();

            foreach (var part in parts)
            {
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
            {
                binary.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return binary.ToString();
        }

        private string ConvertFromBinary(string bits)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i + 8 <= bits.Length; i += 8)
            {
                string byteStr = bits.Substring(i, 8);
                int ascii = Convert.ToInt32(byteStr, 2);
                text.Append((char)ascii);
            }
            return text.ToString();
        }

        private string DecryptBits(long value)
        {
            char[] bits = new char[blockSize];
            for (int i = blockSize - 1; i >= 0; i--)
            {
                if (value >= privateKey[i])
                {
                    bits[i] = '1';
                    value -= privateKey[i];
                }
                else
                {
                    bits[i] = '0';
                }
            }
            return new string(bits);
        }

        private long calculateMultiplierModuloInverse()
        {
            long[] result = extendedEuclid(keyGen.multiplier, keyGen.modulus);
            long inverse = result[0];
            return (inverse % keyGen.modulus + keyGen.modulus) % keyGen.modulus;
        }

        private long[] extendedEuclid(long a, long b)
        {
            if (b == 0) return new long[] { 1, 0 };
            long[] vals = extendedEuclid(b, a % b);
            return new long[] { vals[1], vals[0] - (a / b) * vals[1] };
        }
    }
}