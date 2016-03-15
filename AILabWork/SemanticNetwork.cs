using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AILabWork
{
    public class SemanticNetwork
    {
        public KnowledgeBase KnowledgeBaseResource { get; set; } = new KnowledgeBase();
        public KnowledgeBase KnowledgeBaseCalculated { get; set; } = new KnowledgeBase();
        public Dictionary<int, string> Entities { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, Relation> Relations { get; set; } = new Dictionary<int, Relation>();
        public List<int> TransientRelations = new List<int>();
        //transient rule of undecrease(1)
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
                    continue;
                }
                if (block == 2 && resourceLine.Length == 3)
                {
                    var code = Convert.ToInt32(resourceLine[0]);
                    Relations.Add(code, new Relation() { ID = code, Name = resourceLine[1], Type = Convert.ToInt32(resourceLine[2]) });
                    continue;
                }
                if (block == 3 && resourceLine.Length == 3)
                {
                    KnowledgeBaseResource.AddRelation(new RelationEntity()
                    {
                        Obj1 = Convert.ToInt32(resourceLine[0]),
                        Obj2 = Convert.ToInt32(resourceLine[2]),
                        Rel = Convert.ToInt32(resourceLine[1])
                    });
                    continue;
                }
                throw new FormatException($"Incorrect format. line:\n{line}");
            }
            streamReader.Close();
        }

        public void SetTransientRelations(params int[] relations)
        {
            TransientRelations.AddRange(relations);
        }

        public void CalculateAdditionalKnowledge()
        {
            foreach (var relationEntity in KnowledgeBaseResource)
            {
                FindChildRelation(relationEntity).ForEach(el => KnowledgeBaseCalculated.AddRelation(el));
            }
        }

        private List<RelationEntity> FindChildRelation(RelationEntity relationEntity)
        {
            return FindChildRelation(relationEntity.Obj2, relationEntity.Rel, relationEntity.Obj1);
        }
        private List<RelationEntity> FindChildRelation(int code, int type, int mainCode)
        {
            var knowledgeDictionary = KnowledgeBaseResource.KnowledgeDictionary;
            var resultList = new List<RelationEntity>();
            if (knowledgeDictionary.ContainsKey(code))
            {
                knowledgeDictionary[code]
                    .Where(el => TransientRelations.Contains(type) &&
                                 Relations[type].Type <= Relations[el.Rel].Type) //(1)
                    .ToList()
                    .ForEach(el => resultList.AddRange(FindChildRelation(el.Obj2, el.Rel, mainCode)));
                resultList.Add(new RelationEntity() {Obj1 = mainCode, Obj2 = code, Rel = type});
            }
            else
            {
                resultList.Add(new RelationEntity() { Obj1 = mainCode, Obj2 = code, Rel = type });
            }
            return resultList;
        }
        

        public List<string> QueryKnowledgeBase(int? code1, int? relation, int? code2)
        {

            //var knowledgeBaseToQuery = KnowledgeBaseCalculated;
            var knowledgeBaseToQuery = KnowledgeBaseCalculated.Union(KnowledgeBaseResource.Select(el => el)).ToList();// or Except
            var result = knowledgeBaseToQuery
                .Where(el =>
                    (code1 == null || el.Obj1 == code1) &&
                    (code2 == null || el.Obj2 == code2) &&
                    (relation == null || relation == el.Rel)
                )
                .Select(GetHumanReadableForm).ToList();
            if (result.Count == 0)
            {
                return new List<string>() { "Don't know" };
            }
            if (code1.HasValue && code2.HasValue && relation.HasValue)
            {
                result = new List<string>() { "YES" };
            }
            return result;
        }
        private string GetHumanReadableForm(RelationEntity relationEntity) =>
            $"{Entities[relationEntity.Obj1]} - {Relations[relationEntity.Rel].Name} - {Entities[relationEntity.Obj2]} ";
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
        public override string ToString() => $"{ID} - {Name} type: {Type}";

    }

    public class RelationEntity
    {
        public int Obj1 { get; set; }
        public int Obj2 { get; set; }
        public int Rel { get; set; }
        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            // hash selection 
            return (Obj1.GetHashCode() * 100 + Obj2.GetHashCode() * 10 + Rel.GetHashCode());
        }
        
    }

    public class KnowledgeBase : IEnumerable<RelationEntity>
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

        public IEnumerator<RelationEntity> GetEnumerator()
        {
            return KnowledgeDictionary.Values.SelectMany(el => el).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class FormatException : Exception
    {
        public FormatException(string message = "Incorrect Format") : base(message) { }
    }
}
