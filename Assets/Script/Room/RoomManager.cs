using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    [SerializeField] private GameObject[] doors;
    [SerializeField] private MousePool mousePool;
    [SerializeField] private GameObject roomMinimap;
    private int mouseNeedToClear;
    private int mouseCleared;
    private void Awake()
    {
        roomMinimap.SetActive(false);
        mouseNeedToClear = mousePool.TotalMouse();
        DiactiveDoors();
        mousePool.gameObject.SetActive(false);
    }
    public void DiactiveDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(false);
        }
    }
    public void ActiveRoom()
    {
        Instance = this;
        roomMinimap.SetActive(true);
        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }
        mousePool.gameObject.SetActive(true);
    }

    public void ClearRoom()
    {
        DiactiveDoors();
        mousePool.gameObject.SetActive(false);
        Instance = null;
    }
    public void ClearMouse()
    {
        mouseCleared++;
        if (mouseCleared >= mouseNeedToClear)
        {
            ClearRoom();
        }
    }
}
