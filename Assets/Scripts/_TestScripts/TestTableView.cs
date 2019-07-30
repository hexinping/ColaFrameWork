using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using ColaFramework;

public class TestTableView : MonoBehaviour
{

    private UITableView tableView;

    public int CellCount = 10;

    public GameObject cellPrefab = null;
    // Use this for initialization
    void Start()
    {
        InitTableView();
    }

    void InitTableView()
    {
        tableView = GetComponent<UITableView>();
        tableView.SetTotalCellCallback(TotalCellCount);
        tableView.SetCellByIndexCallback(GetCellByIndex);
        tableView.Reload(true);
        //tableView.Reload(100);
    }

    /// <summary>
    /// tableview callback
    /// </summary>
    /// <returns></returns>
    int TotalCellCount()
    {
        return CellCount;
    }

    /// <summary>
    /// tableview callback
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    UITableViewCell GetCellByIndex(int index)
    {
        UITableViewCell cell = tableView.GetReUseCell();
        if (cell == null)
        {
            GameObject go = Instantiate(cellPrefab) as GameObject;
            cell = go.GetComponent<UITableViewCell>();
            cell.gameObject = go;
            cell.extenParams["Label"] = CommonHelper.GetComponentByPath<Text>(go, "Text");
        }
        cell.index = index;
        //Text label = cell.transform.GetComponentInChildren<Text>();
        //label.text = "cell_" + index;
        Refresh(cell, index);
        return cell;
    }

    private void Refresh(UITableViewCell cell, int index)
    {
        Text text = cell.extenParams["Label"] as Text;
        text.text = "cell_" + index;
    }
}
