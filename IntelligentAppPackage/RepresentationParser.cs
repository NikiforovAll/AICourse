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
    public class RepresentationParser
    {
        IntelligentSolver _intelligentSolver;
        Dictionary<string, Expression> _expressions;
        Dictionary<string, Expression> _newExpressions;
        string _pathM;
        StreamReader _modulSr;
        public RepresentationParser(StreamReader baseSr, StreamReader modulSr, string modulpath, string basePath)
        {
            _intelligentSolver = new IntelligentSolver(baseSr, basePath);
            _intelligentSolver.Start();
            _pathM = modulpath;
            this._modulSr = modulSr;
            Start();
        }
        public bool AnswerQuestion(string unknown, out double result, out bool f)
        {
            var t = false;
            result = 0;
            f = true;
            if (_intelligentSolver.IsCalc(unknown, out result))
                t = true;
            Stack<string> s;
            if (_intelligentSolver.GetPath(unknown, out s))
            {
                while (s.Count != 0)
                {
                    var modul = s.Pop();
                    double a;
                    if (Calc(modul, GetParam(modul), out a))
                        f = false;
                    _intelligentSolver.PutValue(_intelligentSolver.ModulOutput(modul), a);
                }
            }
            if (_intelligentSolver.IsCalc(unknown, out result))
                t = true;
            return t;
        }

        public string ShowModel()
        {
            var result = "ModulInterfaces: \n";
            for (var i = 0; i < _intelligentSolver.ModuleInterfaceCollection.Length; i++)
            {
                result += _intelligentSolver.ModuleInterfaceCollection[i] + "\n";
            }
            result += "TrpObjects \n";
            for (var i = 0; i < _intelligentSolver.RepresentationModule.Length; i++)
            {
                result += _intelligentSolver.RepresentationModule[i] + "\n";
            }
            return result;

        }
        private List<VariableValue> GetParam(string modul)
        {
            return _intelligentSolver.GetParam(modul);
        }
        private bool Calc(string modul, List<VariableValue> variables, out double result)
        {
            var t = false;
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
            return _intelligentSolver.GetObjects();
        }
        public void PutValue(string param, double value)
        {
            _intelligentSolver.PutValue(param, value);
        }
        public void Start()
        {
            _expressions = new Dictionary<string, Expression>();
            _newExpressions = new Dictionary<string, Expression>();
            while (!_modulSr.EndOfStream)
            {
                var line = _modulSr.ReadLine();
                if (line == "") continue;
                Expression ex;
                _expressions.Add(Parse(line, out ex), ex);
            }
        }
        public void Close()
        {
            var sw = new StreamWriter(_pathM, true);
            foreach (var kvp in _newExpressions)
            {
                var s = kvp.Key + " " + kvp.Value.Output + "=" + ToolsHelper.Decompiler.Decompile(kvp.Value.Ce);
                sw.WriteLine(s);
            }
            sw.Close();
        }
        class Expression
        {
            public Expression(string output, CompiledExpression ce)
            {
                Ce = ce;
                Output = output;
            }
            public CompiledExpression Ce { get; }
            public string Output { get; }
        }

        static string Parse(string modul, out Expression ex)
        {
            var m = modul.Split(' ', '=', ':');
            var preparedExpression = ToolsHelper.Parser.Parse(m[2]);
            var compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
            var optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);
            ex = new Expression(m[1], optimizedExpression);
            return m[0];
        }

        static string Parse(string modul, out Expression ex, out string description)
        {
            var m = modul.Split(' ', '=', ':');
            var preparedExpression = ToolsHelper.Parser.Parse(m[2]);
            var compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);
            var optimizedExpression = ToolsHelper.Optimizer.Optimize(compiledExpression);
            ex = new Expression(m[1], optimizedExpression);
            description = m[3];
            return m[0];
        }
        public void AddModul(string modul)
        {
            Expression ex;
            string sn;
            var s = Parse(modul, out ex, out sn);
            _newExpressions.Add(s, ex);
        }
        public string GetDescription(string variable)
        {
            return _intelligentSolver.GetDescription(variable);
        }
    }
}
