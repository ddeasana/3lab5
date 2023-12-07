using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DAL
{
#pragma warning disable SYSLIB0011
    public class CustomProvider<T> : IProvider<T> where T : class
    
    {
        string FileName = "";
        IFormatter formatter = new BinaryFormatter();
        public CustomProvider(string fileName)
        {
            FileName = AppDomain.CurrentDomain.BaseDirectory + fileName;
            using StreamWriter w = File.AppendText(FileName);
        }
        public void Save(List<T> obj)
        {
            using (FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, obj);
            }
        }
       
        public List<T> Load()
        {
            List<T> deserialised = new List<T>();
            using (FileStream filestream = new FileStream(FileName, FileMode.Open))
            {
                if (filestream.Length != 0)
                {
                    deserialised = (List<T>)formatter.Deserialize(filestream);
                }
            }
            return deserialised;
        }
    }
}