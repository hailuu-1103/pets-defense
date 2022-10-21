namespace Utils
{
    using System.IO;
    using Newtonsoft.Json;

    public static class JsonUtil
    {
        /// <summary>
        /// Save type T to file 
        /// </summary>
        /// <param name="data">Generic type</param>
        /// <param name="path">File path</param>
        /// <typeparam name="T">Generic type</typeparam>
        public static void Save<T>(T data, string path)
        {
            var stream   = new StreamWriter(path, false);
            var jsonData = JsonConvert.SerializeObject(data);
            stream.Write(jsonData);
            stream.Close();
        }
        /// <summary>
        /// Load file T from path
        /// </summary>
        /// <param name="path">file path</param>
        /// <typeparam name="T">Generic type</typeparam>
        /// <returns>T type</returns>
        public static T Load<T>(string path) where T : new()
        {
            if (!File.Exists(path)) return new T();
            var stream   = new StreamReader(path);
            var jsonData = stream.ReadToEnd();
            var data     = JsonConvert.DeserializeObject<T>(jsonData);
            stream.Close();
            return data;
        }
        /// <summary>
        /// Convert json file to type T
        /// </summary>
        /// <param name="jsonData">json file path</param>
        /// <typeparam name="T">Generic type</typeparam>
        /// <returns>T type</returns>
        public static T Convert<T>(string jsonData) where T : new()
        {
            var data = JsonConvert.DeserializeObject<T>(jsonData);
            return data;
        }
    }
}