using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using ELW.Library.Math.Expressions;

namespace IntelligentAppPackage
{
    class Program
    {
        static void Main(string[] args)
        {
            Ippp system;
            List<CompiledExpression> expresion;
            expresion = new List<CompiledExpression>();
            Assembly ass = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(ass.Location);
            string modulPath = path + "\\M.txt";
            string basePath = path + "\\B.txt";
            StreamReader srM = new StreamReader(modulPath);
            StreamReader srB = new StreamReader(basePath);
            system = new Ippp(srB, srM, modulPath, basePath);
            system.PutValue("Hc", 2);
            system.PutValue("b",1.5);
            double s;
            bool res;
            system.AnswerQuestion("S", out s, out res);
            Console.WriteLine(s);


        }
    }
}
