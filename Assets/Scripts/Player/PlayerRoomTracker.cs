using UnityEngine;

public class PlayerRoomTracker : MonoBehaviour
{
    public float roomSize = 10f; // 실제 게임 맵에서 방 하나 크기 (예: 10 유닛)
    private Vector2Int currentRoomPos = Vector2Int.zero;

    void Start()
    {
        // 게임 시작 시 시작방 표시
        MinimapManager.Instance.RevealRoom(currentRoomPos);
        MinimapManager.Instance.HighlightRoom(currentRoomPos);
    }

    void Update()
    {
        Vector2 playerPos = transform.position;

        // 플레이어 위치를 방 단위로 변환
        Vector2Int roomPos = new Vector2Int(
            Mathf.RoundToInt(playerPos.x / roomSize),
            Mathf.RoundToInt(playerPos.y / roomSize)
        );

        // 방이 바뀌었을 때만 갱신
        if (roomPos != currentRoomPos)
        {
            currentRoomPos = roomPos;

            MinimapManager.Instance.RevealRoom(currentRoomPos);
            MinimapManager.Instance.HighlightRoom(currentRoomPos);
        }
    }
}
