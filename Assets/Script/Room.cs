using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Directions
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        NONE
    }

    [SerializeField] GameObject topWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;

    private Dictionary<Directions, GameObject> walls =
        new Dictionary<Directions, GameObject>();

    public Vector2Int Index { get; set; }

    public bool visited { get; set; } = false;

    private void Awake()
    {
        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;

        foreach (GameObject wall in walls.Values)
        {
            AddWallCollider(wall);
        }
    }

    private void AddWallCollider(GameObject wall)
    {
        if (wall == null)
        {
            Debug.LogError("Missing wall reference in " + name);
            return;
        }

        BoxCollider2D collider =
            wall.GetComponent<BoxCollider2D>();

        if (collider == null)
            collider = wall.AddComponent<BoxCollider2D>();

        collider.enabled = true;
        collider.isTrigger = false;

        SpriteRenderer sr =
            wall.GetComponent<SpriteRenderer>();

        if (sr == null || sr.sprite == null)
            return;

        Bounds spriteBounds = sr.sprite.bounds;

        collider.size = spriteBounds.size;
        collider.offset = spriteBounds.center;

        Vector3 scale = wall.transform.lossyScale;

        Vector3 worldSize = sr.bounds.size;

        float overlap = 0.4f;

        if (worldSize.x >= worldSize.y)
        {
            collider.size = new Vector2(
                collider.size.x +
                overlap / Mathf.Max(0.001f, Mathf.Abs(scale.x)),
                collider.size.y
            );
        }
        else
        {
            collider.size = new Vector2(
                collider.size.x,
                collider.size.y +
                overlap / Mathf.Max(0.001f, Mathf.Abs(scale.y))
            );
        }
    }

    public bool HasWall(Directions dir)
    {
        return walls.ContainsKey(dir) &&
               walls[dir] != null &&
               walls[dir].activeSelf;
    }

    private void SetActive(Directions dir, bool flag)
    {
        if (walls.ContainsKey(dir) && walls[dir] != null)
        {
            walls[dir].SetActive(flag);
        }
    }

    public void SetDirFlag(Directions dir, bool flag)
    {
        if (dir == Directions.NONE)
            return;

        SetActive(dir, flag);
    }
}

