using MySql.Data.MySqlClient;

namespace PlugInTest
{
    public class Class1
    {
        public Class1()
        {

            try
            {
                MySqlConnection sqlconnection = new MySqlConnection("123");
                sqlconnection.Open();
            }
            catch (System.Exception)
            {

                Name = "更改前";
            }
          
        }
        public string Name;

    }
}
