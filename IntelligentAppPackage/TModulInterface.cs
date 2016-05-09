using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAppPackage
{
   public class ModulInterface
    {
        public string ModuleIdent { get;}
        public string ModuleName { get;}
        public string ModuleOutput { get; } 
        public List<string> ModulParams { get; }
        public ModulInterface() { }
        public ModulInterface(string moduleIdent, string moduleName, string output, List<string> modulParams)
        {
            ModuleIdent = moduleIdent;
            ModuleName = moduleName;
            ModuleOutput = output;
            ModulParams = modulParams;
        }
        public override string ToString()
        {
            return $"{ModuleIdent} : {ModuleName} : {String.Join(", ", ModulParams)} :: {ModuleOutput}";
        }


    }
}
