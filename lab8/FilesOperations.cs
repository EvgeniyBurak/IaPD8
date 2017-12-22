using System;
using System.IO;

namespace lab8
{
    class FilesOperations
    {
        public static bool FixOrCreate(string path)
        {
            if (!Create(path))
            {
                RemoveReadOnly(path);
                return true;
            }
            return false;
        }

        public static void Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void Move(string oldPath, string newPath)
        {
            try
            {
                if (File.Exists(newPath))
                {
                    Delete(newPath);
                }
                File.Move(oldPath, newPath);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void Write(string path, string content, bool append)
        {
            FixOrCreate(path);
            try
            {
                using (StreamWriter file = new StreamWriter(path, append))
                {
                    file.WriteLine(content);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static string Read(string path) => 
            FixOrCreate(path) ? File.ReadAllText(path) : null;

        public static long ContentLength(string path) => 
            new FileInfo(path).Length;

        private static void RemoveReadOnly(string path)
        {
            try
            {
                var attributes = File.GetAttributes(path);
                if (attributes.HasFlag(FileAttributes.ReadOnly))
                {
                    File.SetAttributes(path,
                        attributes & ~FileAttributes.ReadOnly);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static bool Create(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                return true;
            }
            return false;
        }
    }
}
