using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELW.Library.Math;
using ELW.Library.Math.Exceptions;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;

namespace IntelligentAppPackage
{
    public class Ippp
    {
        Is _IS;
        Dictionary<string, Expression> _expressions;
        Dictionary<string, Expression> _newExpressions;
        string _pathM;
        StreamReader _modulSr;
        public Ippp(StreamReader baseSr, StreamReader modulSr, string modulpath, string basePath)
        {
            _IS = new Is(baseSr, basePath);
            _IS.Start();
            _pathM = modulpath;
            this._modulSr = modulSr;
            Start();
        }
        public bool AnswerQuestion(string unknown, out double result, out bool f)
        {
            bool t = false;
            result = 0;
            f = true;
            if (_IS.isCalc(unknown, out result))
                t = true;
            Stack<string> s;
            if (_IS.GetPath(unknown, out s))
            {
                while (s.Count != 0)
                {
                    string modul = s.Pop();
                    double a;
                    if (Calc(modul, GetParam(modul), out a))
                        f = false;
                    _IS.PutValue(_IS.ModulOutput(modul), a);
                }
            }
            if (_IS.isCalc(unknown, out result))
                t = true;
            return t;
        }

        public string ShowModel()
        {
            var result = "ModulInterfaces: \n";
            for (int i = 0; i < _IS.MI.Length; i++)
            {
                result += _IS.MI[i] + "\n";
            }
            result += "TrpObjects \n";
            for (int i = 0; i < _IS.RP.Length; i++)
            {
                result += _IS.RP[i] + "\n";
            }
            return result;

        }
        private List<VariableValue> GetParam(string modul)
        {
            return _IS.GetParam(modul);
        }
        private bool Calc(string modul, List<VariableValue> variables, out double result)
        {
            bool t = false;
            if (_expressions.ContainsKey(modul))
            {
                result = ToolsHelper.Calculator.Calculate(_expressions[modul].CE, variables);
                t = true;
            }
            else
            {
                if (_newExpressions.ContainsKey(modul))
                {
                    result = ToolsHelper.Calculator.Calculate(_newExpressions[modul].CE, variables);
                    t = true;
                }
                else
                    result = 0;
            }
            return t;
        }
        public List<string> GetObjects()
        {
            return _IS.GetObjects();
        }
        public void PutValue(string param, double value)
        {
            _IS.PutValue(param, value);
        }
        public void Start()
        {
            _expressions = new Dictionary<string, Expression>();
            _newExpressions = new Dictionary<string, Expression>();
            while (!_modulSr.EndOfStream)
            {
                string line = _modulSr.ReadLine();
                if (line != "")
                {
                    Expression ex;
                    _expressions.Add(Parse(line, out ex), ex);
                }
            }
        }
        public void Close()
        {
            StreamWriter sw = new StreamWriter(_pathM, true);
            foreach (KeyValuePair<string, Expression> kvp in _newExpressions)
            {
                string s = kvp.Key + " " + kvp.Value.Output + "=" + ToolsHelper.Decompiler.Decompile(kvp.Value.CE);
                sw.WriteLine(s);
            }
            sw.Close();
            _IS.Close();
        }
        class Expression
        {
            public Expression(string output, CompiledExpression ce)
            {
                CE = ce;
                Output = output;
            }
            public CompiledExpression CE { get; }
            public string Output { get; }
        }
        string Parse(string modul, out Expression ex)
        {
            string[] m = modul.Split(' ', '=', ':');
            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(m[2]);
            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
            CompiledExpression optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);
            ex = new Expression(m[1], optimizedExpression);
            return m[0];
        }
        string Parse(string modul, out Expression ex, out string description)
        {
            string[] m = modul.Split(' ', '=', ':');
            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(m[2]);
            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
            CompiledExpression optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);
            ex = new Expression(m[1], optimizedExpression);
            description = m[3];
            return m[0];
        }
        public void AddModul(string modul)
        {
            Expression ex;
            string sn;
            string s = Parse(modul, out ex, out sn);
            _newExpressions.Add(s, ex);
            _IS.AddModul(s, sn, ex.Output, ex.CE.CompiledExpressionVariebles);
        }
        public string GetDescription(string variable)
        {
            return _IS.GetDescription(variable);
        }
    }
}
