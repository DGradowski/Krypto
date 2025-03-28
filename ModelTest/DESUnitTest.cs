using Xunit;
using Model;
using Newtonsoft.Json.Bson;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace ModelTest
{
    public class DESUnitTest
    {
        [Fact]
        public void checkBitTest()
        {
            var des = new DES();
            byte[] data = new byte[] {0b10100000};
            bool result = des.checkBit(data, 0);
            Assert.True(result);
            bool result2 = des.checkBit(data, 1);
            Assert.False(result2);
        }

        [Fact]
        public void setBitTest()
        {
            var des = new DES();
            byte[] data = new byte[] { 0b10100000 };
            des.setBit(data, 2, false);
            Assert.Equal(0b10000000, data[0]);
            des.setBit(data, 1, true);
            Assert.Equal(0b11000000, data[0]);
        }

        [Fact]
        public void splitBytesTest()
        {
            var des = new DES();
            byte[] input1 = new byte[] { 0b01010101, 0b10101010, 0b00110011, 0b11001100, 0b00001111, 0b11110000 };
            byte[] output1 = des.splitBytes(input1);
            byte[] expected1 = new byte[] { 0b01010100, 0b01101000, 0b10100000, 0b11001100, 0b11001100, 0b00000000, 0b11111100, 0b11000000 };
            Assert.Equal(expected1, output1);
            byte[] input2 = new byte[] { 0b10101010, 0b11001100};
            int index = 4;
            int length = 6;
            byte[] output2 = des.splitBytes(input2,index,length);
            byte[] expected2 = new byte[] { 0b10101100 };
            Assert.Equal(expected2, output2);
        }

        [Fact]
        public void prepareOutputTest()
        {
            var des = new DES();
            byte[] output1 = des.prepareOutput(8, 7);
            byte[] expected1 = new byte[15];
            Assert.Equal(expected1, output1);
            byte[] output2 = des.prepareOutput(9);
            byte[] expected2 = new byte[2];
            Assert.Equal(expected2, output2);
        }

        [Fact]
        public void concatenateBytesTest()
        {
            var des = new DES();
            byte[] byte1 = new byte[] {0x01, 0xAA, 0xB1, 0xDC};
            byte[] byte2 = new byte[] {0x02, 0x55};
            byte[] output = des.concatenateBytes(byte1,byte2);
            byte[] expected = new byte[] { 0x01, 0xAA, 0xB1, 0xDC, 0x02, 0x55};
            Assert.Equal(expected, output);
        }

        [Fact]
        public void shuffleBitsTest()
        {
            var des = new DES();
            byte[] input = new byte[] { 0b10101010 };
            int[] permTable = new int[] { 1, 3, 5, 7, 2, 4, 6, 8 };
            byte[] output = des.shuffleBits(input, permTable);
            byte[] expected = new byte[] { 0b11110000 };
            Assert.Equal(expected, output);
        }
    }
}