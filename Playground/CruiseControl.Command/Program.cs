namespace CruiseControl.Command
{
    using CruiseControl.Common;

    class Program
    {
        static void Main(string[] args)
        {
            var connection = new ServerConnection(args[0]);
            connection.Ping();
        }
    }
}
