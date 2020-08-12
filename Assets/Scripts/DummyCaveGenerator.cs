using PCG.Generators.Planar;
using PCG.Types.Planar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCaveGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CellularAutomatonGenerator generator = new CellularAutomatonGenerator(64, 64);
        generator.Generate(true);

        string result = "";
        for(int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Cell cell = generator.Cells.Cells[pos];
                if (cell.CellType == ECellType.WALL)
                    result += "#";
                else
                    result += ".";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
