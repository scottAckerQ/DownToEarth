using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private void LateUpdate()
    {
        if (ShouldFollow())
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }
    }

    private bool ShouldFollow()
    {
        //TODO: determine bounds of the scene and only move if still on screen.
        return true;
    }
}
