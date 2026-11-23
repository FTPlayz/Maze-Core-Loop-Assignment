using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float waypointDistance = 0.2f;

    private Rigidbody2D rb;
    private GenerateMaze maze;

    private List<Vector3> path = new List<Vector3>();
    private int pathIndex = 0;

    private Vector2Int lastPlayerRoom =
        new Vector2Int(-999, -999);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maze = FindObjectOfType<GenerateMaze>();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
        }

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        Debug.Log("Player found: " + (player != null));
        Debug.Log("Maze found: " + (maze != null));
    }

    private void FixedUpdate()
    {
        if (rb == null || player == null || maze == null)
            return;

        if (!maze.MazeReady)
            return;

        Vector2Int playerRoom =
            maze.GetRoomIndex(player.position);

        // Recalculate when player enters another room.
        if (playerRoom != lastPlayerRoom)
        {
            path = maze.FindPath(
                rb.position,
                player.position
            );

            pathIndex = 0;
            lastPlayerRoom = playerRoom;

            Debug.Log("New path: " + path.Count);
        }

        Vector2Int enemyRoom =
            maze.GetRoomIndex(rb.position);

        // Chase player directly inside the same room.
        if (enemyRoom == playerRoom)
        {
            Vector2 next = Vector2.MoveTowards(
                rb.position,
                player.position,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(next);
            return;
        }

        // Skip waypoints already reached.
        while (pathIndex < path.Count &&
               Vector2.Distance(
                   rb.position,
                   path[pathIndex]
               ) < waypointDistance)
        {
            pathIndex++;
        }

        // Follow the next waypoint.
        if (pathIndex < path.Count)
        {
            Vector2 target = path[pathIndex];

            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                target,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);
        }
    }
}







