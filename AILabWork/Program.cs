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
            Console.WriteLine(String.Join("\n",semanticNetwork.QueryKnowledgeBase(14, null, null)));
            Console.WriteLine("");
            semanticNetwork = new SemanticNetwork();
            semanticNetwork.ParseData("data2.txt");
            semanticNetwork.SetTransientRelations(1,2,4);
            semanticNetwork.CalculateAdditionalKnowledge();
            Console.WriteLine(String.Join("\n", semanticNetwork.QueryKnowledgeBase(null, null, null)));


        }
    }
}
