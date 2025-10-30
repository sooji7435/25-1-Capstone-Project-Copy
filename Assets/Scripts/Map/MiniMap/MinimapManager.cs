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
    private Dictionary<Vector2Int, Color> roomColors = new Dictionary<Vector2Int, Color>();
    public void HighlightRoom(Vector2Int roomPos)
    {
        if (previousRoom.HasValue &&
            minimapIcons.TryGetValue(previousRoom.Value, out GameObject prevIcon))
        {
            // 원래 색 복원
            if (roomColors.TryGetValue(previousRoom.Value, out Color originalColor))
                prevIcon.GetComponent<UnityEngine.UI.Image>().color = originalColor;
        }

        if (minimapIcons.TryGetValue(roomPos, out GameObject currentIcon))
        {
            currentIcon.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            previousRoom = roomPos;
        }
    }

    public void SetRoomColor(Vector2Int roomPos, Color color)
    {
        if (minimapIcons.TryGetValue(roomPos, out GameObject icon))
        {
            icon.GetComponent<UnityEngine.UI.Image>().color = color;
            roomColors[roomPos] = color;
        }
    }



}