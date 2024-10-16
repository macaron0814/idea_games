using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text[] otherIdeaText;
    [SerializeField] private GameObject[] otherIdea;

    // シングルトンインスタンス
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnIdeaButton()
    {
        PlayfabManager.instance.SavePlayerData(inputField.text);
    }

    public void SetOtherIdeaText()
    {
        List<int> randomNum = new List<int>();
        for (int i = 0; i < PlayfabManager.instance.allPlayerValue.Count; i++)
        {
            randomNum.Add(i);
        }
        Shuffle<int>(randomNum);

        for (int i = 0; i < otherIdeaText.Length; i++)
        {
            if (i == otherIdeaText.Length - 1 &&
                !string.IsNullOrEmpty(PlayfabManager.instance.idea1MyName))
            {
                StartCoroutine(SetIntervalActive(otherIdea[i], i * 5.0f));
                otherIdeaText[i].text = PlayfabManager.instance.idea1MyName;
                return;
            }

            StartCoroutine(SetIntervalActive(otherIdea[i], i * 5.0f));

            if (i > randomNum.Count - 1)
            {
                otherIdeaText[i].text = PlayfabManager.instance.allPlayerValue[0];
                return;
            }
            if (randomNum[i] > PlayfabManager.instance.allPlayerValue.Count - 1) randomNum[i] = 0;
            otherIdeaText[i].text = PlayfabManager.instance.allPlayerValue[randomNum[i]];
        }
    }

    IEnumerator SetIntervalActive(GameObject item, float interval)
    {
        yield return new WaitForSeconds(interval);
        item.SetActive(true);
    }

    public void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;

        // Fisher-Yatesアルゴリズム
        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(i + 1);

            // 要素を入れ替える
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
