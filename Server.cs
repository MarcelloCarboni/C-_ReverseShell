using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // CHANGE THIS
            int PORT = 1234;
            int BUFFER_SIZE = 2 * 1024;
            
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, PORT);

            Socket serverSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipe);
            serverSocket.Listen(5);

            Console.WriteLine("Server listening on port {0}", PORT);

            Socket clientSocket = serverSocket.Accept();

            Console.WriteLine("[+] New connection from {0}", clientSocket.LocalEndPoint);

            string msg;
            byte[] buffer = new byte[BUFFER_SIZE];
            do
            {
                Console.Write("$: ");
                msg = Console.ReadLine();

                clientSocket.Send(Encoding.ASCII.GetBytes(msg));

                Array.Clear(buffer, 0, BUFFER_SIZE);
                clientSocket.Receive(buffer);

                Console.WriteLine("{0}", Encoding.ASCII.GetString(buffer).TrimEnd('\0'));
            } while (msg != "quit");

            clientSocket.Close();
            serverSocket.Close();
        }
    }
}
