using Cinemachine;
using UnityEngine;

public class DollyMove : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float speed = 1f;

    CinemachineTrackedDolly dolly;

    void Start()
    {
        dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        dolly.m_PathPosition += speed * Time.deltaTime;
    }
}
