using System;
using System.IO;

class AdjMatrixBuilder
{
    static void Build()
    {
        int n = 42;

        // Initialize a n x n adjacency matrix with all zeros
        int[,] adjMatrix = new int[n, n];

        // Define edges (list of tuples in Python, array of arrays in C#)
        int[][] edges = new int[][]
        {
            new int[] {0, 1}, new int[] {0, 3}, new int[] {0, 30},                                  // Alaska neighbors
            new int[] {1, 0}, new int[] {1, 3}, new int[] {1, 4}, new int[] {1, 2},                 // Northwest neighbors
            new int[] {2, 1}, new int[] {2, 4}, new int[] {2, 5}, new int[] {2, 13},                // Greenland neighbors
            new int[] {3, 0}, new int[] {3, 1}, new int[] {3, 4}, new int[] {3, 6},                 // Alberta neighbors
            new int[] {4, 1}, new int[] {4, 3}, new int[] {4, 6}, new int[] {4, 7}, new int[] {4, 5}, new int[] {4, 2}, // Ontario neighbors
            new int[] {5, 4}, new int[] {5, 2}, new int[] {5, 7},                                   // Quebec neighbors
            new int[] {6, 3}, new int[] {6, 4}, new int[] {6, 7}, new int[] {6, 8},                 // Western U.S. neighbors
            new int[] {7, 5}, new int[] {7, 4}, new int[] {7, 6}, new int[] {7, 8},                 // Eastern U.S. neighbors
            new int[] {8, 6}, new int[] {8, 7}, new int[] {8, 9},                                   // Central America neighbors
            new int[] {9, 8}, new int[] {9, 10}, new int[] {9, 11},                                 // Venezuela neighbors
            new int[] {10, 9}, new int[] {10, 11}, new int[] {10, 12},                              // Peru neighbors
            new int[] {11, 9}, new int[] {11, 10}, new int[] {11, 12},                              // Brazil neighbors
            new int[] {12, 10}, new int[] {12, 11},                                                 // Argentina neighbors
            new int[] {13, 2}, new int[] {13, 15}, new int[] {13, 14},                              // Iceland neighbors
            new int[] {14, 13}, new int[] {14, 15}, new int[] {14, 19},                             // Scandinavia neighbors
            new int[] {15, 13}, new int[] {15, 14}, new int[] {15, 16}, new int[] {15, 17},         // Great Britain neighbors
            new int[] {16, 15}, new int[] {16, 19}, new int[] {16, 17}, new int[] {16, 18},         // Northern Europe neighbors
            new int[] {17, 15}, new int[] {17, 16}, new int[] {17, 18},                             // Western Europe neighbors
            new int[] {18, 17}, new int[] {18, 16}, new int[] {18, 19}, new int[] {18, 34}, new int[] {18, 21}, new int[] {18, 20}, // Southern Europe neighbors
            new int[] {19, 14}, new int[] {19, 16}, new int[] {19, 18}, new int[] {19, 34}, new int[] {19, 33}, new int[] {19, 26}, // Ukraine neighbors
            new int[] {20, 18}, new int[] {20, 11}, new int[] {20, 21}, new int[] {20, 23}, new int[] {20, 22}, // North Africa neighbors
            new int[] {21, 18}, new int[] {21, 20}, new int[] {21, 23}, new int[] {21, 34},         // Egypt neighbors
            new int[] {22, 20}, new int[] {22, 23}, new int[] {22, 24},                             // Congo neighbors
            new int[] {23, 34}, new int[] {23, 21}, new int[] {23, 20}, new int[] {23, 22}, new int[] {23, 24}, new int[] {23, 25}, // Eastern Africa neighbors
            new int[] {24, 22}, new int[] {24, 23}, new int[] {24, 25},                             // Southern Africa neighbors
            new int[] {25, 23}, new int[] {25, 24},                                                 // Madagascar neighbors
            new int[] {26, 19}, new int[] {26, 33}, new int[] {26, 36}, new int[] {26, 27},         // Ural neighbors
            new int[] {27, 26}, new int[] {27, 36}, new int[] {27, 32}, new int[] {27, 29}, new int[] {27, 28}, // Siberia neighbors
            new int[] {28, 27}, new int[] {28, 29}, new int[] {28, 30},                             // Yakutsk neighbors
            new int[] {29, 27}, new int[] {29, 32}, new int[] {29, 30}, new int[] {29, 28},         // Irkutsk neighbors
            new int[] {30, 28}, new int[] {30, 29}, new int[] {30, 32}, new int[] {30, 31}, new int[] {30, 0}, // Kamchatka neighbors
            new int[] {31, 30}, new int[] {31, 32},                                                 // Japan neighbors
            new int[] {32, 31}, new int[] {32, 30}, new int[] {32, 29}, new int[] {32, 27}, new int[] {32, 36}, // Mongolia neighbors
            new int[] {33, 26}, new int[] {33, 19}, new int[] {33, 34}, new int[] {33, 35}, new int[] {33, 36}, // Afghanistan neighbors
            new int[] {34, 19}, new int[] {34, 18}, new int[] {34, 21}, new int[] {34, 35}, new int[] {34, 33}, new int[] {34, 23}, // Middle East neighbors
            new int[] {35, 34}, new int[] {35, 33}, new int[] {35, 36}, new int[] {35, 37},         // India neighbors
            new int[] {36, 35}, new int[] {36, 37}, new int[] {36, 33}, new int[] {36, 26}, new int[] {36, 27}, new int[] {36, 32}, // China neighbors
            new int[] {37, 35}, new int[] {37, 36}, new int[] {37, 38},                             // Siam neighbors
            new int[] {38, 37}, new int[] {38, 41}, new int[] {38, 39},                             // Indonesia neighbors
            new int[] {39, 38}, new int[] {39, 41}, new int[] {39, 40},                             // New Guinea neighbors
            new int[] {40, 38}, new int[] {40, 39}, new int[] {40, 41},                             // Eastern Australia neighbors
            new int[] {41, 40}, new int[] {41, 39}                                                  // Western Australia neighbors
        };

        // Fill the adjacency matrix with edges
        foreach (var edge in edges)
        {
            int i = edge[0];
            int j = edge[1];
            adjMatrix[i, j] = 1;
            adjMatrix[j, i] = 1; // For undirected graph
        }

        // Save the adjacency matrix to a binary file
        SaveAdjMatrix("adj_matrix.bin", adjMatrix, n);
    }

    static void SaveAdjMatrix(string filePath, int[,] adjMatrix, int n)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    writer.Write(adjMatrix[i, j]);
                }
            }
        }
    }
}

