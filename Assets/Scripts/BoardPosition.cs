using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardPosition : MonoBehaviour
{
    public GameObject xPrefab;
    public GameObject oPrefab;

    public BoardStatus status = BoardStatus.Empty;
    public enum BoardStatus
    {
        X,
        O,
        Empty
    }

    public void OnStatusChange()
    {
        if (status == BoardStatus.X)
        {
            if (transform.childCount > 0)
            {
                if (transform.childCount > 0)
                    Destroy(transform.GetChild(0).gameObject);
            }
            GameObject child = Instantiate(xPrefab);
            child.transform.SetParent(transform, false);
        }
        else if (status == BoardStatus.O)
        {
            if (transform.childCount > 0)
            {
                if (transform.childCount > 0)
                    Destroy(transform.GetChild(0).gameObject);
            }
            GameObject child = Instantiate(oPrefab);
            child.transform.SetParent(transform, false);
        }
        else if (status == BoardStatus.Empty)
        {
            if (transform.childCount > 0)
                Destroy(transform.GetChild(0).gameObject);
        }
    }
}
