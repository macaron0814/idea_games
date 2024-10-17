using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using System.Linq;

public class PlayfabManager : MonoBehaviour
{
    string idea1Name = "idea1";
    public string idea1MyName = "";
    Dictionary<string, object> allPlayerData;
    [HideInInspector] public List<string> allPlayerValue = new List<string>();

    // シングルトンインスタンス
    public static PlayfabManager instance;

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

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "572B2";

        var request = new LoginWithCustomIDRequest
        {
            CustomId = UnityEngine.Random.Range(0, 34567896763452).ToString(),
            CreateAccount = true // アカウントがなければ作成する
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab login successful!");
        ExecuteCloudScript();
        LoadPlayerData();
    }
    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    /// <summary>
    /// ロード
    /// </summary>
    private void LoadPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnDataReceiveError);
    }

    void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(idea1Name))
        {
            idea1MyName = result.Data[idea1Name].Value;
            Debug.Log("idea1: " + result.Data[idea1Name].Value);
        }
    }
    void OnDataReceiveError(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve player data: " + error.GenerateErrorReport());
    }

    /// <summary>
    /// セーブ
    /// </summary>
    public void SavePlayerData(string idea)
    {
        var data = new Dictionary<string, string>
        {
            { "idea1", idea },
        };

        var request = new UpdateUserDataRequest
        {
            Data = data
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendFailure);
    }

    void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Player data updated successfully!");
    }
    void OnDataSendFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update player data: " + error.GenerateErrorReport());
    }

    private void ExecuteCloudScript()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetAllPlayersData", // CloudScriptの関数名
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptSuccess, OnCloudScriptFailure);
    }

    void OnCloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log("CloudScript executed successfully!");

        // CloudScriptのログを確認する
        foreach (var log in result.Logs)
        {
            Debug.Log("Log: " + log.Message);
        }

        // CloudScript内でエラーが発生したかをチェック
        if (result.Error != null)
        {
            Debug.LogError("CloudScript Error: " + result.Error.Message);
        }

        // FunctionResultがnullかどうかチェック
        if (result.FunctionResult != null)
        {
            var jsonObject = result.FunctionResult as PlayFab.Json.JsonObject;

            if (jsonObject == null) return;

            allPlayerData = jsonObject.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var playerData in allPlayerData)
            {
                allPlayerValue.Add(playerData.Value.ToString());
                Debug.Log("Player ID: " + playerData.Key + ", idea1 Data: " + playerData.Value);
            }

            GameManager.instance.SetOtherIdeaText();
        }
        else
        {
            Debug.LogError("FunctionResult is null.");
        }
    }
    void OnCloudScriptFailure(PlayFabError error)
    {
        Debug.LogError("Failed to execute CloudScript: " + error.GenerateErrorReport());
    }
}