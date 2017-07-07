using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFeedBacks : MonoBehaviour 
{
    //Methods to give user feedback on UI
    private Text text;
    private float scaleRate = 0.1f;
    private float scaleCurrentRate = 0;
    private float roundGoalDelay = 3f;

    public void DisplayText()
    {
        text = gameObject.GetComponent<Text>();
        CancelInvoke("DisableTextUI");
        DisableTextUI();
        text.rectTransform.localScale = Vector3.one;
        Invoke("DisableTextUI", 1f);
        InvokeRepeating("TextScaleDown", 0.5f, 1.0f/60.0f);
    }

    private void DisableTextUI()
    {
        text.rectTransform.localScale = Vector3.zero;
        CancelInvoke("TextScaleDown");
        scaleCurrentRate = 0;
    }

    private void TextScaleDown()
    {
        scaleCurrentRate = scaleCurrentRate + scaleRate;
        text.rectTransform.localScale = text.rectTransform.localScale * (1 - scaleRate);
    }

    public void ShowGreenText()
    {
        text = gameObject.GetComponent<Text>();
        CancelInvoke("TurnBackWhiteText");
        TurnBackWhiteText();
        text.color = Color.green;
        Invoke("TurnBackWhiteText", 0.1f);
    }

    public void ShowRedText()
    {
        text = gameObject.GetComponent<Text>();
        CancelInvoke("TurnBackWhiteText");
        TurnBackWhiteText();
        text.color = Color.red;
        Invoke("TurnBackWhiteText", 0.1f);
    }

    public void TurnBackWhiteText()
    {
        text.color = Color.white;
    }

    public void ShowRoundGoal()
    {
        roundGoalDelay = 3f;
        Debug.Log("Showing goal");
        UnshowRoundGoal();
        InvokeRepeating("ScaleUpRoundGoal", 0.1f, 1.0f / 60.0f);
    }
    public void ShowRoundGoal(float delay)
    {
        roundGoalDelay = delay;
        Debug.Log("Showing goal");
        UnshowRoundGoal();
        InvokeRepeating("ScaleUpRoundGoal", 0.1f, 1.0f / 60.0f);
    }

    public void UnshowRoundGoal()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
    }

    private void ScaleUpRoundGoal()
    {
        if (Time.timeScale == 1)
        {
            if (gameObject.GetComponent<RectTransform>().localScale.x < 1.0f)
            {
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(gameObject.GetComponent<RectTransform>().localScale.x + (scaleRate / 2), 1, 1);
            }
            else
            {
                Debug.Log("Showing goal canceled");
                CancelInvoke("ScaleUpRoundGoal");
                InvokeRepeating("ScaleDownRoundGoal", roundGoalDelay, 1.0f / 60.0f);
            }
        }
    }

    private void ScaleDownRoundGoal()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(gameObject.GetComponent<RectTransform>().localScale.x - (scaleRate / 2), 1, 1);
        if (!(gameObject.GetComponent<RectTransform>().localScale.x > 0))
        {
            Debug.Log("unShowing goal canceled");
            UnshowRoundGoal();
            CancelInvoke("ScaleDownRoundGoal");
        }
    }

}
