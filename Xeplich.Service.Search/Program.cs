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

            var result = indexBuilder.Search("Lập");
            result.ForEach(item => Console.WriteLine($"{item.SubjectName} {item.SubjectCode} {item.Discipline} {item.Keyword} {item.SubjectDescription}"));
        }
    }
}
