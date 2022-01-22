using Cs4rsaDatabaseService.DataProviders;
using System;

namespace Cs4rsaDatabaseService
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Cs4rsaDbContext cs4RsaDbContext = new Cs4rsaDbContext())
            {
                cs4RsaDbContext.Database.EnsureCreated();
                Console.WriteLine("Hello World");
            }
        }
    }
}
