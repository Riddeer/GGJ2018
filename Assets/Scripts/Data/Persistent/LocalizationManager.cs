using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

public class LocalizationManager : MonoBehaviour
{
    /*
    // public static LocalizationManager instance;
    public PlayerProgress playerProgress;
    private string saveData =
    @"{
          'charactersDatas': [
              {
                  'id': '0',
                  'weapons': [
                     '0',
                     '-1'
                  ],
                  'select': '0'
              },
              {
                  'id': '1',
                  'weapons': [
                     '1',
                     '2'
                  ],
                  'select': '1'
              }
           ],
          'weaponsData': [
              {
                  'id': '0'
              }
           ],
          'playerData': {
              'coin': '50'
           }
      }";
    private string characterData =
    @"
              {
                  'id': '-1',
                  'weapons': [
                     '-1',
                     '-1'
                  ],
                  'select': '-1'
              }";
    public string fileName = "saveData.json";
    private JObject m_SaveDataObject;
    public JObject SaveDataObject
    {
        get{return m_SaveDataObject;}
        set{m_SaveDataObject = value;}
    }
    // void Awake()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //     }
    //     else if (instance != this)
    //     {
    //         Destroy(gameObject);
    //     }
    //     DontDestroyOnLoad(gameObject);
    //     CheckGameData();
    // }
    public void LoadLocalizedData()
    {
        // string filePath = Path.Combine(Application.streamingAssetsPath,filename);
        // string filePath	= System.IO.Path.Combine(Application.streamingAssetsPath,filename);

        string filePath = Path.Combine(Application.persistentDataPath, fileName);   
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, saveData);
        }
        string data = File.ReadAllText(filePath);
        m_SaveDataObject = JObject.Parse(data);


        // test
        SerializationLocalData_All newData = new SerializationLocalData_All();
        string newJson = JsonConvert.SerializeObject(newData);
        Debug.Log("new json : \n" + newJson);
        SerializationLocalData_All desData = 
            JsonConvert.DeserializeObject<SerializationLocalData_All>(newJson);
        if (desData.heros.ContainsKey(0))
        {
            SerializationLocalData_Hero hero = desData.heros[0];
            hero.weaponsID[0] = 3;
        }
        newJson = JsonConvert.SerializeObject(desData);
        Debug.Log("changed json : \n" + newJson);
        SerializationLocalData_All checkData = 
            JsonConvert.DeserializeObject<SerializationLocalData_All>(newJson);
        Debug.Log("checked json : \n" + checkData.heros[0].weaponsID[0]);
    }


    // get value

    // get cur hero list
    public List<int> GetData_HeroList()
    {
        var jt = m_SaveDataObject["charactersDatas"];
        var herosID = jt.FindTokens("id");
        List<int> result = new List<int>();

        foreach (var one in herosID)
        {
            var id = one.ToString();
            result.Add(int.Parse(id));
        }
        return result;
    }
    // get hero by id
    private JToken GetData_HeroJTokenById(int id)
    {
        JToken result;
        result = null;
        var charactersDatas = m_SaveDataObject["charactersDatas"];
        var ids = charactersDatas.FindTokens("id");
        foreach (var one in ids)
        {
            if (one.ToString() == id.ToString())
            {
                result = one.Parent.Parent;
            }
        }
        if (result == null)
        {
            Debug.LogError("Hero is Null !  ID: " + id+"  Method: GetData_HeroJTokenById()");
        }
        return result;
    }
    // get length of hero list
    public int GetData_LengthOfHeroList()
    {
        return GetData_HeroList().Count;
    }
    // get weapons list by hero ID
    public List<int> GetData_WeaponList(int heroID)
    {
        List<int> result = new List<int>();
        if (GetData_HeroJTokenById(heroID) != null)
        {
            var weapons = GetData_HeroJTokenById(heroID)["weapons"].Values();
            foreach (var weapon in weapons)
            {
                result.Add(int.Parse(weapon.ToString()));
            }
        }
        else
        {
            Debug.LogError("Hero is Null !  ID: "+heroID +"  Method: GetData_WeaponList()");
        }
        return result;
    }
    // get weapon by index
    public int GetData_Weapon(int weaponindex, int heroID)
    {
        int result = -1;
        List<int> curWeaponList = this.GetData_WeaponList(heroID);
        if (weaponindex < curWeaponList.Count)
        {
            result = GetData_WeaponList(heroID)[weaponindex];
        }

        return result;
    }
    // get cur select hero
    public int GetData_CurSelectHero()
    {
        int result = -1;
        var charactersDatas = m_SaveDataObject["charactersDatas"];
        var selectValues = charactersDatas.FindTokens("select");
        foreach (var one in selectValues)
        {
            if (one.ToString() == "1")
            {
                JToken parent = one.Parent.Parent;
                var heroID = parent["id"];
                result = int.Parse(heroID.ToString());
            }
        }
        return result;
    }
    // save data

    public void SaveLocalizedData(string filename = "saveData.json")
    {
        string content = m_SaveDataObject.ToString();
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(filePath, content);
    }

    // set value

    // set select hero
    public void SetData_SelectHero(int selectHero)
    {
        for (int i = 0; i < GetData_LengthOfHeroList(); i++)
        {
            // init all select value to 0;
            m_SaveDataObject["charactersDatas"][i]["select"] = "0";
        }
        for (int i = 0; i < GetData_LengthOfHeroList(); i++)
        {
            // if value is not exit
            if (m_SaveDataObject["charactersDatas"][i]["id"].ToString() == selectHero.ToString())
            {
                m_SaveDataObject["charactersDatas"][i]["select"] = "1";
                return;
            }
        }
        // if value is not exit ,create a Jtoken
        JObject newCharacter = JObject.Parse(characterData);
        InitCharacterData(newCharacter, selectHero);
        newCharacter["select"] = "1";
        m_SaveDataObject["charactersDatas"][GetData_LengthOfHeroList() - 1]
        .AddAfterSelf(newCharacter);
        Debug.Log(m_SaveDataObject);
    }
    // set weapon by hero id
    public void SetData_Weapon(int heroID, int weaponHole, int weapon)
    {
        JToken hero = GetData_HeroJTokenById(heroID);
        if (GetData_WeaponList(heroID).Count > weaponHole && hero != null)
        {
            hero["weapons"][weaponHole] = weapon.ToString();
        }
        else if (hero != null)
        {
            Debug.LogError("hero: " + heroID + "'s weaponHole: " + weaponHole + " is not exit! ");
        }

    }
    
    // init (JObject)hero to default value 
    private void InitCharacterData(JObject o, int id)
    {
        o["id"] = id.ToString();
        o["select"] = "0";
        // init weapons value
        for (int i = 0; i < GetData_WeaponList(id).Count; i++)
        {
            SetData_Weapon(id,i,-1);
        }
    }


    private void LoadPlayerProgress()
    {
        playerProgress = new PlayerProgress();
        if (PlayerPrefs.HasKey("dataAsJson"))
        {
            playerProgress.dataAsJson = PlayerPrefs.GetString("dataAsJson");
        }
        if (PlayerPrefs.HasKey("gameVersion"))
        {
            playerProgress.gameVersion = PlayerPrefs.GetString("gameVersion");
        }
    }


    // IEnumerator loadStreamingAsset(string fileName)
    // {
    // string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
    // string result;
    // if (filePath.Contains("://") || filePath.Contains(":///"))
    // {
    // 	WWW www = new WWW(filePath);
    //     yield return www;
    //     result = www.text;
    // }
    // else
    //     result = System.IO.File.ReadAllText(filePath);
    // yield return result;
    // }
    public void ExitGame()
    {
        Application.Quit();
    }
    // public void InitGameData()
    // {
    //     for (int i = 0; i < dicData.Count; i++)
    //     {
    //         dicData[i] = -1;
    //         ChangeDictionaryValue(i, -1);

    //     }
    //     foreach (var one in m_DropMeList)
    //     {
    //         one.InitUI();
    //         // one.GetComponent<Image>().sprite = one.m_DefaultImage.sprite;
    //     }

    // }


    public void CheckGameData()
    {
        // check game version
        string gameVersion = Constants.GameVersion;
        string prefsGameVersion = PlayerPrefs.GetString("gameVersion", "-1");

        if (gameVersion == prefsGameVersion || prefsGameVersion == "-1")
        {
            LoadLocalizedData();
            return;
        }
        // else if (gameVersion != prefsGameVersion)
        // {
        //     // load json from streamingAssetsPath
        //     string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, filename);

        //     if (File.Exists(streamingAssetsPath))
        //     {
        //         string dataAsJson = File.ReadAllText(streamingAssetsPath);
        //         dataReadByJson = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        //     }
        //     else
        //     {
        //         Debug.LogError("JsonFile From StreamingAssetsPath Is Null !");
        //         return;
        //     }
        //     // load persistent data from persistentDataPath
        //     string filePath = Path.Combine(Application.persistentDataPath, filename);

        //     if (File.Exists(filePath))
        //     {
        //         string data = File.ReadAllText(filePath);
        //         loadedData = JsonUtility.FromJson<LocalizationData>(data);
        //     }
        //     else
        //     {
        //         // 版本号不一致，并且机器中没有persistent文件
        //         loadedData = dataReadByJson;
        //     }
        //     // check json data and persistent
        //     for (int i = 0; i < dataReadByJson.charactersData.Count; i++)
        //     {
        //         for (int j = 0; j < loadedData.charactersData.Count; j++)
        //         {
        //             if (dataReadByJson.charactersData[i].id == loadedData.charactersData[j].id)
        //             {
        //                 // dataReadByJson.charactersData[i].attributes = loadedData.charactersData[j].attributes;
        //             }
        //         }
        //     }
        //     // check is can be reuse		
        //     // for (int i = 0; i < dataReadByJson.charactersData.Count; i++)
        //     // {
        //     //     // if it is can not be reuse 
        //     //     // if (!IsCanReuse((CardType)dataReadByJson.charactersData[i].attributes))
        //     //     // {
        //     //         for (int j = 0; j < dataReadByJson.charactersData.Count; j++)
        //     //         {
        //     //             if (dataReadByJson.charactersData[i].attributes == dataReadByJson.charactersData[j].attributes
        //     //             && dataReadByJson.charactersData[i].id != dataReadByJson.charactersData[j].id)
        //     //             {
        //     //                 // turn it to default value
        //     //                 dataReadByJson.charactersData[j].attributes = -1;
        //     //             }
        //     //         }
        //     //     // }
        //     //     else
        //     //     {
        //     //         // do nothing
        //     //     }
        //     // }
        //     // use truly data
        //     loadedData = dataReadByJson;
        //     // repairJsonData
        //     for (int i = 0; i < loadedData.charactersData.Count; i++)
        //     {
        //         // dicData.Add(loadedData.charactersData[i].id, loadedData.charactersData[i].attributes);
        //     }
        // }
    }
    // Create json item
    // private void CreateJsonItem(int key, int value)
    // {
    //     characterData item = new characterData();
    //     item.id = key;
    //     item.attributes = value;
    //     loadedData.charactersData.Add(item);
    //     dicData.Add(key, value);
    // }
    // Delete json item

    // Update json item
    // private void UpdateJsonItem(int key, int value)
    // {
    //     if (dicData.ContainsKey(key))
    //     {
    //         dicData[key] = value;
    //     }
    //     for (int i = 0; i < loadedData.charactersData.Count; i++)
    //     {
    //         if (loadedData.charactersData[i].id == key)
    //         {
    //             loadedData.charactersData[i].attributes = value;
    //         }
    //     }
    // }

    // private void TurnLoadedDataToJsonDic()
    // {
    //     var keys = dicData.Keys;

    // 	foreach (var one in keys)
    // 	{
    //         for (int i = 0; i < dicData[one].weapons.Length; i++)
    //         {
    //             dicData[one].weapons[i] = 
    //             loadedData.charactersDatas[one].attributes.weapons[i];

    //         }
    //         dicData[one].select = 
    //         loadedData.charactersDatas[one].attributes.select;        
    // 	}
    // }
    // private List<LocalizationItem> RepairJsonData(LocalizationData data)
    // {
    // 	// LocalizationData repairJson = new LocalizationData();
    // 	List<LocalizationItem> repairJson = new List<LocalizationItem>();
    // 	// set Value To (LocalizationData)repairJson
    // 	for ( int i = 0 ; i < data.items.Count ; i++ )
    // 	{
    // 		// set Value To Item
    // 		item.key = data.items[i].key;
    // 		item.value = data.items[i].value;
    // 		// if CardType Can Not Be Reuse
    // 		if(!CardsDataManager.instance.IsCanReuse((CardType)data.items[i].value))
    // 		{	
    // 			if (repairJson != null)
    // 			{
    // 				for (int j = 0 ; j < repairJson.Count ; j++)
    // 				{
    // 					Debug.Log("Item: "+item.value);
    // 					Debug.Log("Repairjson: "+repairJson[j].value);
    // 					// if (repairJson[j].value == item.value)
    // 					// {
    // 					// 	Debug.Log("JValue: "+repairJson[j].value);
    // 					// 	Debug.Log("IValue: "+item.value);
    // 					// 	repairJson[j].value = -1;
    // 					// }
    // 				}	
    // 			}
    // 			else 
    // 			{
    // 				// do noting
    // 			}			
    // 		}
    // 		else if (CardsDataManager.instance.IsCanReuse((CardType)data.items[i].value))
    // 		{
    // 			// do noting
    // 		}
    // 		repairJson.Add(item);
    // 	}
    // 	for (int r = 0 ; r < repairJson.Count; r++)
    // 	{
    // 		Debug.Log("RKey: "+repairJson[r].key);
    // 		Debug.Log("RValue: "+repairJson[r].value);
    // 	}
    // 	return repairJson;
    // }
 

    // show data
    public void ShowData_All()
    {
        Debug.Log("Local : \n" + m_SaveDataObject);
    }
*/
    public string m_FileName = "saveData.json";
    private SerializedLocalData m_JsonData = null;
    private SerializedLocalData m_PrefData = null;

