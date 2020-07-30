using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StatsMenuManager : MonoBehaviour
{
    Questionary[] disciplineQuestionaries;
    int questionariesSize;
    int sumAverages;
    int sumBests;
    int sumAnsweredTimes;
    int answeredQuestionaries;
    string json;
    public int discId = 0;
    public Text statsText;
    public string statsString = "0 vezes\n0%\n0%";

    void Start()
    {
        GetQuestionaries(discId);
        GetNumbers();
        UpdateText();
    }

    void GetQuestionaries(int disciplineId)
    {
        string filepath = FilePaths.questionariesPath + PathHelper.DisciplineIdToPath(disciplineId) + "/" + PathHelper.DisciplineIdToPath(disciplineId) + "_questionaries.json";

        if (File.Exists(filepath))
        {
            json = File.ReadAllText(filepath);
            disciplineQuestionaries = JsonHelper.FromJson<Questionary>(json);
        }
        else
        {
            Debug.LogWarning("Couldn't load stats from discipline: " + PathHelper.DisciplineIdToPath(disciplineId));
            Debug.LogWarning("Path: " + filepath);
        }
    }

    void GetNumbers()
    {
        questionariesSize = disciplineQuestionaries.Length;
        foreach (Questionary q in disciplineQuestionaries)
        {
            if (q.timesAnswered > 0)
            {
                sumAverages += q.averagescore;
                sumBests += q.bestscore;
                sumAnsweredTimes += q.timesAnswered;
                answeredQuestionaries++;
            }
        }
        sumAverages /= answeredQuestionaries;
        sumBests /= answeredQuestionaries;

    }

    void UpdateText()
    {
        string times = sumAnsweredTimes.ToString() + "\n";
        string averages = sumAverages.ToString() + "%\n";
        string bests = sumBests.ToString() + "%";

        statsText.text = times + averages + bests;
    }
}
