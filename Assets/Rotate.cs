using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float secondsPerRevolution = 2f; // x秒转一圈

    void Update()
    {
        float anglePerSecond = 360f / secondsPerRevolution;
        transform.Rotate(0f, anglePerSecond * Time.deltaTime, 0f);
    }
}