    void OnEnable()
    {
        this.InitData();
    }

    public void InitData()
    {
        // delete data
        // this.DeleteData();

        // .json
        string data = "";
        string filePath = Path.Combine(Application.persistentDataPath, m_FileName);
        if (!File.Exists(filePath))
        {
            m_JsonData = new SerializedLocalData();
            data = JsonConvert.SerializeObject(m_JsonData);
            File.WriteAllText(filePath, data);
        }
        else
        {
            data = File.ReadAllText(filePath);
            m_JsonData = JsonConvert.DeserializeObject<SerializedLocalData>(data);
        }

        // player pref
        m_PrefData = new SerializedLocalData();
        if (!PlayerPrefs.HasKey(Constants.DataKey_Heros))
        {
            SerializedLocalData newData = new SerializedLocalData();
            data = JsonConvert.SerializeObject(newData.heros);
            PlayerPrefs.SetString(Constants.DataKey_Heros, data);
        }
        else
        {
            data = PlayerPrefs.GetString(Constants.DataKey_Heros);
        }
        m_PrefData.heros = JsonConvert.DeserializeObject
            <Dictionary<int, SerializedDomain_Hero>>(data);
        if (!PlayerPrefs.HasKey(Constants.DataKey_Weapons))
        {
            SerializedLocalData newData = new SerializedLocalData();
            data = JsonConvert.SerializeObject(newData.weapons);
            PlayerPrefs.SetString(Constants.DataKey_Weapons, data);
        }
        else
        {
            data = PlayerPrefs.GetString(Constants.DataKey_Weapons);
        }
        m_PrefData.weapons = JsonConvert.DeserializeObject
            <Dictionary<int, SerializedDomain_Weapon>>(data);
        if (!PlayerPrefs.HasKey(Constants.DataKey_CurHeroID))
        {
            SerializedLocalData newData = new SerializedLocalData();
            data = newData.curHero.ToString();
            PlayerPrefs.SetString(Constants.DataKey_CurHeroID, data);
        }
        else
        {
            data = PlayerPrefs.GetString(Constants.DataKey_CurHeroID);
        }
        m_PrefData.curHero = Convert.ToInt32(data);
        if (!PlayerPrefs.HasKey(Constants.DataKey_CoinNum))
        {
            SerializedLocalData newData = new SerializedLocalData();
            data = newData.coinNum.ToString();
            PlayerPrefs.SetString(Constants.DataKey_CoinNum, data);
        }
        else
        {
            data = PlayerPrefs.GetString(Constants.DataKey_CoinNum);
        }
        m_PrefData.coinNum = Convert.ToInt64(data);

    }

