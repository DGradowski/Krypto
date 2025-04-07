//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Model.Values;

namespace Model
{
    public class DES
    {
        private Values v;
        private byte[] key, msg;
        private byte[][] subKeys;
        private byte[] leftSide, rightSide;
        public DES()
        {
            this.v = new Values();
            generateDefaultMessageAndKey();
            generateSubKeys();
            leftSide = null;
            rightSide = null;
        }

        //sprawdzanie czy dany bit jest ustawiony
        public bool checkBit(byte[] data, int index)
        {
            int whichByte = index / 8;
            int whichBit = index % 8;
            return (data[whichByte] >> (7 - whichBit) & 1) == 1;
        }

        //ustawianie bitu
        public void setBit(byte[] data, int index, bool bit)
        {
            int whichByte = index / 8;
            int whichBit = index % 8;
            if (bit)
            {
                data[whichByte] |= (byte)(0x80 >> whichBit);
            }
            else
            {
                data[whichByte] &= (byte)~(0x80 >> whichBit);
            }
        }

        //rozbicie 6 bajtów na 8 bajtów w konwencji MSB-First
        //0-5,6-11,12-17,18-23,24-29,30-35,36-41,42-47. Do każdego bajtu 2 zera na koniec
        public byte[] splitBytes(byte[] input)
        {
            int numOfBytes = 8;
            byte[] output = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    bool bit = checkBit(input, (6 * i) + j);
                    setBit(output, (8 * i) + j, bit);
                }
            }
            return output;
        }

        public byte[] splitBytes(byte[] input, int index, int length)
        {
            byte[] output = prepareOutput(length);
            for (int i = 0; i < length; i++)
            {
                bool bit = checkBit(input, index + i);
                setBit(output, i, bit);
            }
            return output;
        }

        //Przygotowanie tablic na output
        public byte[] prepareOutput(int aBytes, int bBytes)
        {
            int totalBits = (aBytes + bBytes) * 8;
            return new byte[(totalBits + 7) / 8];
        }

        public byte[] prepareOutput(int length)
        {
            int numberOfBytes = ((length - 1) / 8) + 1;
            return new byte[numberOfBytes];
        }

        //Konkatenacja 2 bajtów
        public byte[] concatenateBytes(byte[] byteA, int aLength, byte[] byteB, int bLength)
        {
            byte[] output = prepareOutput(aLength + bLength);  // Przygotowujemy tablicę wynikową

            int i = 0;
            // Przekopiowanie bitów z byteA
            for (; i < aLength; i++)
            {
                bool bit = checkBit(byteA, i);  // Sprawdzamy bit w byteA na indeksie i
                setBit(output, i, bit);         // Ustawiamy bit w wyniku
            }

            // Przekopiowanie bitów z byteB
            for (int j = 0; j < bLength; j++, i++)
            {
                bool bit = checkBit(byteB, j);  // Sprawdzamy bit w byteB na indeksie j
                setBit(output, i, bit);         // Ustawiamy bit w wyniku
            }

            return output;
        }

        //Przestawienie bitów w input według tabeli permTable
        public byte[] shuffleBits(byte[] input, int[] permTable)
        {
            byte[] output = prepareOutput(permTable.Length);
            for (int i = 0; i < permTable.Length; i++)
            {
                bool bit = checkBit(input, permTable[i] - 1);
                setBit(output, i, bit);
            }
            return output;
        }

        public byte[] leftShift(byte[] input, int numOfShifts)
        {
            byte[] output = new byte[4]; // 28 bitów to 4 bajty (z uwzględnieniem paddingu)
            int halfKeySize = 28;
            int bitsToShift = numOfShifts % halfKeySize;

            for (int i = 0; i < halfKeySize; i++)
            {
                bool bit = checkBit(input, (i + bitsToShift) % halfKeySize);
                setBit(output, i, bit);
            }

            return output;
        }

        public void generateSubKeys()
        {
            byte[] keyPC1 = shuffleBits(key, v.keyPermutation);

            byte[] c = splitBytes(keyPC1, 0, 28);
            byte[] d = splitBytes(keyPC1, 28, 28);

            subKeys = new byte[v.shifts.Length][];
            for (int i = 0; i < v.shifts.Length; i++)
            {
                c = leftShift(c, v.shifts[i]);
                d = leftShift(d, v.shifts[i]);
                byte[] cd = concatenateBytes(c, 28, d, 28);
                subKeys[i] = shuffleBits(cd, v.taperPermutation);
            }
            int x = key.Length;
        }

        public void initialPermutation()
        {
            msg = shuffleBits(msg, v.startPermutation);
        }

        public void composeOutput()
        {
            msg = concatenateBytes(rightSide, v.startPermutation.Length / 2, leftSide, v.startPermutation.Length / 2);
            msg = shuffleBits(msg, v.endPermutation);
        }

        public byte[] doXORBytes(byte[] a, byte[] b)
        {
            byte[] output = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                output[i] = (byte)(a[i] ^ b[i]);
            }
            return output;
        }

        public byte createByteFromSBox(int firstSBoxValue, int secondSBoxValue)
        {
            return (byte)(16 * firstSBoxValue + secondSBoxValue);
        }

        public byte[] makeSBox(byte[] input)
        {
            input = splitBytes(input);
            byte[] output = new byte[input.Length / 2];

            for (int i = 0, firstSBoxValue = 0; i < input.Length; i++)
            {
                byte sixBitsFragment = input[i];
                int rowNumb = 2 * (sixBitsFragment >> 7 & 0x0001) + (sixBitsFragment >> 2 & 0x0001);
                int colNumb = (sixBitsFragment >> 3) & 0x000F;
                int secondSBoxValue = v.sBox[64 * i + 16 * rowNumb + colNumb];
                if (i % 2 == 0)
                {
                    firstSBoxValue = secondSBoxValue;
                }
                else
                {
                    output[i / 2] = createByteFromSBox(firstSBoxValue, secondSBoxValue);
                }
            }
            return output;
        }

        public void performSBoxPBoxAndXOR()
        {
            rightSide = makeSBox(rightSide);
            rightSide = shuffleBits(rightSide, v.pBox);
            rightSide = doXORBytes(leftSide, rightSide);
        }

        public void proceedIterations(bool ifEncrypt)
        {
            int iteration = subKeys.Length;
            for (int i = 0; i < iteration; i++)
            {
                byte[] oldRightSide = rightSide;
                rightSide = shuffleBits(rightSide, v.expantionPermutation);
                if (ifEncrypt)
                {
                    rightSide = doXORBytes(rightSide, subKeys[i]);
                }
                else
                {
                    rightSide = doXORBytes(rightSide, subKeys[iteration - i - 1]);
                }
                performSBoxPBoxAndXOR();
                leftSide = oldRightSide;
            }
            composeOutput();
        }

        public void divideMsg()
        {
            int numOfBits = (msg.Length * 8) / 2;

            leftSide = splitBytes(msg, 0, numOfBits);
            rightSide = splitBytes(msg, numOfBits, numOfBits);
        }

        public void run(bool ifEncrypt)
        {
            initialPermutation();
            divideMsg();
            proceedIterations(ifEncrypt);
        }

        public string makeProperMsgLength(string msg)
        {
            int overflowBytesNumb = msg.Length % 8;
            if(overflowBytesNumb != 0)
            {
                for (int i = 0; i < 8 - overflowBytesNumb; i++)
                {
                    msg += " ";
                }
            }
            return msg;
        }

        public void setMsg(byte[] msg)
        {
            this.msg = msg;
        }

        public void setMsg(string msg)
        {
            msg = makeProperMsgLength(msg);
            this.msg = Encoding.ASCII.GetBytes(msg);

        }

        public void setKey(byte[] newKey)
        {
            this.key = newKey;
            generateSubKeys();
        }

        public byte[] getMsg()
        {
            return msg;
        }

        public byte[] getKey()
        {
            return key;
        }
        public byte[] generateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public void generateDefaultMessageAndKey()
        {
            key = generateRandomBytes(8);
            msg = Encoding.ASCII.GetBytes("abcdefgh");
        }
    }
}
