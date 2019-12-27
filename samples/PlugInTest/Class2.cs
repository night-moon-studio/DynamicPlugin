namespace PlugInTest
{
    public class Class2
    {
        public Class1 classTemp;
        public string Get()
        {
            classTemp = new Class1();
            return classTemp.Name;
        }

        public void Set(dynamic a)
        {
            classTemp.Name = a.Name;
        }
    }
}
