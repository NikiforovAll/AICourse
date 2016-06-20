using System;
using System.Collections.Generic;
using System.IO;
using ELW.Library.Math;
using ELW.Library.Math.Exceptions;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAppPackage
{
    public  class IntelligentSolver
    {
        public ModuleInterfaceCollection ModuleInterfaceCollection { get; }
        public RepresentationModule RepresentationModule { get; }
        readonly string _pathB;
        readonly StreamReader _sr;
        private Dictionary<string, double?> _values;
        public IntelligentSolver(StreamReader sr, string basePath)
        {
            ModuleInterfaceCollection = new ModuleInterfaceCollection();
            RepresentationModule = new RepresentationModule();
            _sr = sr;
            _pathB = basePath;
        }

        public void PutValue(string param, double value)
        {
            RepresentationModule.PutValue(param, value);
            if (_values.ContainsKey(param))
                _values[param] = value;
            else
                _values.Add(param, value);
        }

        public string ModulOutput(string modul)
        {
            var s = ModuleInterfaceCollection.GetModulOutPut(modul);
            return s;
        }

        private void ReadRp()
        {
            var line = _sr.ReadLine();
            while (line != "#1") { line = _sr.ReadLine(); }
            line = _sr.ReadLine();
            while (line != "#2")
            {
                if (line != "")
                {
                    RepresentationModule.Add(line);
                }
                line = _sr.ReadLine();
            }
        }

        private void ReadMi()
        {
            var line = _sr.ReadLine();
            do
            {
                if (line != "")
                {
                    ModuleInterfaceCollection.Add(line);
                }
                line = _sr.ReadLine();
            } while (!_sr.EndOfStream);
        }

        public void Start()
        {
            ReadRp();
            ReadMi();
            _values = new Dictionary<string, double?>();
        }

        public bool IsCalc(string unknown, out double result)
        {
            var t = false;
            result = 0;
            if (!_values.ContainsKey(unknown)) return false;
            t = true;
            result = (double)_values[unknown];
            return true;
        }
        public bool IsCalc(string unknown)
        {
            return _values.ContainsKey(unknown);
            
        }

        private bool IsCalculated(string unknown, out double result) => RepresentationModule.IsCalc(unknown, out result);

        public bool GetPath(string unknown, out Stack<string> path)
        {
            var t = false;
            path = new Stack<string>();
            var l = ModuleInterfaceCollection.GetModulsIdent(unknown);
            var badModuls = new List<string>();
            if (l.Count == 0) return false;
            foreach (var t1 in l)
            {
                t = true;
                var T = t1;
                path.Push(T);
                var param = ModuleInterfaceCollection.GetModulParam(T);
                foreach (var line in param)
                {
                    var b = IsCalc(line);
                    if (!b)
                    {
                        Stack<string> temp;
                        if (GetPath(line, out temp, badModuls, path))
                        {
                            Join(path, temp);
                            _values.Add(line, null);
                            b = true;
                        }
                    }
                    t = b;
                    if (b) continue;
                    badModuls.Add(path.Pop());
                    break;
                }
            }
            return t;
        }

        public bool GetPath(string unknown, out Stack<string> tpath, List<string> badModuls, Stack<string> path)
        {
            var t = false;
            tpath = new Stack<string>();
            var l = ModuleInterfaceCollection.GetModulsIdent(unknown);
            if (l.Count == 0) return false;
            foreach (var T in l.Where(T => !(path.Contains(T) && badModuls.Contains(T))))
            {
                tpath.Push(T);
                var param = ModuleInterfaceCollection.GetModulParam(T);
                foreach (var line in param)
                {
                    var b = IsCalc(line);
                    if (!b)
                    {
                        Stack<string> temp;
                        if (GetPath(line, out temp, badModuls, path))
                        {
                            Join(tpath, temp);
                            _values.Add(line, null);
                            b = true;
                        }
                    }
                    t = b;
                    if (b) continue;
                    badModuls.Add(tpath.Pop());
                    break;
                }
            }
            return t;
        }

        public static Stack<string> Join(Stack<string> t1, Stack<string> t2)
        {
            var tt = new Stack<string>();
            var temp = new Stack<string>();

            while (t2.Count != 0)
            {
                var s = t2.Pop();
                tt.Push(s);
            }
            while (tt.Count != 0)
            {
                t1.Push(tt.Pop());
            }
            return t1;
        }

        public List<VariableValue> GetParam(string modul)
        {
            var S = ModuleInterfaceCollection.GetModulParam(modul);
            var result = new List<VariableValue>();
            foreach (var s in S)
            {
                double a;
                if (IsCalc(s, out a))
                    result.Add(new VariableValue(a, s));
                else throw (new Exception("unknown param"));
            }
            return result;
        }

        public List<string> GetObjects()
        {
            return RepresentationModule.GetObjects();
        }

        public string GetDescription(string variable)
        {
            for (var i = 0; i < RepresentationModule.Length; i++)
            {
                if (RepresentationModule[i].Ident == variable) return RepresentationModule[i].Name;
            }
            return "";
        }
    }
}
