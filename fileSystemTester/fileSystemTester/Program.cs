using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileSystemTester
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> l = new List<int>();
            l.Add(1);
            l.Add(2);
            l.Add(3);
            l.Add(4);
            l.Add(5);
            l.Add(6);
            Console.WriteLine(l.FirstOrDefault(x => x == 7));
        }
    }
}
