using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatConsole : MonoBehaviour
{

    public InputField m_Field;

    private bool m_Shown = false;

    // Use this for initialization
    void Start()
    {
        m_Shown = false;

        //Adds a listener that invokes the "LockInput" method when the player finishes editing the main input field.
        //Passes the main input field into the method when "LockInput" is invoked
        m_Field.onEndEdit.AddListener(delegate { LockInput(m_Field); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            m_Shown = !m_Shown;
        }

        // show or not
        m_Field.gameObject.SetActive(m_Shown);

        if (m_Shown)
        {

        }
        else
        {

        }
    }

    // Checks if there is anything entered into the input field.
    void LockInput(InputField input)
    {
        if (input.text.Length > 0)
        {
            // Debug.Log("Text has been entered : \n" + input.text);

            if (CheckInput(input.text))
            {
                m_Field.text = "";
            }
        }
        else if (input.text.Length == 0)
        {
            // Debug.Log("Main Input Empty");
        }
    }

    private bool CheckInput(string inputS)
    {
        string[] splitS = inputS.Split(' ');
        if (splitS.Length <= 1) return false;
        switch (splitS[0])
        {
            case Constants.CheatKey_CameraFollowType:
            case Constants.CheatKey_CameraFollowType_Short:
                {
                    // int intVal = -1;
                    // int.TryParse(splitS[1], out intVal);
                    // if (intVal <= (int)CamFollowType.NULL ||
                    //     intVal >= (int)CamFollowType.SIZE)
                    //     return false;
                    // CamFollowType typeVal = (CamFollowType)intVal;
                    // Camera.main.GetComponent<CameraControl>().m_CurFollowType = typeVal;
                    // return true;
                }
                break;

            default:
                break;
        }

        return false;
    }

}
