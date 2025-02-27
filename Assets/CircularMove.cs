using UnityEngine;

public class CircularMover : MonoBehaviour
{
    // 圆心位置
    Vector3 center = Vector3.zero;
    // 圆的半径
    public float radius = 15f;

    public float timeEachRound = 20;
    // 角速度（弧度/秒），例如 Mathf.PI*2 意味着每秒转一圈
    public float angularSpeed => Mathf.PI * 2f / timeEachRound;
    // 内部角度（以弧度计）
    private float angle = 0f;

    void Start()
    {
        // 获取圆心位置
        center = transform.position;
    }
    void Update()
    {
        // 更新角度，确保是循环的
        angle += angularSpeed * Time.deltaTime;
        // 可选：保持角度在 0 到 2π 之间
        angle %= Mathf.PI * 2f;
        
        // 计算新的位置，假设在 XZ 平面上运动
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        // 保持 y 不变
        transform.position = new Vector3(x, transform.position.y, z);
    }
}