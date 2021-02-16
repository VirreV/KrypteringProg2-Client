using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace KrypteringProg2_Client
{
    class Encrypter : Program
    {
        private int[] key = { 1, 3, 3, 7, 4, 2, 0 };

        public string Encrypt(string str){
            byte[] AsciiStr = new byte[str.Length];
            AsciiStr = Encoding.ASCII.GetBytes(str);
            int i = 0;
            foreach(byte b in AsciiStr){
                try{
                    int temp = Convert.ToInt32(b) + key[i%key.Length];
                    AsciiStr[i] = Convert.ToByte(temp);
                    i++;
                }catch{
                    System.Console.WriteLine(i);
                }
            }
            return System.Text.Encoding.ASCII.GetString(AsciiStr);
        }

        public string Decrypt(string str){
            byte[] AsciiStr = new byte[str.Length];
            AsciiStr = Encoding.ASCII.GetBytes(str);
            int i = 0;
            foreach(byte b in AsciiStr){
                int temp = Convert.ToInt32(b) - key[i%key.Length];
                AsciiStr[i] = Convert.ToByte(temp);
                i++;
            }
            return System.Text.Encoding.ASCII.GetString(AsciiStr);
        }
    }
}