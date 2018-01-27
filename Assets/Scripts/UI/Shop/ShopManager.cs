using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine.Unity;

public enum NetworkMode
{
    NULL = -1,
    PVP_Solo = 0,
    PVP_Double = 1,
    PVE_Solo = 2,
    PVE_Double = 3,


    SIZE

};

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject leftButton;
    public GameObject rightBution;
    public GameObject Character_1;
    public GameObject Character_2;
    public GameObject CharacterContent_1;
    public GameObject CharacterContent_2;
    public GameObject CharacterMenu;
    public List<Text> WeaponHoles = new List<Text>();
    public Toggle m_POEToggle;
    public Text m_PlayerNumText;
    public Text m_EnemyNumText;

    private PlayerDataManager m_PlayerDataMng;
    private int heroIndex = -1;
    private int weaponIndex = 0;
    private List<GameObject> UI_Characters = new List<GameObject>();
    void Awake()
    {
        // if (instance == null)
        //     instance = this;
        // else if (instance != this)
        // {
        //     Destroy(gameObject);
        // }
    }
    void Start()
    {
        m_PlayerDataMng = PlayerDataManager.instance;
        m_PlayerDataMng.ShowData_All();
        heroIndex = GetCurHeroIndex();
        CharacterMenu.SetActive(true);
        InitCharacterTextrue();
        CharacterMenu.SetActive(false);
        SelectCharacter();

        // this.SetPoESwitchCallback();
    }

    public void StartGame()
    {
        m_PlayerDataMng.SaveData_All();
        SceneManager.LoadScene("Scenes/Main");
        AudioManager.instance.Play(Constants.BGM_Noise_Rain_01);
        AudioManager.instance.Play(Constants.BGM_Noise_Thunder_01);
    }

    // set character UI panel active by index
    private void SetCharacterUIValue(int index)
    {
        if (index == -1) return;
        for (int i = 0; i < UI_Characters.Count; i++)
        {
            if (i == index) UI_Characters[i].SetActive(true);
            else
            {
                UI_Characters[i].SetActive(false);
            }
        }
        var length = m_PlayerDataMng.GetData_LengthOfHeroList();
        if (index == 0)
        {
            leftButton.SetActive(false);
            Character_1.SetActive(true);
            Character_2.SetActive(false);
            CharacterContent_1.SetActive(true);
            CharacterContent_2.SetActive(false);
        }
        if (index == length - 1)
        {
            rightBution.SetActive(false);
            Character_2.SetActive(true);
            Character_1.SetActive(false);
            CharacterContent_2.SetActive(true);
            CharacterContent_1.SetActive(false);
        }
    }
    // get select character
    private int GetCurHeroIndex()
    {
        int result = 0;
        result = m_PlayerDataMng.GetData_CurSelectHeroID();
        return result;
    }
    public void IndexUp()
    {
        heroIndex++;
        SelectCharacter();


    }
    public void IndexDown()
    {
        heroIndex--;
        SelectCharacter();

    }
    public void SetWeaponIndex(int index)
    {
        weaponIndex = index;
    }
    // set select weapon value
    public void SetWeaponSetting(int index)
    {
        m_PlayerDataMng.SetData_Weapon(this.heroIndex, weaponIndex, index);
        Init_HeroWeaponData();
        InitCharacterTextrue();
    }
    // set select character value
    private void SelectCharacter()
    {
        if (heroIndex == -1)
        {
            return;
        }

        m_PlayerDataMng.SetData_SelectHero(heroIndex);
        SetCharacterUIValue(heroIndex);
        Init_HeroWeaponData();
    }
    public void Init_HeroWeaponData()
    {
        string weaponName = "default";
        for (int i = 0; i < m_PlayerDataMng.GetData_HeroWeaponList(heroIndex).Count; i++)
        {
            weaponName = GDEManager.instance.GetData_Weapon(m_PlayerDataMng.GetData_WeaponID(i, heroIndex)).SkinName;
            WeaponHoles[i].text = weaponName;
        }
    }
    private void InitCharacterTextrue()
    {
        var heros = m_PlayerDataMng.GetData_HeroList();

        for (int i = 0; i < heros.Count; i++)
        {
            var weapons = m_PlayerDataMng.GetData_HeroWeaponList(heros[i]);
            var weaponID = weapons[weaponIndex];
            var weaponData = GDEManager.instance.GetData_Weapon(weaponID);
            var weaponName = weaponData.SkinName;

            // Debug.Log("InitCharacterTextrue : weaponID = " + weaponID + "\n" +
            //     "weaponName = " + weaponName);

            // 以后需要代码重构，动态地生成UI的prefabs
            // first hero
            if (i == 0)
            {
                var ske_01 = Character_1.transform.GetChild(0)
                .GetComponent<SkeletonGraphic>();
                var ske_contant_01 = CharacterContent_1.GetComponent<SkeletonGraphic>();
                if (weaponData.AniType.ID == 0)
                {
                    ske_01.Skeleton.SetAttachment("Arm_R", "Arm_R");
                    ske_01.Skeleton.SetAttachment("Arm_L", "Arm_L");
                    ske_01.Skeleton.SetAttachment("Hand_Hold", null);
                    ske_01.Skeleton.SetAttachment("Gun", weaponName);
                    ske_01.Skeleton.SetAttachment("Gun_01", null);
                    ske_contant_01.Skeleton.SetAttachment("Arm_R", "Arm_R");
                    ske_contant_01.Skeleton.SetAttachment("Arm_L", "Arm_L");
                    ske_contant_01.Skeleton.SetAttachment("Hand_Hold", null);
                    ske_contant_01.Skeleton.SetAttachment("Gun", weaponName);
                    ske_contant_01.Skeleton.SetAttachment("Gun_01", null);
                }
                else if (weaponData.AniType.ID == 1)
                {
                    ske_01.Skeleton.SetAttachment("Arm_R", null);
                    ske_01.Skeleton.SetAttachment("Arm_L", null);
                    ske_01.Skeleton.SetAttachment("Hand_Hold", "BothHand");
                    ske_01.Skeleton.SetAttachment("Gun", null);
                    ske_01.Skeleton.SetAttachment("Gun_01", weaponName);
                    ske_contant_01.Skeleton.SetAttachment("Arm_R", null);
                    ske_contant_01.Skeleton.SetAttachment("Arm_L", null);
                    ske_contant_01.Skeleton.SetAttachment("Hand_Hold", "BothHand");
                    ske_contant_01.Skeleton.SetAttachment("Gun", null);
                    ske_contant_01.Skeleton.SetAttachment("Gun_01", weaponName);
                }
            }
            // secound hero
            else if (i == 1)
            {
                var ske_01 = Character_2.transform.GetChild(0).GetComponent<SkeletonGraphic>();
                var ske_contant_01 = CharacterContent_2.GetComponent<SkeletonGraphic>();


                if (weaponData.AniType.ID == 0)
                {
                    ske_01.Skeleton.SetAttachment("Hand_hold", null);
                    ske_01.Skeleton.SetAttachment("Armor3", null);
                    ske_01.Skeleton.SetAttachment("Arm_R3", null);
                    ske_01.Skeleton.SetAttachment("Gun_01", null);


                    ske_01.Skeleton.SetAttachment("Gun", weaponName);
                    ske_01.Skeleton.SetAttachment("Armor", "Armor 2");
                    ske_01.Skeleton.SetAttachment("Hand_R", "Hand_R");
                    ske_01.Skeleton.SetAttachment("Arm_R", "Arm_R");
                    ske_01.Skeleton.SetAttachment("Hand_L", "Hand_L");
                    ske_01.Skeleton.SetAttachment("Arm_L", "Arm_L");


                    // UI

                    ske_contant_01.Skeleton.SetAttachment("Hand_hold", null);
                    ske_contant_01.Skeleton.SetAttachment("Armor3", null);
                    ske_contant_01.Skeleton.SetAttachment("Arm_R3", null);
                    ske_contant_01.Skeleton.SetAttachment("Gun_01", null);


                    ske_contant_01.Skeleton.SetAttachment("Gun", weaponName);
                    ske_contant_01.Skeleton.SetAttachment("Armor", "Armor 2");
                    ske_contant_01.Skeleton.SetAttachment("Hand_R", "Hand_R");
                    ske_contant_01.Skeleton.SetAttachment("Arm_R", "Arm_R");
                    ske_contant_01.Skeleton.SetAttachment("Hand_L", "Hand_L");
                    ske_contant_01.Skeleton.SetAttachment("Arm_L", "Arm_L");
                }
                else if (weaponData.AniType.ID == 1)
                {
                    ske_01.Skeleton.SetAttachment("Hand_hold", "Hand_hold");
                    ske_01.Skeleton.SetAttachment("Armor3", "Armor");
                    ske_01.Skeleton.SetAttachment("Arm_R3", "Arm_R");
                    ske_01.Skeleton.SetAttachment("Gun_01", weaponName);


                    ske_01.Skeleton.SetAttachment("Gun", null);
                    ske_01.Skeleton.SetAttachment("Armor", null);
                    ske_01.Skeleton.SetAttachment("Hand_R", null);
                    ske_01.Skeleton.SetAttachment("Arm_R", null);
                    ske_01.Skeleton.SetAttachment("Hand_L", null);
                    ske_01.Skeleton.SetAttachment("Arm_L", null);


                    // UI

                    ske_contant_01.Skeleton.SetAttachment("Hand_hold", "Hand_hold");
                    ske_contant_01.Skeleton.SetAttachment("Armor3", "Armor");
                    ske_contant_01.Skeleton.SetAttachment("Arm_R3", "Arm_R");
                    ske_contant_01.Skeleton.SetAttachment("Gun_01", weaponName);


                    ske_contant_01.Skeleton.SetAttachment("Gun", null);
                    ske_contant_01.Skeleton.SetAttachment("Armor", null);
                    ske_contant_01.Skeleton.SetAttachment("Hand_R", null);
                    ske_contant_01.Skeleton.SetAttachment("Arm_R", null);
                    ske_contant_01.Skeleton.SetAttachment("Hand_L", null);
                    ske_contant_01.Skeleton.SetAttachment("Arm_L", null);



                }

            }
        }
    }


    public void ToggleCallback_NetworkMode(Toggle tog)
    {
        if (!tog.isOn) return;

        switch (tog.gameObject.name)
        {
            case "PVP_Solo":
                {
                    CrossSceneDataManager.instance.m_NetworkMode = 
                        NetworkMode.PVP_Solo;
                }
                break;

            case "PVP_Double":
                {
                    CrossSceneDataManager.instance.m_NetworkMode = 
                        NetworkMode.PVP_Double;
                }
                break;

            case "PVE_Solo":
                {
                    CrossSceneDataManager.instance.m_NetworkMode = 
                        NetworkMode.PVE_Solo;
                }
                break;

            case "PVE_Double":
                {
                    CrossSceneDataManager.instance.m_NetworkMode = 
                        NetworkMode.PVE_Double;
                }
                break;
        }
        
    }
}
