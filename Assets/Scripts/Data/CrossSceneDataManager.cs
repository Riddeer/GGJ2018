using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneDataManager : MonoBehaviour
{
    public static CrossSceneDataManager instance { get; private set; }

    public NetworkMode m_NetworkMode = NetworkMode.NULL;

    void Awake()
    {
        if (CrossSceneDataManager.instance == null)
        {
            CrossSceneDataManager.instance = this;
        }
        else if (CrossSceneDataManager.instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

}
