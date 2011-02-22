namespace CruiseControl.Command
{
    using CruiseControl.Common;

    class Program
    {
        static void Main(string[] args)
        {
            ServerConnection.Ping(args[0]);
        }
    }
}
