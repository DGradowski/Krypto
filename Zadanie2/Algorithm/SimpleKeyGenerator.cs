//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

namespace Algorithm
{
    public class SimpleKeyGenerator
    {
        public int keySize = 8;
        public long multiplier { get; set; }
        public long modulus { get; set; }

        public SimpleKeyGenerator()
        {
            multiplier = 4571L;
            modulus = 7187L;
        }

        public SimpleKeyGenerator(long multiplier, long modulus)
        {
            this.multiplier = multiplier;
            this.modulus = modulus;
        }

        public long[] generateDefaultPrivateKey(int KeySize)
        {
            long[] newPrivateKey = new long[keySize];
            Random rand = new Random();
            long sum = 0;

            for (int i = 0; i < keySize; i++)
            {
                // Każdy kolejny element większy niż suma wszystkich poprzednich
                long next = sum + rand.Next(1, 10); // Możesz zmienić 10 na większą wartość dla większych odstępów
                newPrivateKey[i] = next;
                sum += next;
            }

            return newPrivateKey;
        }

        public long[] generatePublicKey(long[] privateKey)
        {
            long[] publicKey = new long[privateKey.Length];
            for (int i = 0; i < privateKey.Length; i++)
            {
                publicKey[i] = (privateKey[i] * multiplier) % modulus;
            }
            return publicKey;
        }
    }
}
