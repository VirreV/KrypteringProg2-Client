using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace KrypteringProg2_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true){
                //Etablerar variabler
                bool connected = false;
                string adress;
                int port = 8001;

                //Skapa TcpClient-objekt
                TcpClient tcpClient = new TcpClient();

                //Ansluta till server loop.
                do
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                    //Mata in server IP
                    Console.Write("Server IP: ");
                    adress = Console.ReadLine();

                    Console.Clear();

                    //Anslut till server
                    bool connecting = true;
                    while (connecting)
                    {
                        try
                        {
                            Console.WriteLine("Connecting...");
                            tcpClient.Connect(adress, port);
                            connected = true;
                            connecting = false;
                        }
                        catch
                        {
                            Console.WriteLine("Connection Failed");
                            System.Console.WriteLine("Try same IP again? [y/n]");
                            
                            //Om clienten vill försöka med annan IP
                            if(Console.ReadLine().ToLower() != "y"){
                                connecting = false;
                            }//Annars kör om loopen och försöker ansluta med samma IP igen
                        }
                    }
                }while(!connected);

                Console.Write("Connected successfully");
                for(int i = 0; i < 3; i++){
                    Thread.Sleep(350);
                    Console.Write(".");
                }
                Console.Clear();

                ChangeUserName(tcpClient);

                bool disconnect = false;
                while(!disconnect){
                    switch(Menu(new List<String>{"Send message", "Change username", "Show your messages", "Show all messages", "Disconnect"})){
                        case 0:
                            SendMessage(tcpClient);
                            break;
                        case 1:
                            ChangeUserName(tcpClient);
                            break;
                        case 2:
                            //ShowMyMsg();
                            break;
                        case 3:
                            ShowAllMsg(tcpClient);
                            break;
                        default:
                            disconnect = true;
                            Dc(tcpClient);
                            break;
                    }
                }
            }
        }
        static void Dc(TcpClient tcpClient){
            NetworkStream tcpStream;
            try{
                //Öppna NetworkStream till servern
                tcpStream = tcpClient.GetStream();
            }
            catch(Exception e){
                Error(e);
                return;
            }

            try{
                //Skicka byte-arrayen
                tcpStream.Write(Encoding.ASCII.GetBytes("disc"), 0, Encoding.ASCII.GetBytes("disc").Length);
            }catch(Exception e){Error(e);}
        }

        static void ShowAllMsg(TcpClient tcpClient){
            NetworkStream tcpStream;
            try{
                //Öppna NetworkStream till servern
                tcpStream = tcpClient.GetStream();
            }
            catch(Exception e){
                Error(e);
                return;
            }

            try{
                //Skicka byte-arrayen
                tcpStream.Write(Encoding.ASCII.GetBytes("sMes"), 0, Encoding.ASCII.GetBytes("sMes").Length);
            }catch(Exception e){Error(e);}
            string answer = Listen(tcpClient);
            Console.WriteLine(answer);
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        static void SendMessage(TcpClient tcpClient){
            NetworkStream tcpStream;
            try{
                //Öppna NetworkStream till servern
                tcpStream = tcpClient.GetStream();
            }
            catch(Exception e){
                Error(e);
                return;
            }
            string message = "";
            while(true){
                try{
                    Console.Write("Message: ");
                    //Mata in meddelande
                    message = Console.ReadLine();
                    break;
                }catch(Exception e){Error(e);}
            }

            //Etablera byteArray för att skicka till server
            byte[] bMessage = new byte[4 + message.Length];

            //Lägg till taggen nMes i början för att servern ska veta vad det är för något
            Encoding.ASCII.GetBytes("nMes").CopyTo(bMessage, 0);
            //Lägg till meddelandet
            Encoding.ASCII.GetBytes(message).CopyTo(bMessage, 4);

            try{
                //Skicka byte-arrayen
                tcpStream.Write(bMessage, 0, bMessage.Length);
            }catch(Exception e){Error(e);}
        }

        static void ChangeUserName(TcpClient tcpClient){
            NetworkStream tcpStream;
            try{
                //Öppna NetworkStream till servern
                tcpStream = tcpClient.GetStream();
            }
            catch(Exception e){
                Error(e);
                return;
            }
            string message = "";
            while(true){
                try{
                    Console.Write("User: ");
                    //Mata in namn
                    message = Console.ReadLine();
                    break;
                }catch(Exception e){Error(e);}
            }

            //Etablera byteArray för att skicka till server
            byte[] bMessage = new byte[4 + message.Length];

            //Lägg till taggen uStr i början för att servern ska veta vad det är för något
            Encoding.ASCII.GetBytes("uStr").CopyTo(bMessage, 0);
            //Lägg till namnet
            Encoding.ASCII.GetBytes(message).CopyTo(bMessage, 4);

            try{
                //Skicka byte-arrayen
                tcpStream.Write(bMessage, 0, bMessage.Length);
            }catch(Exception e){Error(e);}
        }

        static int Menu(List<string> options)
        {
            Console.Clear();
            int selected = 0;
            string arrow = " > ";
            bool activeMenu = true;

            int topLine = Console.CursorTop;
            Console.CursorVisible = false;

            do
            {
                Console.SetCursorPosition(0, topLine);
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selected) Console.Write(arrow);
                    else Console.Write("   ");

                    Console.WriteLine(options[i]);
                }

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                        activeMenu = false;
                        break;
                    case ConsoleKey.UpArrow:
                        selected--;
                        if (selected < 0) selected = options.Count - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selected++;
                        if (selected == options.Count) selected = 0;
                        break;
                    case ConsoleKey.Backspace:
                        return -1;
                    default:
                        break;
                }
            } while (activeMenu);

            Console.CursorVisible = true;
            return selected;
        }

        static string Listen(TcpClient tcpClient)
        {
            try
            {
                //Ta emot svar från server
                NetworkStream tcpStream = tcpClient.GetStream();
                byte[] bRead = new byte[256];
                int bReadSize = tcpStream.Read(bRead, 0, bRead.Length);

                //Konvertera till string
                string read = "";
                for (int i = 0; i < bReadSize; i++)
                {
                    read += Convert.ToChar(bRead[i]);
                }
                return read;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        
        static void Sender(TcpClient tcpClient)
        {
            while (true)
            {
                try
                {
                    NetworkStream tcpStream = tcpClient.GetStream();
                    //Skriv in meddelande att skicka till severn
                    string message = Console.ReadLine();
                    if (message.ToLower() == "disconnect" || message.ToLower() == "-d")
                    {
                        tcpStream.Close();
                        break;
                    }
                    Console.CursorTop -= 1;
                    Console.WriteLine("You: " + message);
                    byte[] bMessage = Encoding.ASCII.GetBytes(message);

                    //Skicka meddelande
                    tcpStream.Write(bMessage, 0, bMessage.Length);
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }
        }
        static void Error(Exception e)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + e.Message);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}