using System;
using System.Collections.Generic;
using System.Linq;
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
            generateDefaultKeys();
        }

        public void generateDefaultKeys()
        {
            keyFirst = Encoding.ASCII.GetBytes("qwertyui");
            keySecond = Encoding.ASCII.GetBytes("asdfghjk");
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

        public void prepareMsgPackages()
        {
            int msgPartsNumb = ((msg.Length - 1) / 8) + 1;
            msgPackage = new byte[msgPartsNumb][];
            for (int i = 0; i < msgPartsNumb; i++)
            {
                msgPackage[i] = des.splitBytes(msg, i * 8, 64);
            }
        }

        public void concatMsgPackages()
        {
            msg = msgPackage[0];
            if(msgPackage.Length > 1)
            {
                for (int i = 1; i < msgPackage.Length; i++)
                {
                    msg = des.concatenateBytes(msg, msg.Length,msgPackage[i], msgPackage[i].Length*8);
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
                msgPackage[i] = des.doXORBytes(keySecond, msgPackage[i]);
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
                msgPackage[i] = des.doXORBytes(keyFirst, msgPackage[i]);
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