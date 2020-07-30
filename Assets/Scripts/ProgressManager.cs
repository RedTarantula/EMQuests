using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProgressManager : MonoBehaviour
{

    Questionary[] qp;
    string progressJson;
    string progressFilePath = FilePaths.questionariesPath;

    void Start()
    {
        if (!File.Exists(progressFilePath))
        {
            File.Create(progressFilePath);
        }
        GetJsonFile();
    }

    void GetJsonFile()
    {
        progressJson = File.ReadAllText(progressFilePath);
        qp = JsonHelper.FromJson<Questionary>(progressJson);
    }

    void SaveJsonFile()
    {
        File.WriteAllText(progressFilePath,progressJson);
    }

    void UpdateProgress(Questionary newProgress)
    {
        qp[newProgress.id] = newProgress;
    }

    void UpdateJsonString()
    {
        progressJson = JsonHelper.ToJson(qp,true);
    }
}