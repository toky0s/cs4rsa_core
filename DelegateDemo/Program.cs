using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegateDemo
{
    class Program
    {
        delegate string MyDeledate(string s);
        static void Main(string[] args)
        {
            MyDeledate myDeledate = new MyDeledate(somethingMethod);
            myDeledate("Xin");
        }

        static string somethingMethod(string s)
        {
            Console.WriteLine(s);
            return "";
        }
    }
}
