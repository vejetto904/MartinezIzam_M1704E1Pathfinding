using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject token1, token2, token3, token4;
    private int[,] GameMatrix; //0 not chosen, 1 player, 2 enemy
    private int[] startPos = new int[2];
    private int[] objectivePos = new int[2];
    private bool EvaluateWin = false;
    private bool finalizado = false;

    List<AStar> listaAbierta = new List<AStar>();
    List<AStar> listaCerrada = new List<AStar>();

    AStar inicio;
    AStar objetivo;

    private int posicionLista = 0;

    private void Awake()
    {
        GameMatrix = new int[Calculator.length, Calculator.length];

        for (int i = 0; i < Calculator.length; i++) //fila
            for (int j = 0; j < Calculator.length; j++) //columna
                GameMatrix[i, j] = 0;

        //randomitzar pos final i inicial;
        var rand1 = Random.Range(0, Calculator.length);
        var rand2 = Random.Range(0, Calculator.length);
        startPos[0] = rand1;
        startPos[1] = rand2;
        Debug.Log("Start y: " + startPos[0] + " Start x: " + startPos[1]);
        SetObjectivePoint(startPos);

        inicio = new AStar(startPos, objetivo.nodePosition);

        listaAbierta.Add(inicio);
        listaCerrada.Add(inicio);

        GameMatrix[startPos[0], startPos[1]] = 1;
        GameMatrix[objectivePos[0], objectivePos[1]] = 2;

        InstantiateToken(token1, startPos);
        InstantiateToken(token2, objectivePos);
        ShowMatrix();
    }
    private void InstantiateToken(GameObject token, int[] position)
    {
        Instantiate(token, Calculator.GetPositionFromMatrix(position),
            Quaternion.identity);
    }
    private void SetObjectivePoint(int[] startPos) 
    {
        var rand1 = Random.Range(0, Calculator.length);
        var rand2 = Random.Range(0, Calculator.length);
        if (rand1 != startPos[0] || rand2 != startPos[1])
        {
            objectivePos[0] = rand1;
            objectivePos[1] = rand2;
            objetivo = new AStar(objectivePos);

            Debug.Log("Objective y: " + objectivePos[0] + " Objective x: " + objectivePos[1]);
        }
    }

    private void ShowMatrix() //fa un debug log de la matriu
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
    //EL VOSTRE EXERCICI COMENÇA AQUI
    private void Update()
    {
        if(!EvaluateWin)
        {
            //cmprobacion del nodo Izquierdo
            if (inicio.nodePosition[1]-1 >= 0)
            {
                AStar nodoIzquierdo = new AStar(getArrey(inicio.nodePosition[0], inicio.nodePosition[1]-1), objetivo.nodePosition);
                listaAbierta.Add(nodoIzquierdo);
                InstantiateToken(token3, nodoIzquierdo.nodePosition);
            }
            if (inicio.nodePosition[1] + 1 < Calculator.length)
            {
                AStar nodoDerecho = new AStar(getArrey(inicio.nodePosition[0], inicio.nodePosition[1] + 1), objetivo.nodePosition);
                listaAbierta.Add(nodoDerecho);
                InstantiateToken(token3, nodoDerecho.nodePosition);
            }
            if (inicio.nodePosition[0] - 1 >= 0)
            {
                AStar nodoTecho = new AStar(getArrey(inicio.nodePosition[0]-1, inicio.nodePosition[1]), objetivo.nodePosition);
                listaAbierta.Add(nodoTecho);
                InstantiateToken(token3, nodoTecho.nodePosition);
            }
            if (inicio.nodePosition[0] + 1 < Calculator.length)
            {
                AStar nodoSuelo = new AStar(getArrey(inicio.nodePosition[0]+1, inicio.nodePosition[1]), objetivo.nodePosition);
                listaAbierta.Add(nodoSuelo);
                InstantiateToken(token3, nodoSuelo.nodePosition);
            }

            inicio = añadirAListaCerrada(listaAbierta[posicionLista], listaAbierta[posicionLista + 1], listaAbierta[posicionLista + 2], listaAbierta[posicionLista + 3]);
            listaCerrada.Add(inicio);
            posicionLista = posicionLista + 4;
        }
        if (inicio.nodePosition[0] == objetivo.nodePosition[0] && inicio.nodePosition[1] == objetivo.nodePosition[1])
        {
            EvaluateWin = true;
            if (!finalizado)
            {
                List<AStar> listaDefinitiva = new List<AStar>();
                AStar nodoActual = listaCerrada[0];
                listaDefinitiva.Add(inicio);
                for (int i = 1; i < listaCerrada.Count; i++)
                {
                    if(nodoActual.heuristica > listaCerrada[i].heuristica)
                    {
                        listaDefinitiva.Add(listaCerrada[i]);
                        objetivo = listaCerrada[i];
                    }
                }
                foreach (AStar node in listaDefinitiva)
                {
                    InstantiateToken(token4, node.nodePosition);
                }
                finalizado = true;
            }
        }
    }
    AStar añadirAListaCerrada(AStar nodoIzquierdo, AStar nodoDerecho, AStar nodoTecho, AStar nodoSuelo)
    {
        AStar mejorNodo = nodoTecho;
        if(nodoSuelo.heuristica < mejorNodo.heuristica) mejorNodo = nodoSuelo;
        if(nodoDerecho.heuristica < mejorNodo.heuristica) mejorNodo = nodoDerecho;
        if(nodoIzquierdo.heuristica < mejorNodo.heuristica) mejorNodo = nodoIzquierdo;
        return mejorNodo;
    }

    int[] getArrey(int a, int b)
    {
        int[] array = { a, b };
        return array;
    }
}
