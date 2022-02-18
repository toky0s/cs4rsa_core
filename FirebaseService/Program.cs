using System;
using System.Threading.Tasks;

namespace FirebaseService
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            NewFirebase newFirebase = new();
            await newFirebase.PostUser("24211fakeStudentId", "83741phakeSpecialString");
            Console.WriteLine("Done");
            //object latestVersion = await newFirebase.GetLatestVersion();
            //Console.WriteLine(latestVersion);
            //Console.ReadKey();
        }
    }
}
