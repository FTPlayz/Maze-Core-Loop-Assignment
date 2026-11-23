using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject finishPrefab;

    Room[,] rooms;

    [SerializeField] int numX = 10;
    [SerializeField] int numY = 10;

    float roomWidth;
    float roomHeight;

    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    public bool MazeReady { get; private set; } = false;

    private void GetRoomSize()
    {
        SpriteRenderer[] renderers =
            roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach (SpriteRenderer ren in renderers)
        {
            minBounds = Vector3.Min(minBounds, ren.bounds.min);
            maxBounds = Vector3.Max(maxBounds, ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }

    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(
            numX * (roomWidth - 1) / 2,
            numY * (roomHeight - 1) / 2,
            -100f
        );

        float minValue = Mathf.Min(
            numX * (roomWidth - 1),
            numY * (roomHeight - 1)
        );

        Camera.main.orthographicSize = minValue * 0.75f;
    }

    private void Start()
    {
        GetRoomSize();

        rooms = new Room[numX, numY];

        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                GameObject room = Instantiate(
                    roomPrefab,
                    new Vector3(i * roomWidth, j * roomHeight, 0),
                    Quaternion.identity
                );

                room.name = "Room_" + i + "_" + j;

                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        if (finishPrefab != null)
        {
            Vector3 finishPosition = new Vector3(
                (numX - 1) * roomWidth,
                (numY - 1) * roomHeight,
                0
            );

            Instantiate(finishPrefab, finishPosition, Quaternion.identity);
        }

        SetCamera();
    }

    private void RemoveRoomWall(
        int x, int y, Room.Directions dir)
    {
        if (dir != Room.Directions.NONE)
            rooms[x, y].SetDirFlag(dir, false);

        Room.Directions opposite = Room.Directions.NONE;

        switch (dir)
        {
            case Room.Directions.TOP:
                if (y < numY - 1)
                {
                    opposite = Room.Directions.BOTTOM;
                    y++;
                }
                break;

            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    opposite = Room.Directions.LEFT;
                    x++;
                }
                break;

            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    opposite = Room.Directions.TOP;
                    y--;
                }
                break;

            case Room.Directions.LEFT:
                if (x > 0)
                {
                    opposite = Room.Directions.RIGHT;
                    x--;
                }
                break;
        }

        if (opposite != Room.Directions.NONE)
            rooms[x, y].SetDirFlag(opposite, false);
    }

    public List<Tuple<Room.Directions, Room>>
        GetNeighboursNotVisited(int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours =
            new List<Tuple<Room.Directions, Room>>();

        foreach (Room.Directions dir in
            Enum.GetValues(typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    y++;
                    break;

                case Room.Directions.RIGHT:
                    x++;
                    break;

                case Room.Directions.BOTTOM:
                    y--;
                    break;

                case Room.Directions.LEFT:
                    x--;
                    break;

                default:
                    continue;
            }

            if (x >= 0 && x < numX &&
                y >= 0 && y < numY &&
                !rooms[x, y].visited)
            {
                neighbours.Add(
                    new Tuple<Room.Directions, Room>(
                        dir, rooms[x, y]
                    )
                );
            }
        }

        return neighbours;
    }

    private bool GenerateStep()
    {
        if (stack.Count == 0)
            return true;

        Room current = stack.Peek();

        var neighbours = GetNeighboursNotVisited(
            current.Index.x,
            current.Index.y
        );

        if (neighbours.Count > 0)
        {
            int index = UnityEngine.Random.Range(
                0, neighbours.Count
            );

            var item = neighbours[index];

            item.Item2.visited = true;

            RemoveRoomWall(
                current.Index.x,
                current.Index.y,
                item.Item1
            );

            stack.Push(item.Item2);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    // CREATE MAZE
    public void CreateMaze()
    {
        if (generating) return;

        Reset();
        MazeReady = false;

        // Keep all four outside boundaries closed.
        // No outside walls are removed here.

        rooms[0, 0].visited = true;
        stack.Push(rooms[0, 0]);

        StartCoroutine(Coroutine_Generate());
    }

    IEnumerator Coroutine_Generate()
    {
        generating = true;

        bool finished = false;

        while (!finished)
        {
            finished = GenerateStep();
            yield return new WaitForSeconds(0.05f);
        }

        generating = false;
        MazeReady = true;

        Debug.Log("Maze generation complete!");
    }

    private void Reset()
    {
        stack.Clear();

        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);

                rooms[i, j].visited = false;
            }
        }
    }

    public Vector2Int GetRoomIndex(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / roomWidth);
        int y = Mathf.RoundToInt(position.y / roomHeight);

        return new Vector2Int(x, y);
    }

    private bool IsInsideMaze(Vector2Int index)
    {
        return index.x >= 0 && index.x < numX &&
               index.y >= 0 && index.y < numY;
    }

    public List<Vector3> FindPath(
        Vector3 startPosition,
        Vector3 targetPosition)
    {
        List<Vector3> path = new List<Vector3>();

        if (!MazeReady)
            return path;

        Vector2Int start = GetRoomIndex(startPosition);
        Vector2Int target = GetRoomIndex(targetPosition);

        Debug.Log("Enemy Room: " + start);
        Debug.Log("Player Room: " + target);

        if (!IsInsideMaze(start) || !IsInsideMaze(target))
        {
            Debug.LogWarning("Enemy or player outside maze!");
            return path;
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> previous =
            new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        previous[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == target)
                break;

            foreach (Room.Directions dir in new Room.Directions[]
            {
                Room.Directions.TOP,
                Room.Directions.RIGHT,
                Room.Directions.BOTTOM,
                Room.Directions.LEFT
            })
            {
                if (rooms[current.x, current.y].HasWall(dir))
                    continue;

                Vector2Int next = current;

                switch (dir)
                {
                    case Room.Directions.TOP:
                        next.y++;
                        break;

                    case Room.Directions.RIGHT:
                        next.x++;
                        break;

                    case Room.Directions.BOTTOM:
                        next.y--;
                        break;

                    case Room.Directions.LEFT:
                        next.x--;
                        break;
                }

                if (!IsInsideMaze(next) ||
                    previous.ContainsKey(next))
                    continue;

                previous[next] = current;
                queue.Enqueue(next);
            }
        }

        if (!previous.ContainsKey(target))
        {
            Debug.LogWarning("No path found!");
            return path;
        }

        Vector2Int step = target;

        while (step != start)
        {
            path.Add(
                rooms[step.x, step.y].transform.position
            );

            step = previous[step];
        }

        path.Reverse();

        Debug.Log("Path found! Total waypoints: " + path.Count);

        return path;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !generating)
            CreateMaze();
    }
}