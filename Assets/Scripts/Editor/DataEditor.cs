using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using UnityEditor;
using UnityEngine;

public class DataEditor : EditorWindow
{

    [MenuItem("Tools" + "/" + "UpdateDataFromGDE", false, 100)]
    public static void UpdateDataFromGDE()
    {
        // Debug.Log("Start updating data ...");

        // Debug.Log("Init GDE data ...");
        GDEDataManager.Init("gde_data");

        // Debug.Log("Start updating data of Hero ...");
        List<GDEHeroData> allHeroData = GDEDataManager.GetAllItems<GDEHeroData>();
        foreach (GDEHeroData oneData in allHeroData)
        {
            // get prefab
            string fullPath = "Assets/Resources/" + oneData.PrefabPath + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath(
                fullPath,
                typeof(GameObject)) as GameObject;
            GameObject newPrefab = PrefabUtility.InstantiatePrefab(prefab)
                as GameObject;

            Debug.Assert(prefab != null, "!!! Check Path <" + fullPath + "> !!!");

            Hero hero = newPrefab.GetComponent<Hero>();
            hero.m_MaxHP = oneData.MaxHP;
            hero.m_MoveVec = oneData.MoveVec;
            hero.m_HurtProtectTime = oneData.HurtProtect;
            hero.m_MAXShieldVal = oneData.MaxShieldVal;
            hero.m_ShieldRestoreVec = oneData.ShieldRestoreVec;
            hero.m_ShieldRestoreBreak = oneData.ShieldRestoreBreak;
            hero.m_ShieldRestoreInterval = oneData.ShieldRestoreInterval;

            Rigidbody2D rig = newPrefab.GetComponent<Rigidbody2D>();
            rig.mass = oneData.Mass;

            PrefabUtility.ReplacePrefab(newPrefab, prefab,
                ReplacePrefabOptions.Default);
            MonoBehaviour.DestroyImmediate(newPrefab);

        }
        // Debug.Log("Finish updating data of Hero !!!");

        // Debug.Log("Start updating data of Enemy ...");
        List<GDEEnemyData> allEnemyData = GDEDataManager.GetAllItems<GDEEnemyData>();
        foreach (GDEEnemyData oneData in allEnemyData)
        {
            // get prefab
            string fullPath = "Assets/Resources/" + oneData.PrefabPath + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath(
                fullPath,
                typeof(GameObject)) as GameObject;
            GameObject newPrefab = PrefabUtility.InstantiatePrefab(prefab)
                as GameObject;

            Debug.Assert(prefab != null, "!!! Check Path <" + fullPath + "> !!!");

            Enemy enemy = newPrefab.GetComponent<Enemy>();
            enemy.m_MaxHP = oneData.MaxHP;
            enemy.m_MoveVec = oneData.MoveVec;
            enemy.m_AlertRange = oneData.AlertRange;
            enemy.m_AtkRange = oneData.AtkRange;
            enemy.m_AtkInterval = oneData.AtkInterval;
            enemy.m_HurtProtectTime = oneData.HurtProtect;

            Rigidbody2D rig = newPrefab.GetComponent<Rigidbody2D>();
            rig.mass = oneData.Mass;


            PrefabUtility.ReplacePrefab(newPrefab, prefab,
                ReplacePrefabOptions.Default);
            MonoBehaviour.DestroyImmediate(newPrefab);

        }
        // Debug.Log("Finish updating data of Enemy !!!");

        // Debug.Log("Start updating data of Weapon ...");
        List<GDEWeaponData> allWeaponData = GDEDataManager.GetAllItems<GDEWeaponData>();
        foreach (GDEWeaponData oneData in allWeaponData)
        {
            // get prefab
            string fullPath = "Assets/Resources/" + oneData.PrefabPath + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath(
                fullPath,
                typeof(GameObject)) as GameObject;
            GameObject newPrefab = PrefabUtility.InstantiatePrefab(prefab)
                as GameObject;

            Debug.Assert(prefab != null, "!!! Check Path <" + fullPath + "> !!!");

            Weapon weapon = newPrefab.GetComponent<Weapon>();
            weapon.m_WeaponID = (WeaponID)oneData.ID;
            weapon.m_MaxBullet = oneData.MagazineSize;
            weapon.m_ConsumePerHit = oneData.ConsumePerHit;
            weapon.m_AtkInterval = oneData.AtkInterval;
            weapon.m_ChargeTime = oneData.ChargeTime;
            weapon.m_ReloadTime = oneData.ReloadTime;
            WeaponOperateType opaType = (WeaponOperateType)oneData.OpaType.ID;
            weapon.m_OpaType = opaType;
            weapon.m_SpineSkinName = oneData.SkinName;
            weapon.m_AniType = (WeaponAniType)oneData.AniType.ID;
            weapon.m_AlertRange = oneData.AlertRange;

            string dmgName = weapon.m_Dmg.name;
            fullPath = "Assets/Resources/Attack/" + dmgName + ".prefab";
            GameObject dmgPrefab = AssetDatabase.LoadAssetAtPath(
                fullPath,
                typeof(GameObject)) as GameObject;
            GameObject newDmgPrefab = PrefabUtility.InstantiatePrefab(dmgPrefab)
                as GameObject;
            Debug.Assert(dmgPrefab != null, "!!! Check Path <" + fullPath + "> !!!");

            Damage dmg = newDmgPrefab.GetComponent<Damage>();
            dmg.m_DamageVal = oneData.DamageVal;
            dmg.m_Thrust = oneData.Thrust;
            PrefabUtility.ReplacePrefab(newDmgPrefab, dmgPrefab,
                ReplacePrefabOptions.Default);
            MonoBehaviour.DestroyImmediate(newDmgPrefab);


            PrefabUtility.ReplacePrefab(newPrefab, prefab,
                ReplacePrefabOptions.Default);
            MonoBehaviour.DestroyImmediate(newPrefab);

        }
        // Debug.Log("Finish updating data of Weapon !!!");


        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("成功", "UpdateDataFromGDE 完成！", "确定");
    }

}
