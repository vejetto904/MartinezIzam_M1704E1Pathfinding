using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject token1, token2, token3, token4;
    private int[,] GameMatrix; //0 not chosen, 1 player, 2 enemy
    private int[] startPos = new int[2];
    private int[] objectivePos = new int[2];
    private bool EvaluateWin = false;

    private PriorityQueue<AStarNode> openSet;
    private HashSet<AStarNode> closedSet;

    private void Awake()
    {
        GameMatrix = new int[Calculator.length, Calculator.length];

        for (int i = 0; i < Calculator.length; i++) //fila
            for (int j = 0; j < Calculator.length; j++) //columna
                GameMatrix[i, j] = 0;

        var rand1 = UnityEngine.Random.Range(0, Calculator.length);
        var rand2 = UnityEngine.Random.Range(0, Calculator.length);
        startPos[0] = rand1;
        startPos[1] = rand2;
        Debug.Log("Start y: " + startPos[0] + " Start x: " + startPos[1]);
        SetObjectivePoint(startPos);

        AStarNode startNode = new AStarNode(startPos, 0, CalculateDistance(startPos, objectivePos));
        openSet = new PriorityQueue<AStarNode>();
        openSet.Enqueue(startNode, startNode.fCost);

        closedSet = new HashSet<AStarNode>();

        GameMatrix[startPos[0], startPos[1]] = 1;
        GameMatrix[objectivePos[0], objectivePos[1]] = 2;

        InstantiateToken(token1, startPos);
        InstantiateToken(token2, objectivePos);
        ShowMatrix();
    }

    private void InstantiateToken(GameObject token, int[] position)
    {
        Instantiate(token, Calculator.GetPositionFromMatrix(position), Quaternion.identity);
    }

    private void SetObjectivePoint(int[] startPos)
    {
        var rand1 = UnityEngine.Random.Range(0, Calculator.length);
        var rand2 = UnityEngine.Random.Range(0, Calculator.length);
        if (rand1 != startPos[0] || rand2 != startPos[1])
        {
            objectivePos[0] = rand1;
            objectivePos[1] = rand2;
            Debug.Log("Objective y: " + objectivePos[0] + " Objective x: " + objectivePos[1]);
        }
    }

    private void ShowMatrix()
    {
        string matrix = "";
        for (int i = 0; i < Calculator.length; i++)
        {
            for (int j = 0; j < Calculator.length; j++)
            {
                matrix += GameMatrix[i, j] + " ";
            }
            matrix += "\n";
        }
        Debug.Log(matrix);
    }

    private void Update()
    {
        if (!EvaluateWin)
        {
            if (openSet.Count > 0)
            {
                AStarNode currentNode = openSet.Dequeue();

                if (currentNode.position[0] == objectivePos[0] && currentNode.position[1] == objectivePos[1])
                {
                    // Path found
                    EvaluateWin = true;
                    GeneratePath(currentNode);
                    return;
                }

                closedSet.Add(currentNode);

                // Check neighbors
                ExploreNeighbors(currentNode);
            }
            else
            {
                // No path found
                Debug.Log("No Path Found");
                EvaluateWin = true;
            }
        }
    }

    private void ExploreNeighbors(AStarNode currentNode)
    {
        int[][] directions = {
            new int[] { 0, 1 }, // Right
            new int[] { 0, -1 }, // Left
            new int[] { 1, 0 }, // Down
            new int[] { -1, 0 } // Up
        };

        foreach (int[] dir in directions)
        {
            int[] neighborPos = { currentNode.position[0] + dir[0], currentNode.position[1] + dir[1] };

            if (IsValidPosition(neighborPos) && !IsInClosedSet(neighborPos))
            {
                int gCost = currentNode.gCost + 1; // Movement cost always 1
                int hCost = CalculateDistance(neighborPos, objectivePos);
                AStarNode neighbor = new AStarNode(neighborPos, gCost, hCost, currentNode);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Enqueue(neighbor, neighbor.fCost);
                    InstantiateToken(token3, neighborPos);
                }
            }
        }
    }

    private bool IsValidPosition(int[] pos)
    {
        return pos[0] >= 0 && pos[0] < Calculator.length && pos[1] >= 0 && pos[1] < Calculator.length;
    }

    private bool IsInClosedSet(int[] pos)
    {
        foreach (AStarNode node in closedSet)
        {
            if (node.position[0] == pos[0] && node.position[1] == pos[1])
            {
                return true;
            }
        }
        return false;
    }

    private void GeneratePath(AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        foreach (AStarNode node in path)
        {
            InstantiateToken(token4, node.position);
        }
    }

    private int CalculateDistance(int[] startPos, int[] targetPos)
    {
        return Mathf.Abs(startPos[0] - targetPos[0]) + Mathf.Abs(startPos[1] - targetPos[1]);
    }
}
