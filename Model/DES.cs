using System;
using System.Collections.Generic;
using System.Linq;
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
            //generateDefaultMessageAndKey();
            //generateSubKeys();
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
        public void setBit(byte[] data, int index,bool bit)
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
                    bool bit = checkBit(input,(6*i) + j);
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
        public byte[] concatenateBytes(byte[] byteA, byte[] byteB)
        {
            int aBits = byteA.Length * 8;
            int bBits = byteB.Length * 8;
            byte[] output = prepareOutput(byteA.Length, byteB.Length);

            for (int i = 0; i < aBits; i++)
            {
                bool bit = checkBit(byteA, i);
                setBit(output, i, bit);
            }
            for (int i = 0; i < bBits; i++)
            {
                bool bit = checkBit(byteB, i);
                setBit(output, aBits + i, bit);
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
    }
}
