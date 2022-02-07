using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class InventoryItemSO : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public void CreateVisualGrid(Transform visualParentTransform, InventoryItemSO itemTetrisSO, float cellSize)
    {
        Transform visualTransform = Instantiate(prefab, visualParentTransform);

        // Create background
        //Transform template = visualTransform.Find("Template");
        //template.gameObject.SetActive(false);

        //for (int x = 0; x < itemTetrisSO.width; x++)
        //{
        //    for (int y = 0; y < itemTetrisSO.height; y++)
        //    {
        //        Transform backgroundSingleTransform = Instantiate(template, visualTransform);
        //        backgroundSingleTransform.gameObject.SetActive(true);
        //    }
        //}

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;

        visualTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(itemTetrisSO.width, itemTetrisSO.height) * cellSize;

        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        visualTransform.SetAsFirstSibling();
    }



    public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPositionList.Add(offset + new Vector2Int(x, y));
            }
        }
        return gridPositionList;
    }


}
