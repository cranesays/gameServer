namespace EchoServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(10086);
            Console.WriteLine("Sever Start!");
            Console.Read();
            
        }
    }
}