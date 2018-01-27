using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance { get; private set; }

    public LocalizationManager m_LocalDataMng;
    public CloudDataManager m_CloudDataMng;

    protected void OnEnable()
    {
        if (PlayerDataManager.instance == null)
        {
            PlayerDataManager.instance = this;
        }
        else if (PlayerDataManager.instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // init
        // m_LocalDataMng.LoadLocalizedData();
    }

    #region Get data
    public List<int> GetData_HeroList()
    {
        return m_LocalDataMng.GetData_HeroList();
    }
    // get length of hero list
    public int GetData_LengthOfHeroList()
    {
        return GetData_HeroList().Count;
    }
    // get weapons list by hero ID
    public List<int> GetData_HeroWeaponList(int heroID)
    {
        return m_LocalDataMng.GetData_HeroWeaponList(heroID);
    }
    // get weapon by index
    public int GetData_WeaponID(int weaponIndex, int heroID)
    {
        return m_LocalDataMng.GetData_WeaponID(weaponIndex, heroID);
    }
    // get cur select hero
    public int GetData_CurSelectHeroID()
    {
        return m_LocalDataMng.GetData_CurSelectHeroID();
    }
    public long GetData_CoinNum()
    {
        return m_LocalDataMng.GetData_CoinNum();
    }
    #endregion

    #region Set data
    // set data
    public void SetData_SelectHero(int selectHero, bool save = true)
    {
        m_LocalDataMng.SetData_SelectHero(selectHero);

        if (save)
        {
            m_LocalDataMng.SaveData_CurHeroID();
            m_LocalDataMng.SaveData_Heros();
            m_CloudDataMng.SaveData_CurHeroID();
            m_CloudDataMng.SaveData_Heros();
        }
    }
    // set weapon by hero id
    public void SetData_Weapon(int heroID, int weaponIndex, int weaponID,
        bool save = true)
    {
        m_LocalDataMng.SetData_Weapon(heroID, weaponIndex, weaponID);

        if (save)
        {
            m_LocalDataMng.SaveData_Heros();
            m_LocalDataMng.SaveData_Weapons();
            m_CloudDataMng.SaveData_Heros();
            m_CloudDataMng.SaveData_Weapons();
        }
    }
    public void SetData_CoinNum(long num, bool save = true)
    {
        m_LocalDataMng.SetData_CoinNum(num);

        if (save)
        {
            m_LocalDataMng.SaveData_CoinNum();
            m_CloudDataMng.SaveData_CoinNum();
        }
    }
    #endregion

    // save
    public void SaveData_All()
    {
        m_LocalDataMng.SaveData_All();
        m_CloudDataMng.SaveData_All();
    }

    // show 
    public void ShowData_All()
    {
        m_LocalDataMng.ShowData_All();
        m_CloudDataMng.ShowData_All();
    }

    // check data
    public bool CheckData(string dataKey, string data1, string data2,
        out string corString)
    {
        corString = "";
        if (data1 == data2)
        {
            corString = data1;
            return true;
        }

        bool[] isCorrect = new bool[2] { true, true };
        int idx = 0;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Error = delegate (object sender, ErrorEventArgs args)
            {
                isCorrect[idx] = false;
                args.ErrorContext.Handled = true;
            },
            Converters = { new IsoDateTimeConverter() }
        };

        switch (dataKey)
        {
            case Constants.DataKey_Heros:
                {
                    idx = 0;
                    Dictionary<int, SerializedDomain_Hero> dic1 =
                        JsonConvert.DeserializeObject<Dictionary<int, SerializedDomain_Hero>>(
                            data1, settings
                        );
                    idx = 1;
                    Dictionary<int, SerializedDomain_Hero> dic2 =
                    JsonConvert.DeserializeObject<Dictionary<int, SerializedDomain_Hero>>(
                        data2, settings
                    );
                    if (isCorrect[0] && isCorrect[1])
                    {
                        // TODO: get the correct string
                        corString = data1;
                        return true;
                    }
                    else if (isCorrect[0] && !isCorrect[1])
                    {
                        corString = data1;
                        return true;
                    }
                    else if (!isCorrect[0] && isCorrect[1])
                    {
                        corString = data2;
                        return true;
                    }
                    else
                    {
                        // all false
                        return false;
                    }
                }
                break;
            case Constants.DataKey_Weapons:
                {
                    idx = 0;
                    Dictionary<int, SerializedDomain_Weapon> dic1 =
                        JsonConvert.DeserializeObject<Dictionary<int, SerializedDomain_Weapon>>(
                            data1, settings
                        );
                    idx = 1;
                    Dictionary<int, SerializedDomain_Weapon> dic2 =
                    JsonConvert.DeserializeObject<Dictionary<int, SerializedDomain_Weapon>>(
                        data2, settings
                    );
                    if (isCorrect[0] && isCorrect[1])
                    {
                        // TODO: get the correct string
                        corString = data1;
                        return true;
                    }
                    else if (isCorrect[0] && !isCorrect[1])
                    {
                        corString = data1;
                        return true;
                    }
                    else if (!isCorrect[0] && isCorrect[1])
                    {
                        corString = data2;
                        return true;
                    }
                    else
                    {
                        // all false
                        return false;
                    }
                }
                break;
            case Constants.DataKey_CurHeroID:
                {
                    int val1 = Convert.ToInt32(data1);
                    int val2 = Convert.ToInt32(data2);

                    isCorrect[0] = val1 >= 0;
                    isCorrect[1] = val2 >= 0;
                    if (isCorrect[0] && isCorrect[1])
                    {
                        // TODO: get the correct string
                        corString = data1;
                        return true;
                    }
                    else if (isCorrect[0] && !isCorrect[1])
                    {
                        corString = data1;
                        return true;
                    }
                    else if (!isCorrect[0] && isCorrect[1])
                    {
                        corString = data2;
                        return true;
                    }
                    else
                    {
                        // all false
                        return false;
                    }
                }
                break;
            case Constants.DataKey_CoinNum:
                {
                    long val1 = Convert.ToInt64(data1);
                    long val2 = Convert.ToInt64(data2);

                    isCorrect[0] = val1 >= 0;
                    isCorrect[1] = val2 >= 0;
                    if (isCorrect[0] && isCorrect[1])
                    {
                        // TODO: get the correct string
                        corString = data1;
                        return true;
                    }
                    else if (isCorrect[0] && !isCorrect[1])
                    {
                        corString = data1;
                        return true;
                    }
                    else if (!isCorrect[0] && isCorrect[1])
                    {
                        corString = data2;
                        return true;
                    }
                    else
                    {
                        // all false
                        return false;
                    }
                }
                break;
        }

        return false;
    }

}
