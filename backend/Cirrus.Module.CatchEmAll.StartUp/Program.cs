using System;
using Cirrus.Business;

namespace Cirrus.Module.CatchEmAll.StartUp
{
    /// <summary>
    /// Todo's
    /// - Zusätzliche Filter auf Queries
    /// - Triggers für zB Email Versand bei gewissem Preis (oder default?)
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting service.");
                var service = new BusinessServices(args);
                service.Start();
                Console.WriteLine("The service is ready.");
                Console.WriteLine();
                Console.ReadLine();
                Console.WriteLine("The service is stopped.");
                service.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
