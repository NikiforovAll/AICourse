using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAppPackage
{
    public class TrpObject
    {
        public string Ident { get; private set; }
        public string Name { get; private set; }
        private double _value;

        public TrpObject(){}
        public TrpObject(string ident, string name)
        {
            Name = name;
            Ident = ident;
        }

        public TrpObject(string ident, string name, double value):this(ident,name)
        {
            _value = value;
            IsCalc = true;
        }
        public double Value
        {
            get { return _value; }
            set
            {
                _value= value;
                IsCalc = true;
            }
        }
        public bool IsCalc { get; private set; }
        
    }
}
