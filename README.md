# DynamicPlugIn
基于 Natasha 和 ILSpy 的运行时动态编译插件库


```C#

ReWriter reWriter = new ReWriter(dllPath);


//第一种方法：
reWriter.Builder(

     "Class1",  <---- 需要修改的类名
     
      builder => builder
                     .PublicField<string>("Name")
                     .Ctor(ctor => ctor.PublicMember.Body(@"Name=""HelloWorld!"";"))
                     
);
            
//第二种方法：
reWriter["Class1"] = "public class Class1{ ...... }";


reWriter.Complier();
reWriter.Dispose();
return reWriter.NewDllPath;

```
