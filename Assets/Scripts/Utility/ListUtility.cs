using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListUtility : MonoSingleton<ListUtility>
{
    public List<T> ShuffleList<T>(List<T> _list)
    {
        List<T> tempList = _list;

        for (int i = 0; i < tempList.Count; i++)
        {
            int rand = Random.Range(i, tempList.Count);
            T temp = tempList[i];
            tempList[i] = tempList[rand];
            tempList[rand] = temp;
        }

        return tempList;
    }
}
