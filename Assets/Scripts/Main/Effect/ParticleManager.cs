using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public static ParticleManager instance { get; private set; }

    private List<ParticleSystem> m_Particles =
        new List<ParticleSystem>();
    public float m_MapSize_X;
    public float m_MapSize_Y;
    public float m_ParticSize_X;
    public float m_ParticSize_Y;
    public GameObject prefab_Rain;
    public GameObject prefab_Fly;
    private Transform m_Parent;
    private List<GameObject> RainsOnGround = new List<GameObject>();
    private List<GameObject> FlysOnGround = new List<GameObject>();
    private Vector3 rainAnchor = new Vector3(-18, -10, 0);
    private Vector3 flyAnchor = Vector3.zero;
    private float cameraSizeX;
    private float cameraSizeY;



    protected void OnEnable()
    {
        if (ParticleManager.instance == null)
        {
            ParticleManager.instance = this;
        }
    }
    void Start()
    {
        m_Parent = gameObject.GetComponent<Transform>();
        CalcCameraSize();
        flyAnchor = CalcParticleAnchor(cameraSizeX,cameraSizeY);
        // CreateParticsOnMap( m_ParticSize_X, m_ParticSize_Y, 
        // rainAnchor, prefab_Rain,RainsOnGround);
        // CreateParticsOnMap(cameraSizeX,cameraSizeY,flyAnchor,prefab_Fly,FlysOnGround);
        // SetParticActive(Camera.main.transform.position.x, Camera.main.transform.position.y,RainsOnGround,0.75f);
        // SetParticActive(Camera.main.transform.position.x,Camera.main.transform.position.y,FlysOnGround,1f);
        
    }
    void Update()
    {
        foreach (ParticleSystem one in m_Particles)
        {
            if (one == null || !one.IsAlive())
            {
                m_Particles.Remove(one);
                Destroy(one.gameObject);
                break;
            }
        }
        SetParticActive(Camera.main.transform.position.x, Camera.main.transform.position.y,RainsOnGround,0.75f);
        SetParticActive(Camera.main.transform.position.x,Camera.main.transform.position.y,FlysOnGround,1f);
    }

    public GameObject CreateParticle(GameObject par, Vector3 pos, Vector3 ang)
    {
        GameObject parGO = Instantiate(par, transform);
        parGO.transform.position = pos;
        parGO.transform.localEulerAngles = ang;

        ParticleSystem createdPar = parGO.GetComponent<ParticleSystem>();

        m_Particles.Add(createdPar);

        return parGO;
    }
    private int ReturnNumOfShouldCreate(float length, float size)
    {
        int result = (int)length / (int)size;

        if ((int)length % (int)size == 0)
            return result;
        else
            return (result + 1);
    }

    private void CreateParticsOnMap(float sizeX, float sizeY, Vector3 anchor, GameObject prefab,List<GameObject> particlelist)
    {
        int x = ReturnNumOfShouldCreate(m_MapSize_X, sizeX);
        int y = ReturnNumOfShouldCreate(m_MapSize_Y, sizeY);
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                pos.x = anchor.x + i * sizeX;
                pos.y = anchor.y + j * sizeY;

                GameObject go = Instantiate(prefab, pos, Quaternion.identity, m_Parent);
                // go.transform.rotation = Quaternion.Euler(new Vector3(-90f,0,0));
                particlelist.Add(go);
                go.SetActive(false);
            }
        }
    }

    private void ParticWakeUp(float cameraX, float cameraY, List<GameObject> list, float sizeX, float sizeY)
    {
        // Debug.Log(string.Format("{0}--{1}--{2}--{3}",cameraX,cameraY,sizeX,sizeY));
        foreach (var one in list)
        {
            if (Mathf.Abs(one.transform.position.x - cameraX) < sizeX && Mathf.Abs(one.transform.position.y - cameraY) < sizeY)
            {
                if (!one.activeSelf)
                    one.SetActive(true);
            }
            else
            {
                if (one.activeSelf)
                    one.SetActive(false);
            }
        }
    }
    public void SetParticActive(float x, float y,List<GameObject> particleList,float offsize)
    {
        ParticWakeUp(x, y, particleList, cameraSizeX*offsize ,cameraSizeY*offsize);
    }
    private void CalcCameraSize()
    {
        // Game Windows Size = 16/9 , such as 1920/1080

        cameraSizeY = Camera.main.orthographicSize *2;
        cameraSizeX = cameraSizeY / 9f * 16;

        // Debug.Log(string.Format("{0}---{1}", cameraSizeX, cameraSizeY));
    }
    private Vector3 CalcParticleAnchor(float particleX,float particleY)
    {
        float anchorX = - m_MapSize_X/2 + (particleX /2);
        float anchorY = - m_MapSize_Y/2 + (particleY /2);
        return new Vector3(anchorX,anchorY,0);
    }
}
