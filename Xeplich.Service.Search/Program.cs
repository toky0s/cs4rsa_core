using Cs4rsa.Database.DataProviders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xeplich.Service.Search
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            IndexBuilder indexBuilder = new IndexBuilder(
                new RawSql(
                    "Data Source=C:\\Users\\Truong A Xin\\source\\repos\\cs4rsa_core\\Cs4rsa.App\\cs4rsa.db",
                    null));


            indexBuilder.BuildIndex();
            indexBuilder.SearchWithBoost(out List<DataModel> results, out int totalHits, "cs lap trinh");
            results.ForEach(item =>
                Console.WriteLine($"{item.DisplayedText}")
            );
        }
    }
}