    #region Show data
    public void ShowData_All()
    {
        if (m_JsonData == null || m_PrefData == null) return;
        Debug.Log("Local : \n" + 
            "Pref : \n" + JsonConvert.SerializeObject(m_PrefData) + "\n" +
            "Json : \n" + JsonConvert.SerializeObject(m_JsonData));
    }
    #endregion

    #region Save data : PlayerPref & .json
    public void SaveData_All()
    {
        if (m_JsonData == null || m_PrefData == null) return;

        this.SaveData_Heros();
        this.SaveData_Weapons();
        this.SaveData_CurHeroID();
        this.SaveData_CoinNum();
    }
    public void SaveData_Heros()
    {
        if (m_JsonData == null || m_PrefData == null) return;

        // player pref
        PlayerPrefs.SetString(Constants.DataKey_Heros,
            JsonConvert.SerializeObject(m_PrefData.heros));

        // .json
        string filePath = Path.Combine(Application.persistentDataPath, m_FileName);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(m_JsonData));
    }
    public void SaveData_Weapons()
    {
        if (m_JsonData == null || m_PrefData == null) return;

        // player pref
        PlayerPrefs.SetString(Constants.DataKey_Weapons,
            JsonConvert.SerializeObject(m_PrefData.weapons));

        // .json
        string filePath = Path.Combine(Application.persistentDataPath, m_FileName);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(m_JsonData));
    }
    public void SaveData_CurHeroID()
    {
        if (m_JsonData == null || m_PrefData == null) return;

        // player pref
        PlayerPrefs.SetString(Constants.DataKey_CurHeroID,
            m_PrefData.curHero.ToString());

        // .json
        string filePath = Path.Combine(Application.persistentDataPath, m_FileName);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(m_JsonData));
    }
    public void SaveData_CoinNum()
    {
        if (m_JsonData == null || m_PrefData == null) return;

        // player pref
        PlayerPrefs.SetString(Constants.DataKey_CoinNum,
            m_PrefData.coinNum.ToString());

        // .json
        string filePath = Path.Combine(Application.persistentDataPath, m_FileName);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(m_JsonData));
    }
    #endregion

    #region Get data
    public string GetJsonData_Heros()
    {
        if (m_JsonData == null || m_PrefData == null) return "";

        string corString = "";
        if (this.TryGetCorrectData(Constants.DataKey_Heros, out corString))
        {
            return corString;
        }

        return "";
        // return JsonConvert.SerializeObject(m_JsonData.heros);
    }
    public string GetJsonData_Weapons()
    {
        if (m_JsonData == null) return "";

        string corString = "";
        if (this.TryGetCorrectData(Constants.DataKey_Weapons, out corString))
        {
            return corString;
        }

        return "";
        // return JsonConvert.SerializeObject(m_JsonData.weapons);
    }
    public long GetData_CoinNum()
    {
        if (m_JsonData == null || m_PrefData == null) return 0;

        string corString = "";
        if (this.TryGetCorrectData(Constants.DataKey_CoinNum, out corString))
        {
            return Convert.ToInt64(corString);
        }

        return 0;
        // return m_JsonData.coinNum;
    }
    public List<int> GetData_HeroList()
    {
        if (m_JsonData == null || m_PrefData == null) return null;

        string corString = "";
        if (this.TryGetCorrectData(Constants.DataKey_Heros, out corString))
        {
            Dictionary<int, SerializedDomain_Hero> dic =
                JsonConvert.DeserializeObject<Dictionary<int, SerializedDomain_Hero>>(corString);
            return new List<int>(dic.Keys);
        }

        return null;
    }
    public List<int> GetData_HeroWeaponList(int heroID)
    {
        if (m_JsonData == null || m_PrefData == null) return null;

        if (!m_JsonData.heros.ContainsKey(heroID)) return null;
        if (!m_PrefData.heros.ContainsKey(heroID)) return null;

        string corString = "";
        if (this.TryGetCorrectData(Constants.DataKey_Heros, out corString))
        {
            Dictionary<int, SerializedDomain_Hero> dic =
                JsonConvert.DeserializeObject<Dictionary<int, SerializedDomain_Hero>>(corString);
            return new List<int>(dic[heroID].weaponsID);
        }

        return null;
    }
    public int GetData_WeaponID(int weaponIndex, int heroID)
    {
        if (weaponIndex >= 2) return -1;
        if (this.GetData_HeroWeaponList(heroID) == null) return -1;

        return this.GetData_HeroWeaponList(heroID)[weaponIndex];
    }
    public int GetData_CurSelectHeroID()
    {
        if (m_JsonData == null || m_PrefData == null) return -1;
        if (!m_JsonData.heros.ContainsKey(m_JsonData.curHero)) return -1;
        if (!m_PrefData.heros.ContainsKey(m_JsonData.curHero)) return -1;

        string corString = "";
        if (this.TryGetCorrectData(Constants.DataKey_CurHeroID, out corString))
        {
            return Convert.ToInt32(corString);
        }

        return -1;
        // return m_JsonData.curHero;
    }
    #endregion

    #region Check data return the correct data string
    private bool TryGetCorrectData(string dataKey, out string corString)
    {
        switch (dataKey)
        {
            case Constants.DataKey_Heros:
                {
                    string prefData = PlayerPrefs.GetString(dataKey);
                    string jsonData = JsonConvert.SerializeObject(m_JsonData.heros);
                    return PlayerDataManager.instance.CheckData(
                        dataKey, prefData, jsonData, out corString);
                }
            case Constants.DataKey_Weapons:
                {
                    string prefData = PlayerPrefs.GetString(dataKey);
                    string jsonData = JsonConvert.SerializeObject(m_JsonData.weapons);
                    return PlayerDataManager.instance.CheckData(
                        dataKey, prefData, jsonData, out corString);
                }
            case Constants.DataKey_CurHeroID:
                {
                    string prefData = PlayerPrefs.GetString(dataKey);
                    string jsonData = m_JsonData.curHero.ToString();
                    return PlayerDataManager.instance.CheckData(
                        dataKey, prefData, jsonData, out corString);
                }
            case Constants.DataKey_CoinNum:
                {
                    string prefData = PlayerPrefs.GetString(dataKey);
                    string jsonData = m_JsonData.coinNum.ToString();
                    return PlayerDataManager.instance.CheckData(
                        dataKey, prefData, jsonData, out corString);
                }
        }

        corString = "";
        return false;
    }
    #endregion

    #region Set data
    public void SetString(string key, string val)
    {
        switch (key)
        {
            case Constants.DataKey_Heros:
                {
                    this.SetJsonData_Heros(val);
                }
                break;
            case Constants.DataKey_Weapons:
                {
                    this.SetJsonData_Weapons(val);
                }
                break;
            case Constants.DataKey_CurHeroID:
                {
                    this.SetData_SelectHero(Convert.ToInt32(val));
                }
                break;
            case Constants.DataKey_CoinNum:
                {
                    this.SetData_CoinNum(Convert.ToInt64(val));
                }
                break;
        }
    }
    public void SetJsonData_Heros(string jsonData)
    {
        // player prefs
        m_PrefData.heros = JsonConvert.
            DeserializeObject<Dictionary<int, SerializedDomain_Hero>>(jsonData);

        // json
        m_JsonData.heros = JsonConvert.
            DeserializeObject<Dictionary<int, SerializedDomain_Hero>>(jsonData);
    }
    public void SetJsonData_Weapons(string jsonData)
    {
        // player prefs
        m_PrefData.weapons = JsonConvert.
            DeserializeObject<Dictionary<int, SerializedDomain_Weapon>>(jsonData);

        // json
        m_JsonData.weapons = JsonConvert.
            DeserializeObject<Dictionary<int, SerializedDomain_Weapon>>(jsonData);
    }
    public void SetData_SelectHero(int selectHero)
    {
        if (m_JsonData == null || m_PrefData == null) return;

        // player prefs
        m_PrefData.curHero = selectHero;

        if (!m_PrefData.heros.ContainsKey(selectHero))
        {
            SerializedDomain_Hero newHero = new SerializedDomain_Hero(selectHero);
            // newHero.ID = selectHero;
            m_PrefData.heros.Add(newHero.ID, newHero);
        }

        // json
        m_JsonData.curHero = selectHero;

        if (!m_JsonData.heros.ContainsKey(selectHero))
        {
            SerializedDomain_Hero newHero = new SerializedDomain_Hero(selectHero);
            // newHero.ID = selectHero;
            m_JsonData.heros.Add(newHero.ID, newHero);
        }
    }
    public void SetData_Weapon(int heroID, int weaponIndex, int weaponID)
    {
        if (weaponIndex >= 2) return;
        if (m_JsonData == null || m_PrefData == null) return;

        // player prefs
        if (!m_PrefData.heros.ContainsKey(heroID))
        {
            SerializedDomain_Hero newHero = new SerializedDomain_Hero(heroID);
            // newHero.ID = heroID;
            m_PrefData.heros.Add(newHero.ID, newHero);
        }
        if (!m_PrefData.weapons.ContainsKey(weaponID))
        {
            SerializedDomain_Weapon newWeapon = new SerializedDomain_Weapon(weaponID);
            // newWeapon.ID = weaponID;
            m_PrefData.weapons.Add(newWeapon.ID, newWeapon);
        }

        m_PrefData.heros[heroID].weaponsID[weaponIndex] = weaponID;

        // json
        if (!m_JsonData.heros.ContainsKey(heroID))
        {
            SerializedDomain_Hero newHero = new SerializedDomain_Hero(heroID);
            // newHero.ID = heroID;
            m_JsonData.heros.Add(newHero.ID, newHero);
        }
        if (!m_JsonData.weapons.ContainsKey(weaponID))
        {
            SerializedDomain_Weapon newWeapon = new SerializedDomain_Weapon(weaponID);
            // newWeapon.ID = weaponID;
            m_JsonData.weapons.Add(newWeapon.ID, newWeapon);
        }

        m_JsonData.heros[heroID].weaponsID[weaponIndex] = weaponID;

    }
    public void SetData_CoinNum(long num)
    {
        if (m_JsonData == null || m_PrefData == null) return;

        // player prefs
        m_PrefData.coinNum = num;
        // json
        m_JsonData.coinNum = num;

    }
    #endregion

    #region Delete data
    public void DeleteData()
    {
        // .json
        string filePath = Path.Combine(Application.persistentDataPath, m_FileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // player pref
        if (PlayerPrefs.HasKey(Constants.DataKey_Heros))
        {
            PlayerPrefs.DeleteKey(Constants.DataKey_Heros);
        }
        if (PlayerPrefs.HasKey(Constants.DataKey_Weapons))
        {
            PlayerPrefs.DeleteKey(Constants.DataKey_Weapons);
        }
        if (PlayerPrefs.HasKey(Constants.DataKey_CurHeroID))
        {
            PlayerPrefs.DeleteKey(Constants.DataKey_CurHeroID);
        }
        if (PlayerPrefs.HasKey(Constants.DataKey_CoinNum))
        {
            PlayerPrefs.DeleteKey(Constants.DataKey_CoinNum);
        }
    }
    #endregion

}

