using Xunit;
using Model;
using Newtonsoft.Json.Bson;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Text;

namespace ModelTest
{
    public class DESUnitTest
    {
        [Fact]
        public void checkBitTest()
        {
            var des = new DES();
            byte[] data = new byte[] { 0b10100000 };
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
            byte[] input2 = new byte[] { 0b10101010, 0b11001100 };
            int index = 4;
            int length = 6;
            byte[] output2 = des.splitBytes(input2, index, length);
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
            byte[] byte1 = new byte[] { 0b00000001, 0b10101010, 0b01010101, 0b00001111 }; 
            byte[] byte2 = new byte[] { 0b11110000, 0b11111111 };

            byte[] output = des.concatenateBytes(byte1, 32, byte2, 16);
            byte[] expected = new byte[] { 0b00000001, 0b10101010, 0b01010101, 0b00001111, 0b11110000, 0b11111111 };

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

        [Fact]
        public void LeftShiftTest()
        {
            DES des = new DES();
            byte[] input = new byte[] { 0b11001010, 0b11110001, 0b01010010, 0b11010000 };
            int shift = 3;
            
            byte[] output = des.leftShift(input, shift);
            byte[] expected = new byte[] { 0b01010111, 0b10001010, 0b10010110, 0b11100000 };

            Assert.Equal(expected, output);
        }

        [Fact]
        public void doXORBytesTest() {
            DES des = new DES();
            byte[] input1 = new byte[] { 0b11001010, 0b11110001, 0b01010010, 0b11010000 };
            byte[] input2 = new byte[] { 0b10101010, 0b11110001, 0b01010010, 0b11010000 };

            byte[] output = des.doXORBytes(input1, input2);
            byte[] expected = new byte[] { 0b01100000, 0b00000000, 0b00000000, 0b00000000 };

            Assert.Equal(expected, output);
        }

        [Fact]
        public void CreateByteFromSBoxTest()
        {
            DES des = new DES();
            byte result = des.createByteFromSBox(3, 10);
            Assert.Equal((byte)0x3A, result);
        }

        [Fact]
        public void MakeSBoxTest()
        {
            DES des = new DES();
            byte[] input = new byte[] { 0b00111111, 0b00001010, 0b11000101, 0b01110111, 0b11100001, 0b01011100 };

            byte[] output = des.makeSBox(input);
            byte[] expected = new byte[] { 0b00010101, 0b10011011, 0b10000110, 0b10111100 };

            Assert.Equal(expected, output);
        }

        [Fact]
        public void MakeProperMsgLength()
        {
            var des = new DES();

            string msg1 = "abcdefgh"; 
            string expected1 = "abcdefgh";
    
            string msg2 = "abc"; 
            string expected2 = "abc     "; 

            Assert.Equal(expected1, des.makeProperMsgLength(msg1)); // Already 8
            Assert.Equal(expected2, des.makeProperMsgLength(msg2)); // Padded to 8
        }

        [Fact]
        public void GetMsg()
        {
            var des = new DES();
            string testMessage = "abcdefgh";
            des.setMsg(testMessage); 

            byte[] expectedMsg = new byte[] { 97, 98, 99, 100, 101, 102, 103, 104 };

            byte[] result = des.getMsg();

            Assert.Equal(expectedMsg, result);
        }

        [Fact]
        public void GetKey()
        {
            var des = new DES();
            byte[] expectedKey = new byte[] { 49, 50, 51, 52, 53, 54, 55, 56 };

            byte[] result = des.getKey();

            Assert.Equal(expectedKey, result); 
        }

        [Fact]
        public void EncriptionDecriptionTest()
        {
            var des = new DES();
            des.setMsg("abcdefgh"); 


            byte[] expectedEncryptedMsg = new byte[] {
            0x94, 0xd4, 0x43, 0x6b, 0xc3, 0xb5, 0xb6, 0x93
            };

            byte[] expectedDecryptedMsg = Encoding.ASCII.GetBytes("abcdefgh");

            des.run(true); 
            byte[] encryptedMsg = des.getMsg();  

            Assert.Equal(expectedEncryptedMsg, encryptedMsg);

            des.setMsg(encryptedMsg);  
            des.run(false);  
            byte[] decryptedMsg = des.getMsg();  

            Assert.Equal(expectedDecryptedMsg, decryptedMsg);
        }


    }
}