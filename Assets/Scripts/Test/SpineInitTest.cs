using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
using Spine.Unity;
public class SpineInitTest : MonoBehaviour
{

    GDEManager m_DataManager;
    PlayerDataManager m_LM;
    public GameObject Character_1;
    public GameObject Character_2;
    public GameObject Character_Content_1;
    public GameObject Character_Content_2;
    private SkeletonAnimation m_SK;
    bool temp = true;
    void Start()
    {
        m_DataManager = GDEManager.instance;
        m_LM = PlayerDataManager.instance;
        InitCharacter();
    }
    void Update()
    {
        // if(temp)
        // {
        // 	InitCharacter();
        // 	temp = false;
        // }
        SetAttachment();
    }
    // Update is called once per framecharacter
    private void InitCharacter()
    {
        var heros = m_LM.GetData_HeroList();
        for (int i = 0; i < heros.Count; i++)
        {
            var weapons = m_LM.GetData_HeroWeaponList(heros[i]);
            for (int j = 0; j < weapons.Count; j++)
            {
                var weaponID = weapons[0];
                var weaponData = m_DataManager.GetData_Weapon(weaponID);
                var weaponName = weaponData.SkinName;

                // 以后需要代码重构，动态地生成UI的prefabs
                if (i == 0)
                {
                    var ske_01 = Character_1.transform.GetChild(0).GetComponent<SkeletonGraphic>();
					var ske_contant_01 = Character_Content_1.GetComponent<SkeletonGraphic>();
					

                    // if (weaponData.AniType.ID == 0)
                    // {
                    //     ske_01.Skeleton.SetAttachment("Arm_R", "Arm_R");
                    //     ske_01.Skeleton.SetAttachment("Arm_L", "Arm_L");
                    //     ske_01.Skeleton.SetAttachment("Hand_Hold", null);

                    //     ske_01.Skeleton.SetAttachment("Gun", weaponName);
                    //     ske_01.Skeleton.SetAttachment("Gun_01", null);

					// 	ske_contant_01.Skeleton.SetAttachment("Arm_R", "Arm_R");
                    //     ske_contant_01.Skeleton.SetAttachment("Arm_L", "Arm_L");
               
                    //     ske_contant_01.Skeleton.SetAttachment("Hand_Hold", null);

                    //     ske_contant_01.Skeleton.SetAttachment("Gun", weaponName);
                    //     ske_contant_01.Skeleton.SetAttachment("Gun_01", null);

                    // }
                    // else if (weaponData.AniType.ID == 1)
                    // {
                    //     ske_01.Skeleton.SetAttachment("Arm_R", null);
                    //     ske_01.Skeleton.SetAttachment("Arm_L", null);
                    //     ske_01.Skeleton.SetAttachment("Hand_Hold", "BothHand");

                    //     ske_01.Skeleton.SetAttachment("Gun", null);
                    //     ske_01.Skeleton.SetAttachment("Gun_01", weaponName);

					// 	ske_contant_01.Skeleton.SetAttachment("Arm_R", null);
                    //     ske_contant_01.Skeleton.SetAttachment("Arm_L", null);
   
                    //     ske_contant_01.Skeleton.SetAttachment("Hand_Hold", "BothHand");

                    //     ske_contant_01.Skeleton.SetAttachment("Gun", null);
                    //     ske_contant_01.Skeleton.SetAttachment("Gun_01", weaponName);

						
                    // }



                }
				// 由于没有第二个英雄，所以这里省略了
                else if (i == 1)
                {

                }
            }
        }
    }
    private void SetAttachment()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ske = Character_1.transform.GetChild(0).GetComponent<SkeletonGraphic>();

            ske.Skeleton.SetAttachment("Arm_R", "Arm_R");
            ske.Skeleton.SetAttachment("Arm_L", "Arm_L");
          
  
			ske.Skeleton.SetAttachment("Hand_Hold", null);
            ske.Skeleton.SetAttachment("Gun", "Bezier");
            ske.Skeleton.SetAttachment("Gun_01", null);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            var ske = Character_1.transform.GetChild(0).GetComponent<SkeletonGraphic>();

            ske.Skeleton.SetAttachment("Arm_R", null);
            ske.Skeleton.SetAttachment("Arm_L", null);
         

            ske.Skeleton.SetAttachment("Hand_Hold", "BothHand");
            ske.Skeleton.SetAttachment("Gun", null);
            ske.Skeleton.SetAttachment("Gun_01", "Boomerang");
        }

    }
    private void RunAnimation()
    {
        var state = m_SK.state.SetAnimation(0,",",true);
        
    }
}
