using System;
using System.Threading;

namespace Servidor
{
    class Program
    {
        static ServidorModel servidor;
        static Thread listenThread;

        static void Main(string[] args)
        {
            try
            {
                servidor = new ServidorModel();
                listenThread = new Thread(new ThreadStart(servidor.Listen));
                listenThread.Start();

            }
            catch (Exception exception)
            {

                servidor.Disconnect();
                Console.WriteLine(exception.Message);
            }
        }
    }
}
