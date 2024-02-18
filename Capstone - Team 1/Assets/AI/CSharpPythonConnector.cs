using System.Diagnostics;
using System.IO;

using UnityEngine;

public class AI : MonoBehaviour
{
    void Start()
    {
        //Editible Vars
        string pythonInterpreter = @"C:\Users\99\AppData\Local\Programs\Python\Python310\python.exe"; 
        string charDataToSend = "O                        X"; //25 Chars, either "X" "O" " " and the last char (26th) is the team you are generating for

        //Proccesses and paths
        string filePath = Path.Combine(Application.dataPath, "AI", "PythonMoveRequestReceiver.py");
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonInterpreter,
            Arguments = $"\"{filePath}\"",
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
        sw.WriteLine(charDataToSend);
        sw.Flush();
        sw.Close();

        string dataFromPython = sr.ReadToEnd();
        string errors = errorReader.ReadToEnd();

        UnityEngine.Debug.Log("Python Output: " + dataFromPython);

        if (!string.IsNullOrEmpty(errors))
        {
            UnityEngine.Debug.LogError("Python Errors: " + errors);
        }

        process.WaitForExit();
        process.Close();
    }
}
