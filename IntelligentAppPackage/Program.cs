using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using ELW.Library.Math.Expressions;

namespace IntelligentAppPackage
{
    class Program
    {
        public static Dictionary<string, double> SystemInputParams = new Dictionary<string, double>();
        static void Main(string[] args)
        {
            var expresion = new List<CompiledExpression>();
            var ass = Assembly.GetExecutingAssembly();
            var path = Environment.CurrentDirectory;
            var modulPath = path + "\\M.txt";
            var basePath = path + "\\B.txt";
            var dataPath = path + "\\data.txt";
            var srM = new StreamReader(modulPath);
            var srB = new StreamReader(basePath);
            var srD = new StreamReader(dataPath);
            var system = new Ippp(srB, srM, modulPath, basePath);
            Console.WriteLine("Task: ");
            string taskStr = "";
            
            while (!srD.EndOfStream)
            {
                var line = srD.ReadLine()?.Trim();
                var splittedParams = line.Split(new char[] { '=', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedParams[1].Trim() != "?")
                {
                    SystemInputParams.Add(splittedParams[0], Double.Parse(splittedParams[1].Trim()));
                    Console.WriteLine($"{{{splittedParams[0]}: {splittedParams[1]}}}");
                }

                else
                {
                    if (taskStr != string.Empty)
                    {
                        Console.WriteLine("ERROR: Invalid input: only one question per task");
                        return;
                    }
                    taskStr = splittedParams[0];
                    Console.WriteLine($" {taskStr} - ?");
                }

            }
            foreach (var parameter in SystemInputParams.Keys)
            {
                system.PutValue(parameter, SystemInputParams[parameter]);
            }
            if (args.Length > 0 && args[0] == "--show")
            {
                Console.WriteLine(system.ShowModel());
                Console.ReadKey();
            }
            double s;
            bool f;
            bool t;
            if (taskStr != string.Empty)
                t = system.AnswerQuestion(taskStr, out s, out f);
            else
            {
                Console.WriteLine("Task was not found");
                return;
            }
            //if (!f) Console.WriteLine("Unknown modul in base");
            if (SystemInputParams.ContainsKey(taskStr))
            {
                if (Math.Abs(SystemInputParams[taskStr] - s) > 0.001)
                {
                    Console.WriteLine("ERROR: Inconsistent data");
                    return;
                }
                
            }
            Console.WriteLine(!t ? "Not enough data" : $"Result: {taskStr} = {s}");
            srB.Close();
            srM.Close();
            srD.Close();
        }
    }
}
