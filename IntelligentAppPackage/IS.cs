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
    public  class Is
    {
        public void PutValue(string param, double value)
        {
            _rp.PutValue(param, value);
            if (_values.ContainsKey(param))
                _values[param] = value;
            else
                _values.Add(param, value);
        }
        public string ModulOutput(string modul)
        {
            var s = _mI.GetModulOutPut(modul) ?? _newMi.GetModulOutPut(modul);
            return s;
        }
        readonly Mi _mI;
        readonly Mi _newMi;
        readonly Rp _rp;
        readonly string _pathB;
        readonly StreamReader _sr;
        Dictionary<string, double?> _values;
        public Is(StreamReader sr, string basePath)
        {
            _mI = new Mi();
            _rp = new Rp();
            _sr = sr;
            _newMi = new Mi();
            _pathB = basePath;
        }
        private void ReadRp()
        {
            string line = _sr.ReadLine();
            while (line != "#1") { line = _sr.ReadLine(); }
            line = _sr.ReadLine();
            while (line != "#2")
            {
                if (line != "")
                {
                    _rp.Add(line);
                }
                line = _sr.ReadLine();
            }
        }
        private void ReadMi()
        {
            string line = _sr.ReadLine();
            do
            {
                if (line != "")
                {
                    _mI.Add(line);
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
        public bool isCalc(string unknown, out double result)
        {
            bool t = false;
            result = 0;
            if (_values.ContainsKey(unknown))
            {
                t = true;
                result = (double)_values[unknown];
            }
            return t;
        }
        public bool isCalc(string unknown)
        {
            return _values.ContainsKey(unknown);
            
        }
        private bool IsCalculated(string unknown, out double result)
        {
            return _rp.IsCalc(unknown, out result);
        }
        public bool GetPath(string unknown, out Stack<string> path)
        {
            bool t = false;
            path = new Stack<string>();
            List<string> l = _mI.GetModulsIdent(unknown);
            List<string> l1 = _newMi.GetModulsIdent(unknown);
            List<string> badModuls = new List<string>();
            if (l.Count != 0 || l1.Count != 0)
            {
                foreach (string t1 in l)
                {
                    t = true;
                    string T = t1;
                    path.Push(T);
                    List<string> param = _mI.GetModulParam(T);
                    Stack<string> temp = new Stack<string>();
                    bool b = true;
                    foreach (string line in param)
                    {
                        b = isCalc(line);
                        if (!b)
                        {
                            if (GetPath(line, out temp, badModuls, path))
                            {
                                Join(path, temp);
                                _values.Add(line, null);
                                b = true;
                            }
                        }
                        t = t && b;
                        if (!b)
                        {
                            badModuls.Add(path.Pop());
                            break;
                        }
                    }
                }
                if (!t)
                    for (int i = 0; i < l1.Count; i++)
                    {
                        t = true;
                        string T = l1[i];
                        path.Push(T);
                        List<string> param = _newMi.GetModulParam(T);
                        Stack<string> temp = new Stack<string>();
                        bool b = true;
                        foreach (string line in param)
                        {
                            b = isCalc(line);
                            if (!b)
                            {
                                if (GetPath(line, out temp, badModuls, path))
                                {
                                    Join(path, temp);
                                    _values.Add(line, null);
                                    b = true;
                                }
                            }
                            t = t && b;
                            if (!b)
                            {
                                badModuls.Add(path.Pop());
                                break;
                            }
                        }
                    }
            }
            return t;
        }
        public bool GetPath(string unknown, out Stack<string> tpath, List<string> badModuls, Stack<string> path)
        {
            bool t = false;
            tpath = new Stack<string>();
            List<string> l = _mI.GetModulsIdent(unknown);
            List<string> l1 = _newMi.GetModulsIdent(unknown);
            if (l.Count != 0 || l1.Count != 0)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    string T = l[i];
                    if (!(path.Contains(T) && badModuls.Contains(T)))
                    {
                        tpath.Push(T);
                        List<string> param = _mI.GetModulParam(T);
                        Stack<string> temp = new Stack<string>();
                        bool b = true;
                        foreach (string line in param)
                        {
                            b = isCalc(line);
                            if (!b)
                            {
                                if (GetPath(line, out temp, badModuls, path))
                                {
                                    Join(tpath, temp);
                                    _values.Add(line, null);
                                    b = true;
                                }
                            }
                            t = b;
                            if (!b)
                            {
                                badModuls.Add(tpath.Pop());
                                break;
                            }
                        }
                    }
                }
                if (!t)
                    for (int i = 0; i < l1.Count; i++)
                    {

                        string T = l1[i];
                        if (!(path.Contains(T) && badModuls.Contains(T)))
                        {
                            tpath.Push(T);
                            List<string> param = _newMi.GetModulParam(T);
                            Stack<string> temp = new Stack<string>();
                            bool b = true;
                            foreach (string line in param)
                            {
                                b = isCalc(line);
                                if (!b)
                                {
                                    if (GetPath(line, out temp, badModuls, path))
                                    {
                                        Join(tpath, temp);
                                        _values.Add(line, null);
                                        b = true;
                                    }
                                }
                                t = b;
                                if (!b)
                                {
                                    badModuls.Add(tpath.Pop());
                                    break;
                                }
                            }
                        }
                    }
            }
            return t;
        }
        public static Stack<string> Join(Stack<string> t1, Stack<string> t2)
        {
            Stack<string> tt = new Stack<string>();
            Stack<string> temp = new Stack<string>();

            while (t2.Count != 0)
            {
                string s = t2.Pop();
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
            List<string> S = _mI.GetModulParam(modul);
            List<VariableValue> result = new List<VariableValue>();
            if (S == null) S = _newMi.GetModulParam(modul);
            foreach (string s in S)
            {
                double a;
                if (isCalc(s, out a))
                    result.Add(new VariableValue(a, s));
                else throw (new Exception("unknown param"));
            }
            return result;
        }
        public List<string> GetObjects()
        {
            return _rp.GetObjects();
        }
        public string GetDescription(string variable)
        {
            for (int i = 0; i < _rp.Length; i++)
            {
                if (_rp[i].Ident == variable) return _rp[i].Name;
            }
            return "";
        }
        public void AddModul(string output, string name, string id, List<string> param)
        {
            _newMi.Add(id, name, output, param);
        }
        public void Close()
        {
            StreamWriter sw = new StreamWriter(_pathB, true);
            for (int i = 0; i < _newMi.Length; i++)
            {
                string s = _newMi[i].ModuleOutput + "=" + _newMi[i].ModuleIdent + "(";
                List<string> l = _newMi[i].ModulParams;
                s += l[0];
                for (int j = 1; j < l.Count; j++)
                    s += "," + l[j];
                sw.WriteLine(s + "):" + _newMi[i].ModuleName);
            }
            sw.Close();
        }
    }
}
