using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene,LoadSceneMode.Single);
    }

    public void ChooseDiscipline(int disciplineID)
    {
        MenuVariables.selectedDiscipline = disciplineID;
        FilePaths.selectedDisciplinePath = FilePaths.questionariesPath + PathHelper.DisciplineIdToPath(disciplineID) + "/";
    }

    public void ClearProgress()
    {
        MenuVariables.RemoveAllProgress();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    public void MailSupport()
    {
        Application.OpenURL("mailto:lunyx@lunyxstudios.com" + "?subject=Aplicativo EM Quiz");
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}