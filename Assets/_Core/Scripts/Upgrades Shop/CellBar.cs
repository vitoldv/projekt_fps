using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class CellBar : MonoBehaviour
{
    [SerializeField] private GameObject cellImage;
    
    private List<GameObject> cells = new List<GameObject>();

    public void SetCells(int count)
    {
        ClearCells();
        for (int i = 0; i < count; i++)
        {
            var cell = Instantiate(cellImage, gameObject.transform);
            cells.Add(cell);
        }
    }

    public void ClearCells()
    {
        foreach (var item in cells)
        {
            Destroy(item.gameObject);
        }
        cells.Clear();
    }
}
