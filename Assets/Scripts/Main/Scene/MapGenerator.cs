using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public int m_SlipVal_1 = 2;
    public int m_SlipVal_2 = 3;
    public int m_MaxResNumInS2 = 2;

    private List<int> m_MapList = new List<int>();

    // Use this for initialization
    void Start()
    {
        this.CreateList();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            this.CreateList();
        }
    }

    private void CreateList()
    {
        m_MapList.Clear();
        int[] mapArr = new int[m_SlipVal_1 * m_SlipVal_1 * m_SlipVal_2 * m_SlipVal_2];
        m_MapList = new List<int>(mapArr);

        /* s1
		
		2 3
		0 1

		*/

        /* s2
		
		6 7 8
		3 4 5
		0 1 2

		*/

        for (int k = 0; k < m_SlipVal_1 * m_SlipVal_1; k++)
        {
            int kx = k % m_SlipVal_1;
            int ky = Mathf.FloorToInt((float)k / (float)m_SlipVal_1);

            int resNum = m_MaxResNumInS2;// Random.Range(1, m_MaxResNumInS2 + 1)
            for (int i = 0; i < resNum; i++)
            {
                int randomIdx;
                int x;
                int y;
                int mapX;
                int mapY;
                int mapIdx;

                do
                {
                    randomIdx = Random.Range(0, m_SlipVal_2 * m_SlipVal_2);
                    x = randomIdx % m_SlipVal_2;
                    y = Mathf.FloorToInt((float)randomIdx / (float)m_SlipVal_2);
                    mapX = x + kx * m_SlipVal_2;
                    mapY = y + ky * m_SlipVal_2;
                    mapIdx = mapY * m_SlipVal_1 * m_SlipVal_2 + mapX;
                }
                while (!this.CheckResIdx(m_MapList, mapIdx));
                m_MapList[mapIdx] = 1;
            }
        }

        this.PrintMapList();
    }

    private void PrintMapList()
    {
        if (m_MapList.Count <= 0) return;

        string result = "";
        int idx = 0;
        int line = m_SlipVal_1 * m_SlipVal_2;
        int column = m_SlipVal_1 * m_SlipVal_2;
        for (int y = line - 1; y >= 0; y--)
        {
            for (int x = 0; x < column; x++)
            {
                idx = x + y * column;
                result += " " + m_MapList[idx];
            }
            result += "\n";
        }

		Debug.Log("Map : \n" + result);
    }

    private bool CheckResIdx(List<int> arr, int checkIdx)
    {
        if (checkIdx >= arr.Count) return false;

        if (arr[checkIdx] == 1) return false;

		int x = checkIdx % (m_SlipVal_1 * m_SlipVal_2);
		int y = Mathf.FloorToInt((float)checkIdx / 
			(float)(m_SlipVal_1 * m_SlipVal_2));
		
		// up
		if (y + 1 < m_SlipVal_1 * m_SlipVal_2 &&
		arr[x + (y + 1) * m_SlipVal_1 * m_SlipVal_2] == 1)
			 return false;
		// down
		if (y - 1 >= 0 &&
		arr[x + (y - 1) * m_SlipVal_1 * m_SlipVal_2] == 1)
			 return false;
		// right
		if (x + 1 < m_SlipVal_1 * m_SlipVal_2 &&
		arr[x + 1 + y * m_SlipVal_1 * m_SlipVal_2] == 1)
			 return false;
		// left
		if (x - 1 >= 0 &&
		arr[x - 1 + y * m_SlipVal_1 * m_SlipVal_2] == 1)
			 return false;

        return true;
    }
}
