using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AILabWork
{
    class Program
    {
        //lab1 
        static void Main(string[] args)
        {
            SemanticNetwork semanticNetwork = new SemanticNetwork();
            semanticNetwork.ParseData("data.txt");
            Console.WriteLine("end");
        }
    }
}
