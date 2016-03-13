using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabWork
{
    public class SemanticNetwork
    {
        public KnowledgeBase KnowledgeBaseResource { get; set; } = new KnowledgeBase();
        public Dictionary<int, string> Entities { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, Relation> Relations { get; set; } = new Dictionary<int, Relation>();
        public void ParseData(string fileName)
        {
            string path = Path.Combine(Environment.CurrentDirectory, fileName);
            StreamReader streamReader = new StreamReader(path);
            int block = 0;
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine()?.Trim();
                if (String.Compare(line, "#1", StringComparison.Ordinal) == 0)
                {
                    block = 1;
                    continue;
                }
                if (String.Compare(line, "#2", StringComparison.Ordinal) == 0)
                {
                    block = 2;
                    continue;
                }
                if (String.Compare(line, "#3", StringComparison.Ordinal) == 0)
                {
                    block = 3;
                    continue;
                }
                if (String.IsNullOrEmpty(line))
                {
                    continue;
                }
                var resourceLine =
                        line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(el => el.Trim())
                        .ToArray();
                if (block == 1 && resourceLine.Length == 2)
                {
                    Entities.Add(Convert.ToInt32(resourceLine[0]), resourceLine[1]);
                }
                if (block == 2 && resourceLine.Length == 3)
                {
                    var code = Convert.ToInt32(resourceLine[0]);
                    Relations.Add(code, new Relation() { ID = code, Name = resourceLine[1], Type = Convert.ToInt32(resourceLine[2]) });
                }
                if (block == 3 && resourceLine.Length == 3)
                {
                    KnowledgeBaseResource.AddRelation(new RelationEntity()
                    {
                        Obj1 = Convert.ToInt32(resourceLine[0]),
                        Obj2 = Convert.ToInt32(resourceLine[2]),
                        Rel = Convert.ToInt32(resourceLine[1])
                    });
                }
            }
            streamReader.Close();
        }
    }

    public class Entity
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


    public class Relation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }

    public class RelationEntity
    {
        public int Obj1 { get; set; }
        public int Obj2 { get; set; }
        public int Rel { get; set; }
    }

    public class KnowledgeBase
    {
        public Dictionary<int, List<RelationEntity>> KnowledgeDictionary { get; } = new Dictionary<int, List<RelationEntity>>();

        public void AddRelation(RelationEntity relation)
        {
            if (KnowledgeDictionary.ContainsKey(relation.Obj1))
            {
                List<RelationEntity> list;
                KnowledgeDictionary.TryGetValue(relation.Obj1, out list);
                list?.Add(relation);
            }
            else
            {
                KnowledgeDictionary.Add(relation.Obj1, new List<RelationEntity>() { relation });
            }
        }
    }

    public class FormatException : Exception
    {
        public FormatException(string message = "Incorrect Format") : base(message)
        {
        }
    }


}
