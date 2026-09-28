using UnityEngine;

public class TriggerActiveRoom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || (collision.CompareTag("Vehicle") && collision.GetComponent<Vehicle>().GetDriver()?.gameObject.name == "Player"))
        {
            gameObject.transform.parent.GetComponent<RoomManager>().ActiveRoom();
            gameObject.SetActive(false);
        }
    }
}
