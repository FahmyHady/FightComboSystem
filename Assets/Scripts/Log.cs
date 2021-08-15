using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    static Text logText;
    static int count;
  static  Log instance;
    private void Start()
    {
        instance = this;
        logText = GetComponentInChildren<Text>();
    }
    public static void Print(object print)
    {
        string whatToPrint = print.ToString();
        count++;
        if (count == 4) Clear();
        if (logText.text != "") logText.text += "\n";
        logText.text += whatToPrint;
    }

    public static void Clear()
    {
        count = 0;
        logText.text = "";
    }
    public static void ClearAfterDelay(float delay)
    {
        instance.StartCoroutine(Delay(Clear, delay));
    }
    static IEnumerator Delay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}
