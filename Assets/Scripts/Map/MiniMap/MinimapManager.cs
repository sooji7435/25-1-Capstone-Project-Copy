using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : Singleton<MinimapManager>
{

    [SerializeField] Transform minimapContainer; // minimap용 UI 부모 (예: Canvas 아래)
    [SerializeField] GameObject minimapIconPrefab; // minimap에 표시할 작은 아이콘
    [SerializeField] float roomGap;

    private Dictionary<Vector2Int, GameObject> minimapIcons = new Dictionary<Vector2Int, GameObject>();
    public void InitMiniMap()
    {
        
        foreach (var icon in minimapIcons.Values)
            Destroy(icon);
        minimapIcons.Clear();
    }
    public void RegisterRoom(Vector2Int roomPos)
    {
        GameObject icon = Instantiate(minimapIconPrefab, minimapContainer);
        icon.transform.localPosition = new Vector3(roomPos.x * roomGap, roomPos.y * roomGap, 0); // 20f = minimap 격자 간격
        icon.SetActive(false);
        minimapIcons.Add(roomPos, icon);
    }

    public void RevealRoom(Vector2Int roomPos)
    {
        if (minimapIcons.TryGetValue(roomPos, out GameObject icon))
        {
            icon.SetActive(true);
        }
    }

    private Vector2Int? previousRoom = null;

    public void HighlightRoom(Vector2Int pos)
    {
        // 이전 방 색 복원
        if (previousRoom.HasValue)
        {
            var prev = previousRoom.Value;
            if (MapManager.Instance.roomMap.ContainsKey(prev))
            {
                var prevRoom = MapManager.Instance.roomMap[prev].GetComponent<Room>();
                if (prevRoom.isVisited)
                    SetRoomColor(prev, Color.gray); // 방문한 방
                else
                    SetRoomColor(prev, new Color(0.3f, 0.3f, 0.3f, 0.8f)); // 미방문 방
            }
        }

        // 현재 방을 흰색으로 강조
        SetRoomColor(pos, Color.white);
        previousRoom = pos;
    }

    //[SerializeField] GameObject playerIcon;
    //public void UpdatePlayerPosition(Vector2Int roomPos)
    //{
    //    playerIcon.transform.localPosition = new Vector3(roomPos.x * roomGap, roomPos.y * roomGap, 0);
    //}

    public void SetRoomColor(Vector2Int roomPos, Color color)
    {
        if (minimapIcons.TryGetValue(roomPos, out GameObject icon))
        {
            icon.GetComponent<UnityEngine.UI.Image>().color = color;
        }
    }

    //현재 방 주변(상하좌우)의 방들을 미니맵에 표시
    //방문하지 않은 방은 어두운 회색으로 표시
    public void UpdateVisibleRooms(Vector2Int currentRoomPos)
    {
        // 현재 방: 흰색으로 표시
        SetRoomColor(currentRoomPos, Color.white);

        // 4방향 탐색 (상하좌우)
        Vector2Int[] directions = new Vector2Int[]
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = currentRoomPos + dir;

            // 실제로 존재하는 방만 표시
            if (MapManager.Instance.roomMap.ContainsKey(neighborPos))
            {
                Room neighborRoom = MapManager.Instance.roomMap[neighborPos].GetComponent<Room>();

                // 아직 방문하지 않은 방은 어두운 회색으로 표시
                if (!neighborRoom.isVisited)
                {
                    SetRoomColor(neighborPos, new Color(0.3f, 0.3f, 0.3f, 0.8f)); // 반투명 어두운 회색
                }
            }
        }
    }

}