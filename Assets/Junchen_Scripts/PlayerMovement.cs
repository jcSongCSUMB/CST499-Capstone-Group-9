using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 移动速度
    public Tilemap tilemap; // 引用Tilemap
    private Vector3 targetPosition; // 目标位置
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position; // 初始位置为玩家当前所在位置

        // 检查Tilemap是否已赋值
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标点击的世界坐标
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // 确保Z轴为0

            // 使用Physics2D.Raycast检测点击位置
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"Hit: {hit.collider.name}");
                Vector3Int gridPosition = tilemap.WorldToCell(hit.point);

                // 检查目标Tile是否有效
                if (tilemap.HasTile(gridPosition))
                {
                    targetPosition = tilemap.GetCellCenterWorld(gridPosition);
                    isMoving = true;

                    // 打印调试信息
                    Debug.Log($"Mouse World Pos: {mouseWorldPos}, Grid Pos: {gridPosition}, Target Pos: {targetPosition}");
                }
                else
                {
                    Debug.Log($"Invalid Tile: Mouse World Pos: {mouseWorldPos}, Grid Pos: {gridPosition}");
                }
            }
            else
            {
                Debug.Log("No collider hit at the clicked position.");
            }
        }

        // 平滑移动到目标位置
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 当玩家到达目标位置时停止移动
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // 打印到达目标位置的调试信息
                Debug.Log($"Player reached Target Pos: {targetPosition}");
            }
        }
    }
}
