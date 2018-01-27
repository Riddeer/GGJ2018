using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticMapTest : MonoBehaviour
{

    public float m_MapSize_X;
    public float m_MapSize_Y;
    public float m_ParticSize_X;
    public float m_ParticSize_Y;
    public GameObject m_ParticPrefabs;
    public Transform m_Parent;
    private List<GameObject> m_MapItems = new List<GameObject>();
    private Vector3 m_Anchor = new Vector3(-18, -10, 0);
    void Start()
    {
        CreateParticMap(m_MapSize_X, m_MapSize_Y, m_ParticSize_X, m_ParticSize_Y, m_Anchor, m_Parent);
        ParticWakeUp(Global.instance.m_Hero_01.transform.position.x, Global.instance.m_Hero_01.transform.position.y,
        m_MapItems, m_ParticSize_X, m_ParticSize_Y);     	
    }


    void Update()
    {
        // ParticWakeUp(Global.instance.m_Player.transform.position.x, Global.instance.m_Player.transform.position.y,
        // m_MapItems, 18f, 10f);
		SetParticActive(Global.instance.m_Hero_01.transform.position.x, Global.instance.m_Hero_01.transform.position.y);
    }
    private int ReturnNumOfShouldCreate(float length, float size)
    {
        int result = (int)length / (int)size;

        if ((int)length % (int)size == 0)
            return result;
        else
            return (result + 1);
    }

    private void CreateParticMap(float mapX, float mapY, float sizeX, float sizeY, Vector3 anchor, Transform parent)
    {
        int x = ReturnNumOfShouldCreate(mapX, sizeX);
        int y = ReturnNumOfShouldCreate(mapY, sizeY);
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                pos.x = anchor.x + i * sizeX;
                pos.y = anchor.y + j * sizeY;

                GameObject go = Instantiate(m_ParticPrefabs, pos, Quaternion.identity, parent);
                m_MapItems.Add(go);
                go.SetActive(false);
            }
        }
    }

    private void ParticWakeUp(float roleX, float roleY, List<GameObject> list, float sizeX, float sizeY)
    {
        foreach (var one in list)
        {
            if (Mathf.Abs(one.transform.position.x - roleX) < sizeX && Mathf.Abs(one.transform.position.y - roleY) < sizeY)
            {
				if(!one.activeSelf)
                	one.SetActive(true);
            }
            else
            {
				if(one.activeSelf)
                	one.SetActive(false);
            }
        }
    }
    public void SetParticActive(float x, float y)
    {
		ParticWakeUp(x,y,m_MapItems,m_ParticSize_X,m_ParticSize_Y);
    }
}
