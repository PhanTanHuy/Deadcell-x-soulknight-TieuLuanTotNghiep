using UnityEngine;

public class TriggerActiveRoom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.transform.parent.GetComponent<RoomManager>().ActiveRoom();
            gameObject.SetActive(false);
        }
    }
}
