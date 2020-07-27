using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Cliente
{
    class Program
    {
        static string nickName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            Console.WriteLine("*** Bem vindo ao nosso servidor de bate papo. Entre com um apelido:");
            nickName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //conexão do cliente
                stream = client.GetStream(); //

                string message = nickName;
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                //Inicia nova thread para receber dados
                Thread threadReceiver = new Thread(new ThreadStart(ReceiveMessage));
                threadReceiver.Start();
                Console.WriteLine($"*** Você está registrado como {nickName}.");
                SendMessage();

            }
            catch (Exception exception) 
            {

                Console.WriteLine(exception.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        /// <summary>
        /// Recebe Mensagens
        /// </summary>
        private static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];//Buffer para dados recebidos
                    StringBuilder stringBuilder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        stringBuilder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    string message = stringBuilder.ToString();
                    Console.WriteLine(message);

                }
                catch (Exception exception)
                {

                    Console.WriteLine(exception.Message);
                    Console.ReadLine();
                    Disconnect();
                }

            }
        }
        /// <summary>
        /// Envia mensagens
        /// </summary>
        private static void SendMessage()
        {
            while (true)
            {
                string message = Console.ReadLine();
                if (message.Contains("/exit"))
                {
                    Environment.Exit(0);
                }
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        /// <summary>
        /// Fecha o fluxo de dados, cliente tcp e encerra o processo de execução do programa
        /// </summary>
        private static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Environment.Exit(0);
        }
    }
}
