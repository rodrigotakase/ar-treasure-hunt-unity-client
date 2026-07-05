using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TreasureHuntApi : MonoBehaviour
{
    public string baseUrl = "https://treasurehunt.kaiguritech.com";

    string userId;

    public string UserId
    {
        get
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = PlayerPrefs.GetString("treasureHunt.userId", "");
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString("treasureHunt.userId", userId);
                    PlayerPrefs.Save();
                }
            }
            return userId;
        }
    }

    public void GetTreasure(string treasureId, Action<TreasureInfo> onSuccess, Action<string> onError)
    {
        var url = baseUrl + "/api/treasures/" + treasureId + "?user_id=" + UserId;
        StartCoroutine(Get(url, onSuccess, onError));
    }

    public void GetHuntTreasures(int huntId, Action<HuntTreasures> onSuccess, Action<string> onError)
    {
        var url = baseUrl + "/api/hunts/" + huntId + "/treasures?user_id=" + UserId;
        StartCoroutine(Get(url, onSuccess, onError));
    }

    public void Collect(string treasureId, string nickname, Action<CollectResult> onSuccess, Action<string> onError)
    {
        var body = new CollectRequest
        {
            user_id = UserId,
            nickname = nickname,
            treasure_id = treasureId
        };
        StartCoroutine(Post(baseUrl + "/api/collect", JsonUtility.ToJson(body), onSuccess, onError));
    }

    public void GetLeaderboard(int huntId, Action<Leaderboard> onSuccess, Action<string> onError)
    {
        StartCoroutine(Get(baseUrl + "/api/hunts/" + huntId + "/leaderboard", onSuccess, onError));
    }

    IEnumerator Get<T>(string url, Action<T> onSuccess, Action<string> onError)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            HandleResponse(request, onSuccess, onError);
        }
    }

    IEnumerator Post<T>(string url, string json, Action<T> onSuccess, Action<string> onError)
    {
        using (var request = UnityWebRequest.Post(url, json, "application/json"))
        {
            yield return request.SendWebRequest();
            HandleResponse(request, onSuccess, onError);
        }
    }

    void HandleResponse<T>(UnityWebRequest request, Action<T> onSuccess, Action<string> onError)
    {
        var text = request.downloadHandler != null ? request.downloadHandler.text : "";
        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(ParseError(text, request.error));
            return;
        }
        T parsed;
        try
        {
            parsed = JsonUtility.FromJson<T>(text);
        }
        catch (Exception)
        {
            onError?.Invoke("Unexpected response from server");
            return;
        }
        onSuccess?.Invoke(parsed);
    }

    string ParseError(string text, string fallback)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                var parsed = JsonUtility.FromJson<ApiError>(text);
                if (parsed != null && !string.IsNullOrEmpty(parsed.error))
                    return parsed.error;
            }
            catch (Exception)
            {
            }
        }
        return fallback;
    }

    [Serializable]
    class CollectRequest
    {
        public string user_id;
        public string nickname;
        public string treasure_id;
    }

    [Serializable]
    class ApiError
    {
        public string error;
    }
}

[Serializable]
public class TreasureInfo
{
    public string id;
    public string name;
    public string color;
    public string hint;
    public string location;
    public int hunt_id;
    public string hunt_name;
    public bool collected;
}

[Serializable]
public class HuntTreasures
{
    public int hunt_id;
    public string hunt_name;
    public HuntTreasure[] treasures;
}

[Serializable]
public class HuntTreasure
{
    public string id;
    public string name;
    public string color;
    public string hint;
    public string location;
    public bool collected;
}

[Serializable]
public class CollectResult
{
    public bool alreadyCollected;
    public string collected_at;
    public CollectedTreasure treasure;
}

[Serializable]
public class CollectedTreasure
{
    public string id;
    public string name;
    public string color;
    public string hint;
    public string location;
    public int hunt_id;
    public string hunt_name;
}

[Serializable]
public class Leaderboard
{
    public int hunt_id;
    public string hunt_name;
    public LeaderboardEntry[] leaderboard;
}

[Serializable]
public class LeaderboardEntry
{
    public int rank;
    public string nickname;
    public int collected;
    public string last_collected_at;
}
