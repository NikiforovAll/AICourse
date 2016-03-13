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
            semanticNetwork.SetTransientRelations(1, 2);
            semanticNetwork.CalculateAdditionalKnowledge();
            Console.WriteLine(semanticNetwork.QueryKnowledgeBase(null, null, null).Aggregate((i, j) => i + '\n' + j));
            Console.WriteLine("");
        }
    }
}
