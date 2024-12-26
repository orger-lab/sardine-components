
namespace ExampleSystem
{
    public class DataSaver
    {
        private string Path { get; }
        public DataSaver(string path)
        {
            Path = path;

            if (!System.IO.Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void SaveData(string filename, byte[] data)
        {
            using (FileStream stream = new FileStream(System.IO.Path.Combine(Path, filename), FileMode.CreateNew))
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }

}
