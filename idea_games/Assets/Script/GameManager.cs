using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text[] otherIdeaText;

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
        for (int i = 0; i < 2; i++)
        {
            otherIdeaText[i].text = PlayfabManager.instance.allPlayerValue[i];
        }
    }
}
