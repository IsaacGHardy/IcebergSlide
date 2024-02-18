using System;
using System.Diagnostics;
using System.IO;

using UnityEngine;

public class AI : MonoBehaviour
{
    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "AI", "AI.py");
        string pythonInterpreter = @"C:\Users\99\AppData\Local\Programs\Python\Python310\python.exe";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonInterpreter,
            Arguments = $"\"{filePath}\"", // Enclose the file path in double quotes
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        Process process = new Process { StartInfo = startInfo };
        process.Start();

        StreamWriter sw = process.StandardInput;
        StreamReader sr = process.StandardOutput;
        StreamReader errorReader = process.StandardError;

        // Sending data to Python
        string dataToSend = "OOOOOOOOOOOOOOOOOOOOOOOOX";
        sw.WriteLine(dataToSend);
        sw.Flush();
        sw.Close(); // Close the StandardInput stream to signal the end of input

        string dataFromPython = sr.ReadToEnd();
        string errors = errorReader.ReadToEnd();

        UnityEngine.Debug.Log("Hello, Unity!");
        UnityEngine.Debug.Log("Python Output: " + dataFromPython);

        if (!string.IsNullOrEmpty(errors))
        {
            UnityEngine.Debug.LogError("Python Errors: " + errors);
        }

        process.WaitForExit();
        process.Close();
    }
}
