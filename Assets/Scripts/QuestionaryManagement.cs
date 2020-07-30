using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class QuestionaryManagement : MonoBehaviour
{

    public struct QuestionStructure
    {
        public string[] texts;
        public string[] imgs;
        public bool startWithImg;
        public char[] order;

        public QuestionStructure(string[] _texts, string[] _imgs,char[] elementsOrder, bool startImage = false)
        {
            texts = _texts;
            imgs = _imgs;
            order = elementsOrder;
            startWithImg = startImage;
        }
    }

    Question[] questions;
    int questionsAmmount;
    int rightAnswers = 0;
    int currentQuestion;
    string jsonString;

    public Text progressText;
    public Text titleText;
    public Text questionText;

    public GameObject questionObject;
    public GameObject resultsObject;

    int grade;

    public Text resultsText;
    public Text gradeText;
    public Text bestGradeText;

    int stars;
    public Image star1,star2,star3;
    public Sprite emptyStar, halfStar, fullStar;

    public GameObject questionViewPort;
    public GameObject questionTextObj;
    public GameObject questionImageObj;
    public GameObject questionGapObj;


    void Start()
    {
        jsonString = File.ReadAllText(FilePaths.selectedQuestionaryPath);
        questions = JsonHelper.FromJson<Question>(jsonString);
        currentQuestion = MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion;
        questionsAmmount = questions.Length;
        titleText.text = MenuVariables.selectedQuestionaryObj.title;
        UpdateFields();
    }

    public void Results()
    {
        questionObject.SetActive(false);
        resultsObject.SetActive(true);
        titleText.text = "Results";
        UpdateResults();
    }

    void UpdateResults()
    {
        resultsText.text = questionsAmmount.ToString() + " Perguntas\n" +
            rightAnswers.ToString() + " Acertadas\n" +
            (questionsAmmount - rightAnswers).ToString() + " Erradas\n";

        grade = (int)((float)rightAnswers / (float)questionsAmmount * 100f);
        gradeText.text = grade.ToString() + "%";

        if (grade > MenuVariables.selectedQuestionaryObj.bestscore)
        {
            bestGradeText.text = "NOVO MELHOR!";
            UpdateSave(grade,true);
        }
        else
        {
            bestGradeText.text = "Melhor: " + MenuVariables.selectedQuestionaryObj.bestscore.ToString() + "%";
            UpdateSave(grade,false);
        }
        GetStarsScore();
    }

    public void Interrupt()
    {
        SaveProgress();
        SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
    }

    void UpdateSave(int grade,bool bestScore)
    {
        if (bestScore)
        { MenuVariables.selectedQuestionaryObj.bestscore = grade; }

        float average = (float)(MenuVariables.selectedQuestionaryObj.averagescore * MenuVariables.selectedQuestionaryObj.timesAnswered);
        average += grade;
        MenuVariables.selectedQuestionaryObj.timesAnswered++;
        average /= MenuVariables.selectedQuestionaryObj.timesAnswered;
        MenuVariables.selectedQuestionaryObj.averagescore = (int)average;
        MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion = 0;
        MenuVariables.selectedQuestionaryObj.correctAnswers = 0;
        MenuVariables.SaveQuestionary();

    }

    public void SaveProgress()
    {
        //Debug.Log("Saving Progress");
        MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion = currentQuestion;
        //Debug.Log("Saving currentQuestion: " + currentQuestion.ToString());
        MenuVariables.selectedQuestionaryObj.correctAnswers = rightAnswers;
        //Debug.Log("Saving rightAnswers: " + rightAnswers.ToString());
        MenuVariables.SaveQuestionary();
        //Debug.Log("Finished Saving Progress");
    }

    public void Answer(string answer)
    {
        if (CheckIfAnsweredCorrectly(answer))
        {
            rightAnswers++;
        }
        NextQuestion();
    }

    void NextQuestion()
    {
        if (currentQuestion < questionsAmmount - 1)
        {
            currentQuestion++;
            UpdateFields();
        }
        else
        {
            Results();
        }
    }

    void UpdateFields()
    {

        foreach (Transform tf in questionViewPort.transform)
        {
            GameObject.Destroy(tf.gameObject);
        }

        progressText.text = "Pergunta " + (currentQuestion + 1).ToString() + " de " + questionsAmmount.ToString();
        BuildQuestion(questions[currentQuestion].question);
    }

    void BuildQuestion(string s)
    {
        CreateGap();
        QuestionStructure qs = FindImageTags(s);
        int textId = 0;
        int imgId = 0;
        foreach (char c in qs.order)
        {
            switch (c)
            {
                case 't': CreateText(qs.texts[textId]); textId++; break;
                case 'i': CreateImage(qs.imgs[imgId]); imgId++; break;
                default:
                    break;
            }
            CreateGap();
        }
        CreateAnswers();
            CreateGap();
    }

    void CreateAnswers()
    {
        string s = "Alternativas:\n\n";
        
        s += "A: " + questions[currentQuestion].answerA + "\n";
        s += "B: " + questions[currentQuestion].answerB + "\n";
        s += "C: " + questions[currentQuestion].answerC + "\n";
        s += "D: " + questions[currentQuestion].answerD + "\n";
        s += "E: " + questions[currentQuestion].answerE;

        CreateText(s);
    }

    void CreateGap()
    {
        GameObject go = Instantiate(questionGapObj);
        go.transform.SetParent(questionViewPort.transform);
        go.transform.localScale = new Vector3(1f,1f);
    }

    void CreateText(string txt)
    {
        GameObject go = Instantiate(questionTextObj);
        Text t = go.GetComponent<Text>();
        t.text = txt;
        go.transform.SetParent(questionViewPort.transform);
        go.transform.localScale = new Vector3(1f, 1f);
    }

    void CreateImage(string img)
    {
        float viewPortWidth = questionViewPort.GetComponent<RectTransform>().rect.width - 40;
        float maxHeight = questionViewPort.GetComponent<RectTransform>().rect.height;


        GameObject go = Instantiate(questionImageObj);
        Image i = go.GetComponent<Image>();
        Sprite spr = Resources.Load<Sprite>(FilePaths.imagesPath+ img);

        float sizeRatio = 1f;

        sizeRatio = spr.rect.height / spr.rect.width;

        float newWidth = 1f;
        float newHeight = 1f;

        if(sizeRatio <= 1)
        {
            newWidth = viewPortWidth;
            newHeight = viewPortWidth * sizeRatio;
        }
        else
        {
            newHeight = viewPortWidth;
            newWidth = viewPortWidth * sizeRatio;
        }
        //Debug.Log("Ratio: " + sizeRatio);
        //Debug.Log("New Width x Height: " + newWidth + " - " + newHeight);
        

        i.overrideSprite = spr;


        go.transform.SetParent(questionViewPort.transform);
        go.transform.localScale = new Vector3(1f, 1f);

        go.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth,newHeight);

    }

    bool CheckIfAnsweredCorrectly(string answer)
    {
        if (questions[currentQuestion].correctAnswer == answer)
            return true;
        else
            return false;
    }

    void GetStarsScore()
    {
        if (grade == 100)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = fullStar;
        }
        else if (grade >= 80)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = halfStar;
        }
        else if (grade >= 60)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = emptyStar;
        }
        else if (grade >= 50)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = halfStar;
            star3.sprite = emptyStar;
        }
        else if (grade >= 25)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }
        else if (grade > 0)
        {
            stars = 3;
            star1.sprite = halfStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }
        else
        {
            stars = 0;
            star1.sprite = emptyStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }

    }
    QuestionStructure FindImageTags(string s)
    {
    List<string> questText = new List<string>();
    List<string> images = new List<string>();
        List<char> order = new List<char>();
        bool first = true;
        bool startWithImage = false;

        int startPos = s.IndexOf("[img=");
        int endPos = s.IndexOf("]");
        while (startPos >= 0 && endPos >= 0)
        {
            if(first) { if (startPos == 0) { startWithImage = true; } first = !first; }

            if (s.Substring(0, startPos) != "" && s.Substring(0, startPos) != " ")
            {
                questText.Add(s.Substring(0, startPos));
                order.Add('t');
            }
            images.Add(s.Substring(startPos+5, endPos - startPos-5));
            order.Add('i');

            s = s.Remove(0, endPos + 1);
            startPos = s.IndexOf("[img=");
            endPos = s.IndexOf("]");
            //Debug.Log("Finished a wwhile");
        }
        if (s != "")
        {
            questText.Add(s);
            s = s.Remove(0, s.ToCharArray().Length - 1);
            order.Add('t');
        }

        string[] textsArr = questText.ToArray();
        string[] imgsArr = images.ToArray();
        



        foreach (string str in textsArr)
        {
            if(str.ToCharArray()[0] == ' ')
            {
                str.Remove(0);
            }
        }
        return new QuestionStructure(textsArr, imgsArr, order.ToArray(), startWithImage);
        
    }
}

