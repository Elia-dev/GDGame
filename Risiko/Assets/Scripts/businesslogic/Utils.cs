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
    
    static List<int> GetNeighborsOf(int territoryNode)
    {
    	int[,] adjMatrix = LoadAdjMatrix("adj_matrix.bin", 42);
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
}
