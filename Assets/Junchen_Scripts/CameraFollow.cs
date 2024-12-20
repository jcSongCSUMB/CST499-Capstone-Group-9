using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 主人公的Transform
    public Vector3 offset;   // 摄像机的偏移量
    public float smoothSpeed = 0.125f; // 平滑移动的速度

    void Start()
    {
        Camera.main.orthographicSize = 2f; // 固定大小
    }
    
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
