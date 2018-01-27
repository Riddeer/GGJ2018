using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;

public class GDEManager : MonoBehaviour
{
    public static GDEManager instance { get; private set; }

    void Awake()
    {
        if (GDEManager.instance == null)
        {
            GDEManager.instance = this;
        }
        else if (GDEManager.instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        this.InitData();
    }

    private void InitData()
    {
        GDEDataManager.Init("gde_data");
    }

    // get data
    public GDEHeroData GetData_Hero(int id)
    {
        List<GDEHeroData> allData = GDEDataManager.GetAllItems<GDEHeroData>();
        foreach (GDEHeroData oneData in allData)
        {
            if (oneData.ID == id)
            {
                return oneData;
            }
        }

        return null;
    }
    public GDEEnemyData GetData_Enemy(int id)
    {
        List<GDEEnemyData> allData = GDEDataManager.GetAllItems<GDEEnemyData>();
        foreach (GDEEnemyData oneData in allData)
        {
            if (oneData.ID == id)
            {
                return oneData;
            }
        }

        return null;
    }
    public GDEWeaponData GetData_Weapon(int id)
    {
        List<GDEWeaponData> allData = GDEDataManager.GetAllItems<GDEWeaponData>();
        foreach (GDEWeaponData oneData in allData)
        {
            if (oneData.ID == id)
            {
                return oneData;
            }
        }

        return null;
    }
}
