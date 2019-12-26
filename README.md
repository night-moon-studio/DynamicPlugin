# DynamicPlugIn
基于 Natasha 和 ILSpy 的运行时动态编译插件库


```C#

ReWriter reWriter = new ReWriter(dllPath);

reWriter.Builder(

     "Class1",  <---- 需要修改的类名
     
      builder => builder
                     .PublicField<string>("Name")
                     .Ctor(ctor => ctor.PublicMember.Body(@"Name=""HelloWorld!"";"))
                     
);
            
//或者
reWriter["Class1"] = "public class Class1{ ...... }";

reWriter.Complier();
reWriter.Dispose();
return reWriter.NewDllPath;

```
