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

        public long[] generateDefaultPrivateKey()
        {
            long[] newPrivateKey = new long[keySize];
            long keyElement = 1;
            for (int i = 0; i < newPrivateKey.Length; i++)
            {
                keyElement *= 2;
                newPrivateKey[i] = keyElement;
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
