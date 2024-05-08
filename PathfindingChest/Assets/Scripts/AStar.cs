using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AStar 
{
    public int[] nodePosition;
    public float heuristica;

    public AStar(int[] nodePosition, int[] nodoObjetivo = null)
    {
        this.nodePosition = nodePosition;
        if(nodoObjetivo != null)
        {
            heuristica = Vector3.Distance(new Vector3(nodePosition[0], nodePosition[1], 0), new Vector3(nodoObjetivo[0], nodoObjetivo[1], 0));
        }
        else { heuristica = 0; }
    }
}
