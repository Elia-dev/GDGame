using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Utils 
{
    public static string CheckNickname(string username)
    {
        if (username.Contains("-") || username.Contains(":") ||
            username.Contains(";") ||username.Contains("'") ||
            username.Contains("[") || username.Contains("]") || 
            username.Contains(",") || username.Contains("{") || 
            username.Contains("}") || username.Contains("_") || 
            username.Contains(" "))
        {
            return "Not allowed characters :;' -[],{}_";
        }
        
        if (username.Length > 16) {
            return "MAX characters number: 16";
        }
        
        return string.IsNullOrWhiteSpace(username) ? "Username can not be empty" : "OK";
    }

    public static Color32 ColorCode(string color, byte alpha) {
        switch (color) {
            case "red":
                return new Color32(255, 0, 0, alpha);
            case "green":
                return new Color32(0, 190, 0, alpha);
            case "blue":
                return new Color32(0, 0, 255, alpha);
            case "yellow":
                return new Color32(226, 230, 30, alpha);
            case "purple":
                return new Color32(124, 33, 239, alpha);
            case "black":
                return new Color32(0, 0, 0, alpha);      
            default:
                return new Color32(0, 0, 0, alpha);
        }
    }
    
    static int[,] LoadAdjMatrix(string filePath, int n)
    {
        int[,] adjMatrix = new int[n, n];
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    adjMatrix[i, j] = reader.ReadInt32();
                }
            }
        }
        return adjMatrix;
    }
    
    public static List<int> GetNeighborsNodeOf(int territoryNode)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "adj_matrix.bin");
    	int[,] adjMatrix = LoadAdjMatrix(filePath, 42);
        List<int> neighbors = new List<int>();
        int n = adjMatrix.GetLength(0);  // Get the number of rows (or columns) in the matrix

        for (int i = 0; i < n; i++)
        {
            if (adjMatrix[territoryNode, i] == 1)
            {
                neighbors.Add(i);
            }
        }

        return neighbors;
    }
    
    public static List<Territory> GetNeighborsOf(Territory territory)
    {
        List<Territory> territories = new List<Territory>();
        Debug.Log("Provo a leggere tutti i nodi vicini del terr " + territory.name + " che ha come nodo: " + territory.node);
        List<int> nodes = GetNeighborsNodeOf(territory.node);
        Debug.Log("Il territorio ha " + nodes.Count + " nodi vicini e sono i seguenti territori (stampo a capo): ");
        foreach (var node in nodes)
        {
            Debug.Log(GetTerritoryFromNode(node).name);
            territories.Add(GetTerritoryFromNode(node));
        }
        
        Debug.Log("Quindi ritorno in totale " + territories.Count + " territori");
        return territories;
    }
    
    public static Territory GetTerritoryFromNode(int node)
    {
        foreach (var terr in GameManager.Instance.AllTerritories)
        {
            if (terr.node == node)
            {
                Debug.Log("Territorio trovato: " + terr.name);
                return terr;
            }
        }
        Debug.Log("Territorio non trovato");
        return null;
        //return GameManager.Instance.AllTerritories.Find(x => x.node == node);
    }
}
