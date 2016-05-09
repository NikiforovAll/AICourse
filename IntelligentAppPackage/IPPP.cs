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
        Is _is;
        Dictionary<string, Expression> _expressions;
        Dictionary<string, Expression> _newExpressions;
        string _pathM;
        StreamReader _modulSr;
        public Ippp(StreamReader baseSr, StreamReader modulSr, string modulpath, string basePath)
        {
            _is = new Is(baseSr, basePath);
            _is.Start();
            _pathM = modulpath;
            this._modulSr = modulSr;
            Start();
        }
        public bool AnswerQuestion(string unknown, out double result, out bool f)
        {
            bool t = false;
            result = 0;
            f = true;
            if (_is.isCalc(unknown, out result))
                t = true;
            Stack<string> s;
            if (_is.getPath(unknown, out s))
            {
                while (s.Count != 0)
                {
                    string modul = s.Pop();
                    double a;
                    if (Culc(modul, GetParam(modul), out a))
                        f = false;
                    _is.PutValue(_is.ModulOutput(modul), a);
                }
            }
            if (_is.isCalc(unknown, out result))
                t = true;
            return t;
        }
        private List<VariableValue> GetParam(string modul)
        {
            return _is.GetParam(modul);
        }
        private bool Culc(string modul, List<VariableValue> variables, out double result)
        {
            bool t = false;
            if (_expressions.ContainsKey(modul))
            {
                result = ToolsHelper.Calculator.Calculate(_expressions[modul].Ce, variables);
                t = true;

            }
            else
            {
                if (_newExpressions.ContainsKey(modul))
                {
                    result = ToolsHelper.Calculator.Calculate(_newExpressions[modul].Ce, variables);
                    t = true;
                }
                else
                    result = 0;
            }
            return t;
        }
        public List<string> GetObjects()
        {
            return _is.GetObjects();
        }
        public void PutValue(string param, double value)
        {
            _is.PutValue(param, value);
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
                string s = kvp.Key + " " + kvp.Value.Output + "=" + ToolsHelper.Decompiler.Decompile(kvp.Value.Ce);
                sw.WriteLine(s);
            }
            sw.Close();
            _is.Close();
        }
        class Expression
        {
            CompiledExpression _cE;
            string _output;
            public Expression(string output, CompiledExpression ce)
            {
                _cE = ce;
                _output = output;
            }
            public CompiledExpression Ce
            {
                get { return _cE; }
            }
            public string Output
            {
                get { return _output; }
            }
        }
        string Parse(string modul, out Expression ex)
        {
            string[] m = modul.Split(' ', '=', ':');
            // Compiling an expression
            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(m[2]);
            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
            // Optimizing an expression
            CompiledExpression optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);
            ex = new Expression(m[1], optimizedExpression);
            return m[0];
        }
        string Parse(string modul, out Expression ex, out string description)
        {
            string[] m = modul.Split(' ', '=', ':');
            // Compiling an expression
            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(m[2]);
            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
            // Optimizing an expression
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
            _is.AddModul(s, sn, ex.Output, ex.Ce.CompiledExpressionVariebles);
        }
        public string GetDescription(string variable)
        {
            return _is.GetDescription(variable);
        }
    }
}
