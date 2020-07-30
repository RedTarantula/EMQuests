using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringHelpers : MonoBehaviour
{
    public string str = "TESTE TESTE TESTE [img=2] TESTE TESTE [img=5]";

    public List<string> questText = new List<string>();
    public List<string> images = new List<string>();
    public List<char> order = new List<char>();
    public bool startWithImage = false;

    void Start()
    {
        FindImageTags(str);
    }

    void FindImageTags(string s)
    {
        bool first = true;

        int startPos = s.IndexOf("[img=");
        int endPos = s.IndexOf("]");
        while (startPos >= 0 && endPos >= 0)
        {
            if (first) { if (startPos == 0) { startWithImage = true; } first = !first; }

            if (s.Substring(0, startPos) != "" && s.Substring(0, startPos) != " ")
            {
                questText.Add(s.Substring(0, startPos));
                order.Add('t');
            }
            images.Add(s.Substring(startPos, endPos - startPos + 1));
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

        foreach (string str in questText)
        {
            if (str.ToCharArray()[0] == ' ')
            {
                str.Remove(0);
            }
        }
    }
    }
