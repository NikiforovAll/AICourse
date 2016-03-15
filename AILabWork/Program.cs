using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace AILabWork
{
    static class Program
    {
        //lab1 
        static void Main(string[] args)
        {
            SemanticNetwork semanticNetwork = new SemanticNetwork();
            semanticNetwork.ParseData("data.txt");
            semanticNetwork.SetTransientRelations(1, 2);
            semanticNetwork.CalculateAdditionalKnowledge();
            Console.WriteLine(semanticNetwork.ShowSemanticNetwork());
            Console.WriteLine("Please enter values for your query");
            var queryList = new List<int?>();
            Console.WriteLine("code1: ");
            queryList.Add(Console.ReadLine().ConvertForQuery());
            Console.WriteLine("relation: ");
            queryList.Add(Console.ReadLine().ConvertForQuery());
            Console.WriteLine("code2: ");
            queryList.Add(Console.ReadLine().ConvertForQuery());
            Console.WriteLine(queryList.Select((el, i) =>
                el != null ? (i % 2 != 1 ?
                semanticNetwork.Entities[el.Value] : semanticNetwork.Relations[el.Value].Name)
                : "?"
                ).Aggregate((i, j) => i + ' ' + j) + "\nAnswer:");
            Console.WriteLine(String.Join("\n", semanticNetwork.QueryKnowledgeBase(queryList[0], queryList[1], queryList[2])));

            Console.WriteLine("");
        }

        private static int? ConvertForQuery(this string str)
        {
            return str == "?" ? null : Convert.ToInt32(str) as int?;
        }

        private static string ShowSemanticNetwork(this SemanticNetwork semanticNetwork)
        {
            return "Objects:\n" +
                String.Join("\n", semanticNetwork.Entities.Keys.Select(x => $"{semanticNetwork.Entities[x]} code:{x}").ToList())
                + "\nRelations\n" + String.Join("\n", semanticNetwork.Relations.Values.Select(x => x.ToString()));
        }
    }
}
