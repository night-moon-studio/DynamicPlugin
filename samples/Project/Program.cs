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
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "core2", "PlugInTest.dll");
            ShowPlugin(path);
            path = ReWrite(path);
            ShowPlugin(path);
            Console.ReadKey();
        }


        public static string ReWrite(string path)
        {
            //  上面路径 使用 core3 第二个参数就不用传了
            //  上面路径 使用 core2 第二个参数需要传false
            ReWriter reWriter = new ReWriter(path,false);
            reWriter.Builder(
                "Class1",
                builder => builder
                                    .Using("MySql.Data.MySqlClient")
                                    .PublicField<string>("Name")
                                     .Ctor(ctor => ctor.PublicMember.Body(@"try
            {{
                MySqlConnection sqlconnection = new MySqlConnection(""23"");
                sqlconnection.Open();
        }}
            catch (System.Exception)
            {

                Name = ""更改后"";
            }"))
            );
            reWriter.References.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "core2", "MySql.Data.dll"));
            reWriter.Complier();
            reWriter.Dispose();
            return reWriter.NewDllPath;

        }


        public static void ShowPlugin(string path)
        {
            var domain = DomainManagment.Random;
            domain.LoadStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "core2", "MySql.Data.dll"));
            var assembly = domain.LoadStream(path);
            var action = NDomain.Create(domain).Action("Class2 temp = new Class2();Console.WriteLine(temp.Get());", assembly);
            action();
            action.DisposeDomain();
        }
    }
}
