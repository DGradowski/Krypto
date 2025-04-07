//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Model.DES;

namespace Model
{
    public class DESX
    {
        private DES des;
        private byte[] keyFirst, keySecond, msg;
        private byte[][] msgPackage;


        public DESX()
        {
            des = new DES();
            msg = des.getMsg();
            generateRandomKeys();
        }

        public void generateDefaultKeys()
        {
            keyFirst = Encoding.ASCII.GetBytes("qwertyui");
            keySecond = Encoding.ASCII.GetBytes("asdfghjk");
        }

        public void generateRandomKeys()
        {
            keyFirst = des.generateRandomBytes(8);
            keySecond = des.generateRandomBytes(8);
        }
        public void setKeys(byte[] keyX1, byte[] desKey, byte[] keyX2)
        {
            this.keyFirst = keyX1;
            this.keySecond = keyX2;

            des.setKey(desKey);
        }
        public byte[] getKeyFirst()
        {
            return keyFirst;
        }

        public byte[] getKeySecond()
        {
            return keySecond;
        }

        public byte[] getMsg()
        {
            return msg;
        }

        public void setMsg(string msg)
        {
            des.setMsg(msg);
            this.msg = des.getMsg();
        }
        public void setMsg(byte[] msg)
        {
            this.msg = (byte[])msg.Clone();
            des.setMsg(msg);
        }

        public void prepareMsgPackages()
        {
            int fullBlocks = msg.Length / 8;
            int remainder = msg.Length % 8;
            int totalBlocks = fullBlocks + (remainder > 0 ? 1 : 0);

            msgPackage = new byte[totalBlocks][];

            for (int i = 0; i < fullBlocks; i++)
            {
                byte[] block = new byte[8];
                Array.Copy(msg, i * 8, block, 0, 8);
                msgPackage[i] = block;
            }

            if (remainder > 0)
            {
                byte[] lastBlock = new byte[8];
                Array.Copy(msg, fullBlocks * 8, lastBlock, 0, remainder);

                for (int i = remainder; i < 8; i++)
                {
                    lastBlock[i] = 0;
                }
                msgPackage[fullBlocks] = lastBlock;
            }
        }

        public void concatMsgPackages()
        {
            int totalLength = msgPackage.Length * 8;
            if (msg.Length % 8 != 0)
            {
                totalLength = (msgPackage.Length - 1) * 8 + (msg.Length % 8);
            }

            msg = new byte[totalLength];

            for (int i = 0; i < msgPackage.Length; i++)
            {
                int bytesToCopy = (i == msgPackage.Length - 1 && msg.Length % 8 != 0) ?
                                  msg.Length % 8 : 8;
                Array.Copy(msgPackage[i], 0, msg, i * 8, bytesToCopy);
            }
        }

        public void encrypt()
        {
            for (int i = 0; i < msgPackage.Length; i++)
            {
                msgPackage[i] = des.doXORBytes(msgPackage[i], keyFirst);
                des.setMsg(msgPackage[i]);
                des.run(true);
                msgPackage[i] = des.getMsg();
                msgPackage[i] = des.doXORBytes(msgPackage[i], keySecond);
            }
        }

        public void decrypt()
        {
            for (int i = 0; i < msgPackage.Length; i++)
            {
                msgPackage[i] = des.doXORBytes(msgPackage[i], keySecond);
                des.setMsg(msgPackage[i]);
                des.run(false);
                msgPackage[i] = des.getMsg();
                msgPackage[i] = des.doXORBytes(msgPackage[i], keyFirst);
            }
        }

        public void run(bool ifEncrypt)
        {
            prepareMsgPackages();
            if (ifEncrypt)
            {
                encrypt();
            }
            else
            {
                decrypt();
            }
            concatMsgPackages();
        }


    }
}