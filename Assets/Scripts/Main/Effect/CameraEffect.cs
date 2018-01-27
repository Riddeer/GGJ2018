using UnityEngine;
using System.Collections;
// using Colorful;
using DG.Tweening;

public enum CamEftType
{
    NULL = -1,

    Shake = 0,
    LookAt = 1,
    Flash = 2,


    SIZE
};

public class CameraEffect : MonoBehaviour
{
    public static CameraEffect instance;
    public Camera[] m_Cameras;

    public Vector3 m_Pos2Hero = new Vector3(0f, 0f, -10f);
    public Vector3 m_FollowTar;
    public float m_CamFollowSpeed = 1;
    public Light m_SunLight;
    private CamEftType m_CurType = CamEftType.NULL;
    private IEnumerator m_IE_SmoothFollow;
    private IEnumerator m_IE_ChangeCameraSize;
    private float dis = 1;
    private float m_SunDefaultIntensity;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        m_Pos2Hero = new Vector3(0, 0, -10f * dis);
        m_CurType = CamEftType.NULL;
        // SetFollowTar(Global.instance.m_Player.transform.position);
        m_SunDefaultIntensity = m_SunLight.intensity;
        // auto loop flash
        StartCoroutine(IE_LoopFlash());

        // set follow target
        this.SetFollowTar(Global.instance.m_Hero_01.transform);
    }


    // shake 
    public void Shake(float shakeStrength = 0.1f, float rate = 14, float shakeTime = 0.3f)
    {
        if (m_CurType == CamEftType.LookAt) return;

        m_CurType = CamEftType.Shake;
        StartCoroutine(ShakeCamera(shakeStrength, rate, shakeTime));
    }

    // flash
    public void Flash()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_SunLight.DOIntensity(4f, 0.01f))
            .Append(m_SunLight.DOIntensity(m_SunDefaultIntensity, 0.1f))
            .Append(m_SunLight.DOIntensity(8f, 0.01f))
            .Append(m_SunLight.DOIntensity(m_SunDefaultIntensity, 1.2f));
    }

    private IEnumerator IE_LoopFlash()
    {
        float t = 0f;
        while (true)
        {
            t = Random.Range(6f, 12f);
            yield return new WaitForSeconds(t);
            this.Flash();
        }
    }

    public void DoLookAt_MoveBy(Vector3 deltPos)
    {
        m_CurType = CamEftType.LookAt;

    }

    /// <summary>
    /// 摄像机震动
    /// </summary>
    /// <param name="shakeStrength">震动幅度</param>
    /// <param name="rate">震动频率</param>
    /// <param name="shakeTime">震动时长</param>
    /// <returns></returns>
    IEnumerator ShakeCamera(float shakeStrength = 0.2f, float rate = 14, float shakeTime = 0.3f)
    {
        // StopCamSmoothFollow();
        float t = 0;
        float speed = 1 / shakeTime;
        foreach (Camera oneCamera in m_Cameras)
        {
            Vector3 m_DefaultPos = oneCamera.transform.position;
            while (t < 1)
            {
                t += Time.deltaTime * speed;

                oneCamera.transform.position = m_DefaultPos +
                    new Vector3(Mathf.Sin(rate * t),
                        Mathf.Cos(rate * t), 0) * Mathf.Lerp(shakeStrength, 0, t);
                yield return null;
            }

            oneCamera.transform.position = m_DefaultPos;
        }


        m_CurType = CamEftType.NULL;
    }

    IEnumerator SmoothFollow(Transform tar)
    {
        while (true)
        {

            yield return new WaitForEndOfFrame();

            if (tar == null)
            {
                this.StopCamSmoothFollow();
                continue;
            }

            dis -= Input.GetAxis("Mouse ScrollWheel");//鼠标中键控制摄像机距离的大小
            dis = Mathf.Clamp(dis, 0.25f, 1.29f);//摄像机距离主角的最大距离和最小距离
            m_Pos2Hero = new Vector3(0, 0, -10f * dis);


            foreach (Camera oneCamera in m_Cameras)
            {
                Vector3 defaultPos = oneCamera.transform.position;
                Vector3 targetPos = tar.position + m_Pos2Hero;
                oneCamera.transform.position = new Vector3(
                    Mathf.Lerp(defaultPos.x, targetPos.x,
                    Time.deltaTime * m_CamFollowSpeed),
                    Mathf.Lerp(defaultPos.y, targetPos.y,
                    Time.deltaTime * m_CamFollowSpeed),
                    Mathf.Lerp(defaultPos.z, targetPos.z,
                    Time.deltaTime * m_CamFollowSpeed));

            }



        }
    }
    public void StartCamSmoothFollow(Transform tar)
    {
        if (m_IE_SmoothFollow != null)
        {
            StopCoroutine(m_IE_SmoothFollow);
            m_IE_SmoothFollow = null;
        }
        m_IE_SmoothFollow = SmoothFollow(tar);
        StartCoroutine(m_IE_SmoothFollow);
    }
    public void StopCamSmoothFollow()
    {
        if (m_IE_SmoothFollow != null)
        {
            StopCoroutine(m_IE_SmoothFollow);
            m_IE_SmoothFollow = null;
        }

    }

    public void SetFollowTar(Transform tar)
    {
        // SpriteRenderer sp = m_MaskGO.GetComponent<SpriteRenderer>();
        // sp.DOFade(0f, 2f);

        // m_FollowTar = tar;
        transform.position = tar.position + m_Pos2Hero;
        StartCamSmoothFollow(tar);
    }

    IEnumerator ChangeCameraSize()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            foreach (Camera oneCamera in m_Cameras)
            {
                oneCamera.orthographicSize = Mathf.Lerp(
                    oneCamera.orthographicSize, 5f, Time.deltaTime * 8);
            }


        }
    }
    private void StartChangeCameraSize()
    {
        if (m_IE_ChangeCameraSize != null)
        {
            StopCoroutine(m_IE_ChangeCameraSize);
            m_IE_ChangeCameraSize = null;
        }
        m_IE_ChangeCameraSize = ChangeCameraSize();
        StartCoroutine(m_IE_ChangeCameraSize);
    }
    private void TurnLight()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_SunLight.DOIntensity(0f, 0.1f));
    }
    public void CameraEffect_GameOver()
    {
        StopAllCoroutines();
        // StopCoroutine(IE_LoopFlash());
        CameraEffect.instance.TurnLight();
        CameraEffect.instance.StartChangeCameraSize();
    }
}
