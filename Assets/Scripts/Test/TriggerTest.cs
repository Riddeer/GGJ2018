using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Enter  " +col.name);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log("Stay  " +col.name);
       
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("Exit  " +col.name);
    }

}
