using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class CloudDataManager : MonoBehaviour
{
    public bool m_UseCloudServices = true;

    private bool m_Inited = false;
    private LoadingLayer m_LoadingLayer;

    void OnEnable()
    {
        CloudServices.KeyValueStoreDidInitialiseEvent += OnKeyValueStoreInitialised;
        CloudServices.KeyValueStoreDidSynchroniseEvent += OnKeyValueStoreDidSynchronise;
        CloudServices.KeyValueStoreDidChangeExternallyEvent += OnKeyValueStoreChanged;

        this.GetLoadingLayer();

        if (m_UseCloudServices)
        {
            this.InitData();
        }
    }

    void OnDisable()
    {
        CloudServices.KeyValueStoreDidInitialiseEvent -= OnKeyValueStoreInitialised;
        CloudServices.KeyValueStoreDidSynchroniseEvent -= OnKeyValueStoreDidSynchronise;
        CloudServices.KeyValueStoreDidChangeExternallyEvent -= OnKeyValueStoreChanged;
    }

    void Start()
    {
        this.GetLoadingLayer();

    }

    private void GetLoadingLayer()
    {
        if (m_LoadingLayer == null)
        {
            GameObject go =
               GameObject.FindGameObjectWithTag(Constants.SceneObject_LoadingLayer);
            if (go != null)
            {
                m_LoadingLayer = go.GetComponent<LoadingLayer>();
            }
        }
    }

    public void EnableCloudService(bool enable)
    {
        m_UseCloudServices = enable;

        if (enable)
        {
            if (m_Inited) return;

            this.InitData();
        }
        else
        {
            if (m_LoadingLayer != null)
                m_LoadingLayer.SetVisible(false);
        }
    }

    public void InitData()
    {
        // #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //         // disable cloud data
        // #else
        NPBinding.CloudServices.Initialise();
        // Debug.Log("Initialise send!!!");

        // if (m_LoadingLayer != null)
        //     m_LoadingLayer.SetVisible(true);
        // #endif
    }


    private void OnKeyValueStoreInitialised(bool _success)
    {
        if (_success)
        {
            if (!m_Inited) m_Inited = true;

            if (m_LoadingLayer != null)
                m_LoadingLayer.SetVisible(false);

            Debug.Log("Successfully initialise cloud services.");

            this.CheckAllDataWhenInit();
            // NPBinding.CloudServices.Synchronise();
            // Debug.Log("Synchronise send!!!");
        }
        else
        {
            Debug.Log("Failed to initialise cloud services.");
        }

    }
    private void OnKeyValueStoreDidSynchronise(bool _success)
    {
        if (m_LoadingLayer != null)
            m_LoadingLayer.SetVisible(false);
        if (_success)
        {
            Debug.Log("Successfully synchronised in-memory keys and values.");
        }
        else
        {
            Debug.Log("Failed to synchronise in-memory keys and values.");
        }
    }
    private void OnKeyValueStoreChanged(eCloudDataStoreValueChangeReason _reason,
        string[] _changedKeys)
    {
        Debug.Log("Cloud key-value store has been changed.");
        Debug.Log(string.Format("Reason: {0}.", _reason));

        if (_changedKeys != null)
        {
            Debug.Log(string.Format("Total keys changed: {0}.", _changedKeys.Length));
            Debug.Log(string.Format("Pick a value from old and new and set the value to cloud for resolving conflict."));

            foreach (string oneKey in _changedKeys)
            {
                string corString = NPBinding.CloudServices.GetString(oneKey);

                PlayerDataManager.instance.m_LocalDataMng.SetString(oneKey, corString);

                // if (this.TryGetCorrectData(oneKey, out corString))
                // {
                //     NPBinding.CloudServices.SetString(
                //         oneKey, corString);
                // }
                // else
                // {
                //     // cannot check , let player choose one
                //     // TODO: show dialog

                // }
            }
            PlayerDataManager.instance.m_LocalDataMng.SaveData_All();

            // foreach (string _changedKey in _changedKeys)
            // {
            //     if (_changedKey.Equals(KEY_FOR_LONG)) // Shows example of resolving a conflict
            //     {
            //         long _newCloudLongValue = NPBinding.CloudServices.GetLong(_changedKey);
            //         Debug.Log(string.Format("New value for key: {0} is {1}. Old value is {2}", _changedKey, _newCloudLongValue, m_longValue));
            //         // Lets assume, we pick the bigger value and set it here.
            //         long _biggerValue = _newCloudLongValue > m_longValue ? _newCloudLongValue : m_longValue;

            //         NPBinding.CloudServices.SetLong(_changedKey, _biggerValue);

            //         Debug.Log(string.Format("Picking bigger value {0} and setting to cloud.", _biggerValue));
            //     }
            //     else if (_changedKey.Equals(kKeyForDoubleValue))
            //     {
            //         Debug.Log(string.Format("New value for key: {0} is {1}", _changedKey, NPBinding.CloudServices.GetDouble(_changedKey)));
            //     }

            // }
        }
    }

    private void CheckAllDataWhenInit()
    {
        this.ShowData_All();

        string cData_Heros = NPBinding.CloudServices.
            GetString(Constants.DataKey_Heros);
        string cData_Weapons = NPBinding.CloudServices.
            GetString(Constants.DataKey_Weapons);
        string cData_CurHero = NPBinding.CloudServices.
            GetString(Constants.DataKey_CurHeroID);
        string cData_Coin = NPBinding.CloudServices.
                GetString(Constants.DataKey_CoinNum);
        string lData_Heros = PlayerDataManager.instance.m_LocalDataMng.
            GetJsonData_Heros();
        string lData_Weapons = PlayerDataManager.instance.m_LocalDataMng.
            GetJsonData_Weapons();
        string lData_CurHero = PlayerDataManager.instance.m_LocalDataMng.
            GetData_CurSelectHeroID().ToString();
        string lData_Coin = PlayerDataManager.instance.m_LocalDataMng.
            GetData_CoinNum().ToString();

        if (cData_Heros != lData_Heros)
        {
            Debug.Log("WARRNING!!! Data_Heros : \n" +
                "local : " + lData_Heros + "\n" +
                "cloud : " + cData_Heros);
        }
        if (cData_Weapons != lData_Weapons)
        {
            Debug.Log("WARRNING!!! Data_Weapons : \n" +
                "local : " + lData_Weapons + "\n" +
                "cloud : " + cData_Weapons);
        }
        if (cData_CurHero != lData_CurHero)
        {
            Debug.Log("WARRNING!!! Data_CurHero : \n" +
                "local : " + lData_CurHero + "\n" +
                "cloud : " + cData_CurHero);
        }
        if (cData_Coin != lData_Coin)
        {
            Debug.Log("WARRNING!!! Data_Coin : \n" +
                "local : " + lData_Coin + "\n" +
                "cloud : " + cData_Coin);
        }
    }

    private bool TryGetCorrectData(string dataKey, out string corString)
    {
        switch (dataKey)
        {
            case Constants.DataKey_Heros:
                {
                    string localData = PlayerDataManager.instance.
                        m_LocalDataMng.GetJsonData_Heros();
                    string cloudData = NPBinding.CloudServices.
                        GetString(Constants.DataKey_Heros);
                    return PlayerDataManager.instance.CheckData(
                        dataKey, localData, cloudData, out corString);
                }
            case Constants.DataKey_Weapons:
                {
                    string localData = PlayerDataManager.instance.
                        m_LocalDataMng.GetJsonData_Weapons();
                    string cloudData = NPBinding.CloudServices.
                        GetString(Constants.DataKey_Weapons);
                    return PlayerDataManager.instance.CheckData(
                        dataKey, localData, cloudData, out corString);
                }
            case Constants.DataKey_CurHeroID:
                {
                    string localData = PlayerDataManager.instance.
                        m_LocalDataMng.GetData_CurSelectHeroID().ToString();
                    string cloudData = NPBinding.CloudServices.
                        GetString(Constants.DataKey_CurHeroID);
                    return PlayerDataManager.instance.CheckData(
                        dataKey, localData, cloudData, out corString);
                }
            case Constants.DataKey_CoinNum:
                {
                    string localData = PlayerDataManager.instance.
                        m_LocalDataMng.GetData_CoinNum().ToString();
                    string cloudData = NPBinding.CloudServices.
                        GetString(Constants.DataKey_CoinNum);
                    return PlayerDataManager.instance.CheckData(
                        dataKey, localData, cloudData, out corString);
                }
        }

        corString = "";
        return false;
    }

    public void SaveData_All()
    {
        this.SaveData_Heros();
        this.SaveData_Weapons();
        this.SaveData_CurHeroID();
        this.SaveData_CoinNum();
    }
    public void SaveData_Heros()
    {
        if (!m_UseCloudServices) return;

        string herosData = PlayerDataManager.instance.m_LocalDataMng.GetJsonData_Heros();
        NPBinding.CloudServices.SetString(Constants.DataKey_Heros, herosData);

        NPBinding.CloudServices.Synchronise();
    }
    public void SaveData_Weapons()
    {
        if (!m_UseCloudServices) return;

        string weaponsData = PlayerDataManager.instance.m_LocalDataMng.GetJsonData_Weapons();
        NPBinding.CloudServices.SetString(Constants.DataKey_Weapons, weaponsData);

        NPBinding.CloudServices.Synchronise();
    }
    public void SaveData_CurHeroID()
    {
        if (!m_UseCloudServices) return;

        int curHeroID = PlayerDataManager.instance.m_LocalDataMng.GetData_CurSelectHeroID();
        NPBinding.CloudServices.SetString(Constants.DataKey_CurHeroID, curHeroID.ToString());

        NPBinding.CloudServices.Synchronise();
    }
    public void SaveData_CoinNum()
    {
        if (!m_UseCloudServices) return;

        long coinNum = PlayerDataManager.instance.m_LocalDataMng.GetData_CoinNum();
        NPBinding.CloudServices.SetString(Constants.DataKey_CoinNum, coinNum.ToString());

        NPBinding.CloudServices.Synchronise();
    }

    public void ShowData_All()
    {
        // if (!m_Inited)
        // {
        //     Debug.Log("Has not inited!!!");
        //     return;
        // }
        string allData = "";
        allData += "<" + Constants.DataKey_Heros + "> : ";
        allData += NPBinding.CloudServices.GetString(Constants.DataKey_Heros) + "\n";
        allData += "<" + Constants.DataKey_Weapons + "> : ";
        allData += NPBinding.CloudServices.GetString(Constants.DataKey_Weapons) + "\n";
        allData += "<" + Constants.DataKey_CurHeroID + "> : ";
        allData += NPBinding.CloudServices.GetString(Constants.DataKey_CurHeroID) + "\n";
        allData += "<" + Constants.DataKey_CoinNum + "> : ";
        allData += NPBinding.CloudServices.GetString(Constants.DataKey_CoinNum) + "\n";
        Debug.Log("Cloud : \n" + allData);
    }
}
