using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherIdea : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(AlphaTextAnimation());
    }

    IEnumerator AlphaTextAnimation()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += 0.01f;
            yield return null;
        }

        yield return new WaitForSeconds(10.0f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 0.01f;
            yield return null;
        }

        List<int> randomNum = new List<int>();
        for (int i = 0; i < PlayfabManager.instance.allPlayerValue.Count; i++)
        {
            randomNum.Add(i);
        }
        GameManager.instance.Shuffle<int>(randomNum);
        int rand = Random.Range(0, PlayfabManager.instance.allPlayerValue.Count);
        transform.GetChild(0).transform.GetComponent<TMP_Text>().text = PlayfabManager.instance.allPlayerValue[randomNum[rand]];

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(AlphaTextAnimation());
    }
}
