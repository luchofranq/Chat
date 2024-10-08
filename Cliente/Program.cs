using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private TcpClient? client;
    private NetworkStream? stream;

    static void Main(string[] args)
    {
        Program program = new Program();
        program.StartClient();
    }

    private void StartClient()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 8888);
            stream = client.GetStream();
            Console.WriteLine("Conectado al servidor.");

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            while (true)
            {
                string? message = Console.ReadLine(); 

                if (!string.IsNullOrEmpty(message)) 
                {
                    byte[] data = Encoding.ASCII.GetBytes(message); 
                    stream?.Write(data, 0, data.Length); 
                }
                else
                {
                    Console.WriteLine("El mensaje no puede estar vacío.");
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void ReceiveMessages()
    {
        try
        {
            while (true)
            {
                byte[] data = new byte[1024];
                int bytes = stream?.Read(data, 0, data.Length) ?? 0;
                if (bytes > 0)
                {
                    string message = Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error recibiendo mensajes: {ex.Message}");
        }
    }
}
