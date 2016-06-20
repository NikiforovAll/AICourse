using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAppPackage
{
    public class RepresentationModule
    {
        private readonly List<TrpObject> _trpObjects = new List<TrpObject>(50);
        public int Length => _trpObjects.Count;
        public TrpObject this[int index] => _trpObjects[index];

        public void Add(string line)
        {
            _trpObjects.Add(GetTrpObject(line));
        }
        public bool IsCalc(string unknown, out double result)
        {
            var item = _trpObjects.LastOrDefault(obj => obj.Ident == unknown);
            result = item?.Value ?? 0.0;
            return item?.IsCalc ?? false;

        }
        public void PutValue(string param, double value)
        {
            _trpObjects.Where( obj => obj.Name == param).ToList().ForEach(obj => obj.Value = value);
        }
        public List<string> GetObjects()
        {
            return _trpObjects.Select(obj => obj.Ident).ToList();
        }

        private static TrpObject GetTrpObject(string line)
        {
            var m = line.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!m.SelectMany(word => word.ToCharArray()).All(Char.IsLetterOrDigit))
                throw new Exception("Illegal name");
            return new TrpObject(m[0], m[1]);
        }
    }
}
