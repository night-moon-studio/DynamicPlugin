using DynamicPlugin;
using Natasha;
using System;
using System.IO;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "PlugInTest.dll");
            ShowPlugin(path);
            path = ReWrite(path);
            ShowPlugin(path);
            Console.ReadKey();
        }

        public static string ReWrite(string path)
        {

            ReWriter reWriter = new ReWriter(path);
            reWriter.Builder(
                "Class1", 
                builder => builder
                                    .PublicField<string>("Name")
                                     .Ctor(ctor => ctor.PublicMember.Body(@"Name=""HelloWorld!"";"))

            );
            reWriter.Complier();
            return reWriter.NewDllPath;

        }
        public static void ShowPlugin(string path)
        {
            var domain = DomainManagment.Random;
            var assembly =  domain.LoadStream(path);
            var action = NDomain.Create(domain).Action("Class2 temp = new Class2();Console.WriteLine(temp.Get());", assembly);
            action();
            action.DisposeDomain();
        }
    }
}
