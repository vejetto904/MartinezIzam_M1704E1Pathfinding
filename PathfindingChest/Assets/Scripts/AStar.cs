using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AStarNode
{
    public int[] position;
    public int gCost;
    public int hCost;
    public AStarNode parent;

    public int fCost => gCost + hCost;

    public AStarNode(int[] _position, int _gCost, int _hCost, AStarNode _parent = null)
    {
        position = _position;
        gCost = _gCost;
        hCost = _hCost;
        parent = _parent;
    }
}