public class SerializedDomain_Hero
{
    public int ID { get; set; }
    public int[] weaponsID { get; set; }

    public SerializedDomain_Hero()
    {
        this.ID = -1;
        this.weaponsID = new int[2];
        this.weaponsID[0] = 0;
        this.weaponsID[1] = 1;
    }

    public SerializedDomain_Hero(int id)
    {
        this.ID = id;
        this.weaponsID = new int[2];
        this.weaponsID[0] = 0;
        this.weaponsID[1] = 1;
    }
    public SerializedDomain_Hero(int id, int wID1, int wID2)
    {
        this.ID = id;
        this.weaponsID = new int[2];
        this.weaponsID[0] = wID1;
        this.weaponsID[1] = wID2;
    }

    [OnError]
    internal void OnError(StreamingContext context, ErrorContext errorContext)
    {
        errorContext.Handled = true;
    }
}

public class SerializedDomain_Weapon
{
    public int ID { get; set; }

    public SerializedDomain_Weapon(int id)
    {
        this.ID = id;
    }
}

public class SerializedLocalData
{
    public Dictionary<int, SerializedDomain_Hero> heros { get; set; }
    public Dictionary<int, SerializedDomain_Weapon> weapons { get; set; }
    public int curHero { get; set; }
    public long coinNum { get; set; }

    public SerializedLocalData()
    {
        heros = new Dictionary<int, SerializedDomain_Hero>();
        SerializedDomain_Hero _defaultHero_0 = new SerializedDomain_Hero(0, 0, 1);
        // _defaultHero_0.ID = 0;
        // _defaultHero_0.weaponsID = new List<int>(2);
        // _defaultHero_0.weaponsID.Add(0);
        // _defaultHero_0.weaponsID.Add(1);
        heros.Add(_defaultHero_0.ID, _defaultHero_0);
        SerializedDomain_Hero _defaultHero_1 = new SerializedDomain_Hero(1, 1, 2);
        // _defaultHero_1.ID = 1;
        // _defaultHero_1.weaponsID = new List<int>(2);
        // _defaultHero_1.weaponsID.Add(1);
        // _defaultHero_1.weaponsID.Add(2);
        heros.Add(_defaultHero_1.ID, _defaultHero_1);
        weapons = new Dictionary<int, SerializedDomain_Weapon>();
        SerializedDomain_Weapon _defaultWeapon = new SerializedDomain_Weapon(0);
        // _defaultWeapon.ID = 0;
        weapons.Add(_defaultWeapon.ID, _defaultWeapon);
        curHero = 0;
        coinNum = 50;
    }
}