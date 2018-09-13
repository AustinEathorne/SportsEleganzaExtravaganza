using UnityEngine;
using FullSerializer;

/// <summary>
/// Base class for all Json files. Handles loading and saving the file
/// </summary>
public class JsonFile<FileClass>
{
    private static readonly fsSerializer FSerializer = new fsSerializer();

    public JsonFile() { }



    public static string SerializeText(FileClass _obj)
    {
        fsData data;
        FSerializer.TrySerialize(typeof(FileClass), _obj, out data).AssertSuccess();

        return fsJsonPrinter.CompressedJson(data);
    }

    public static bool SerializeFile(FileClass _obj, string _filePath)
    {
        string fileText = SerializeText(_obj);

        if (string.IsNullOrEmpty(fileText) == false)
        {
            EnsureDirectoryExists(_filePath);
            System.IO.StreamWriter file = new System.IO.StreamWriter(_filePath, false);

            if (file != null)
            {
                file.Write(fileText);
                file.Close();
                return true;
            }
        }

        return false;
    }


    public static FileClass DeserializeText(string _json)
    {
        fsData data = fsJsonParser.Parse(_json);

        object deserializedObj = default(FileClass);
        FSerializer.TryDeserialize(data, typeof(FileClass), ref deserializedObj).AssertSuccess();

        return (FileClass)deserializedObj;
    }

    public static FileClass DeserializeFile(string _filePath)
    {
        if (string.IsNullOrEmpty(_filePath) == false)
        {
            bool exists = EnsureDirectoryExists(_filePath, false);

            if (exists)
            {
                System.IO.FileInfo info = new System.IO.FileInfo(_filePath);
                System.IO.StreamReader file = info.OpenText();

                string fileText = file.ReadToEnd();
                file.Close();

                return DeserializeText(fileText);
            }
        }

        return default(FileClass);
    }

    public static FileClass DeserializeResource(string _resourcePath)
    {
        TextAsset resource = Resources.Load(_resourcePath) as TextAsset;

        return DeserializeText(resource.text);
    }


    private static bool EnsureDirectoryExists(string _filePath, bool _isCreatingDirectory = true)
    {
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(_filePath);

        bool exists = fileInfo.Directory.Exists;
        if (!exists && _isCreatingDirectory)
        {
            System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);
        }

        return exists;
    }
}
