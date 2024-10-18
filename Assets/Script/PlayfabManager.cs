using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using System.Linq;
using PlayFab.AdminModels;
using PlayFab.PfEditor.EditorModels;
using System;

public class PlayfabManager : MonoBehaviour
{
    string idea1Name = "idea1";
    public string idea1MyName = "";
    Dictionary<string, object> allPlayerData;
    [HideInInspector] public List<string> allPlayerValue = new List<string>();

    public List<string> playerIds = new List<string>();
    public int playerCount;
    bool isSetOtherIdeaText;

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

    private void Update()
    {
        if(!isSetOtherIdeaText && playerIds.Count != 0)
        {
            if(playerCount >= playerIds.Count)
            {
                GameManager.instance.SetOtherIdeaText();
                isSetOtherIdeaText = true;
            }
        }
    }

    void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("PlayFab login successful!");
        GetAllPlayers();
        LoadPlayerData();
    }
    void OnLoginFailure(PlayFab.PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    /// <summary>
    /// ロード
    /// </summary>
    private void LoadPlayerData()
    {
        PlayFabClientAPI.GetUserData(new PlayFab.ClientModels.GetUserDataRequest(), OnDataReceived, OnDataReceiveError);
    }

    void OnDataReceived(PlayFab.ClientModels.GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(idea1Name))
        {
            idea1MyName = result.Data[idea1Name].Value;
            Debug.Log("idea1: " + result.Data[idea1Name].Value);
        }
    }
    void OnDataReceiveError(PlayFab.PlayFabError error)
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

        var request = new PlayFab.ClientModels.UpdateUserDataRequest
        {
            Data = data
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendFailure);
    }

    void OnDataSendSuccess(PlayFab.ClientModels.UpdateUserDataResult result)
    {
        Debug.Log("Player data updated successfully!");
    }
    void OnDataSendFailure(PlayFab.PlayFabError error)
    {
        Debug.LogError("Failed to update player data: " + error.GenerateErrorReport());
    }

    public void GetAllPlayers()
    {
        var request = new GetPlayersInSegmentRequest
        {
            SegmentId = "9B4A91E43298B95" // SegmentsのID
        };

        PlayFabAdminAPI.GetPlayersInSegment(request, OnGetPlayersSuccess, OnGetPlayersError);
    }

    private void OnGetPlayersError(PlayFab.PlayFabError error)
    {
        Debug.LogError("Error getting user data: " + error.GenerateErrorReport());
    }

    private void OnGetPlayersSuccess(GetPlayersInSegmentResult result)
    {
        foreach (var player in result.PlayerProfiles)
        {
            playerIds.Add(player.PlayerId);

            var request = new PlayFab.ClientModels.GetUserDataRequest
            {
                PlayFabId = player.PlayerId,
                Keys = null // 特定のキーを指定する場合はここに追加
            };

            PlayFabClientAPI.GetUserData(request,
        result =>
        {
            // 成功時の処理
            playerCount++;
            if (result.Data != null)
            {
                foreach (var data in result.Data)
                {
                    allPlayerValue.Add(data.Value.Value);
                    Debug.Log($"Player ID: {data.Key}, Title Data: {data.Value.Value}");
                }
            }
        },
        error =>
        {

        });
        }
    }

    // ユーザーアカウント削除のリクエスト
    void DeletePlayerAccount(string playerId)
    {
        var request = new DeleteMasterPlayerAccountRequest
        {
            PlayFabId = playerId
        };
        PlayFabAdminAPI.DeleteMasterPlayerAccount(request, OnDeleteAccountSuccess, OnDeleteAccountError);
    }

    private void OnDeleteAccountSuccess(DeleteMasterPlayerAccountResult result)
    {
        Debug.Log("Player account deleted successfully.");
    }


    // エラー時のコールバック
    private void OnDeleteAccountError(PlayFab.PlayFabError error)
    {
        Debug.LogError("Error deleting player account: " + error.ErrorMessage);
    }
}