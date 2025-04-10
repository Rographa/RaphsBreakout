using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Utilities.Editor
{
    // Thanks to haruna9x
    // https://discussions.unity.com/t/can-i-add-an-enum-value-in-the-inspector/196544
    public class EditorMethods : UnityEditor.Editor
    {
        private const string Extension = ".cs";

        public static void WriteToEnum<T>(string path, string name, ICollection<T> data)
        {
            var fullName = path + name + Extension;
            using (StreamWriter file = File.CreateText(fullName))
            {
                file.WriteLine("public enum "+ name + "{");
                var i = 0;
                foreach (var line in data)
                {
                    var lineRep = line.ToString().Replace(" ", string.Empty);
                    if (!string.IsNullOrEmpty(lineRep))
                    {
                        file.WriteLine($"      {lineRep} = {i},");
                        i++;
                    }
                }
                file.WriteLine("}");
            }
            AssetDatabase.ImportAsset(fullName);
        }
    }
}