using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAppPackage
{
    public class Mi
    {
        private readonly List<ModulInterface> _tModulInterfaces = new List<ModulInterface>(50);
        public int Length => _tModulInterfaces.Count;
        public ModulInterface this[int index] => _tModulInterfaces[index];
        public void Add(string line)
        {
            _tModulInterfaces.Add(GetTModulInterface(line));
        }
        public void Add(string output, string name, string id, List<string> param)
        {
            _tModulInterfaces.Add(new ModulInterface(id, name, output, param));
        }
        public string GetModulOutPut(string modulIdent) => _tModulInterfaces.LastOrDefault(mi => mi.ModuleIdent == modulIdent)?.ModuleOutput;

        public List<string> GetModulParam(string modulIdent) => _tModulInterfaces.LastOrDefault(mi => mi.ModuleIdent == modulIdent)?.ModulParams;

        public List<string> GetModulsIdent(string unknown) => _tModulInterfaces.Where(mi => mi.ModuleOutput == unknown).Select(mi => mi.ModuleIdent).ToList();

        public List<string> GetModulsName(string unknown) => _tModulInterfaces.Where(mi => mi.ModuleOutput == unknown).Select(mi => mi.ModuleName).ToList();

        ModulInterface GetByName(string modul) => _tModulInterfaces.FirstOrDefault(mi => mi.ModuleName == modul);

        ModulInterface GetByIdent(string modul) => _tModulInterfaces.FirstOrDefault(mi => mi.ModuleIdent == modul);

        ModulInterface GetTModulInterface(string line)
        {
            ModulInterface T;
            if (!ParseModul(line, out T))
                throw (new Exception("Illegal modul"));
            return T;
        }
        bool ParseModul(string line, out ModulInterface T)
        {
            line = line.Replace(" ", "");
            string id = "";
            string output;
            string name = "";
            List<string> param = new List<string>();
            bool t = true;
            int i = 0;
            int j = 0;
            for (; i < line.Length && t; i++)
            {
                t = Char.IsLetterOrDigit(line[i]);
            }
            output = line.Substring(0, i - 1);
            bool b = false;
            if (line[i - 1] == '=')
            {
                t = true;
                j = i;
                for (; i < line.Length && t; i++)
                {
                    t = Char.IsLetterOrDigit(line[i]);
                }
                id = line.Substring(j, i - j - 1);
                if (line[i - 1] == '(')
                {
                    t = true;
                    while (t && !b)
                    {
                        j = i;
                        b = i < line.Length && t;
                        for (; i < line.Length && t; i++)
                        {
                            t = Char.IsLetterOrDigit(line[i]);

                        }
                        param.Add(line.Substring(j, i - j - 1));
                        if (b)
                        {
                            t = line[i - 1] == ',';
                            b = line[i - 1] == ')';
                        }
                        else
                            break;
                    }
                }
                t = b;
            }
            if (t)
            {
                j = line.IndexOf(':');
                name = line.Substring(j + 1, line.Length - j - 1);
            }
            T = new ModulInterface(id, name, output, param);
            return t;
        }
       
    }
}
