using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using Natasha;
using Natasha.Operator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DynamicPlugin
{

    public class ReWriter:IDisposable
    {

        public readonly static DecompilerSettings _setting;
        public readonly Assembly _assembly;
        public readonly AssemblyComplier _complier;
        public readonly ConcurrentDictionary<string, string> _cache;
        public readonly ConcurrentDictionary<string, Type> _typeCache;
        public string NewDllPath;
        public Assembly NewAssembly;
        public List<string> References;



        static ReWriter()
        {
            _setting = new DecompilerSettings(LanguageVersion.Latest);
            _setting.LoadInMemory = true;
            _setting.Dynamic = true;
        }




        public ReWriter(string filePath,bool useDepsJson=true, bool useRandomAssembly=true)
        {

            References = new List<string>();
            _typeCache = new ConcurrentDictionary<string, Type>();
            _cache = new ConcurrentDictionary<string, string>();
            var domain = DomainManagment.Random;
           

            _complier = new AssemblyComplier
            {
                Domain = domain,
                ComplieInFile = true
            };


            if (!useRandomAssembly)
            {
                _complier.AssemblyName = Path.GetFileNameWithoutExtension(filePath);
            }


            _assembly = DomainManagment.Random.LoadStream(filePath);
            CSharpDecompiler _decomplier;
            if (useDepsJson)
            {
                var module = new PEFile(filePath);
                var resolver = new UniversalAssemblyResolver(filePath, false, module.Reader.DetectTargetFrameworkId());
                _decomplier = new CSharpDecompiler(filePath, resolver, _setting);
            }
            else
            {
                _decomplier = new CSharpDecompiler(filePath, _setting);
            }
            
            foreach (var item in _assembly.ExportedTypes)
            {
                var temp = item.GetDevelopName();
                _typeCache[temp] = item;
                _cache[temp] = _decomplier.DecompileTypeAsString(new FullTypeName(item.FullName));
            }
            _assembly.DisposeDomain();

        }


        public void Builder(string typeName, Action<OopOperator> action)
        {

            if (_typeCache.ContainsKey(typeName))
            {

                OopOperator @operator = new OopOperator();
                @operator.UseType(_typeCache[typeName], typeName);
                action?.Invoke(@operator);
                this[typeName] = @operator.Script;

            }
            else
            {
                throw new Exception($"Can't find type:{typeName}!");
            }
          
        }
        public void Builder(Type type, Action<OopOperator> action)
        {
            Builder(type.GetDevelopName(), action);
        }




        public string this[string name]
        {

            get
            {
                if (_cache.ContainsKey(name))
                {
                    return _cache[name];
                }
                return default;
            }


            set
            {
                _cache[name] = value;

            }
        
        }




        public string this[Type type]
        {

            get
            {

                var name = type.GetDevelopName();
                return this[name];

            }


            set
            {

                var name = type.GetDevelopName();
                _cache[name] = value;

            }

        }




        public Assembly Complier()
        {

            foreach (var item in References)
            {
                _complier.Domain.LoadStream(item);
            }
            foreach (var item in _cache)
            {
                _complier.Add(item.Value);
            }
            NewAssembly = _complier.GetAssembly();
            NewDllPath = _complier.DllFilePath;
            return NewAssembly;

        }




        public void Dispose()
        {

            _assembly.DisposeDomain();
            _cache.Clear();
            _typeCache.Clear();
            _complier.Domain.Dispose();

        }
 
    }

}
