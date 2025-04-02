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

        public byte[] generateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public void generateRandomKeys()
        {
            // Dla DESX potrzebujemy dwóch 8-bajtowych kluczy
            keyFirst = generateRandomBytes(8);
            keySecond = generateRandomBytes(8);
        }
        public void setKeys(byte[] keyX1, byte[] desKey, byte[] keyX2)
        {
            // Ustawienie dodatkowych kluczy DESX
            this.keyFirst = keyX1;
            this.keySecond = keyX2;

            // Ustawienie klucza DES
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
            // Zakładamy, że wewnętrzny obiekt DES posiada metodę setMsg(byte[])
            des.setMsg(msg);
            this.msg = des.getMsg();
        }

        public void prepareMsgPackages()
        {
            int msgPartsNumb = ((msg.Length - 1) / 8) + 1;
            msgPackage = new byte[msgPartsNumb][];
            for (int i = 0; i < msgPartsNumb; i++)
            {
                msgPackage[i] = des.splitBytes(msg, i * 8*8, 64);
            }
        }

        public void concatMsgPackages()
        {
            msg = msgPackage[0];
            if(msgPackage.Length > 1)
            {
                for (int i = 1; i < msgPackage.Length; i++)
                {
                    msg = des.concatenateBytes(msg, msg.Length*8,msgPackage[i], msgPackage[i].Length*8);
                }
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