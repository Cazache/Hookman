using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseItems : MonoBehaviour
{
    public Data data;

    public void loadData()
    {
        data = JsonUtility.FromJson<Data>(Resources.Load<TextAsset>("Files/Items").text);
    }
    public ItemData FechItem(int id)
    {
        for (int i = 0; i < data.itemData.Length; i++)
        {
            if (id == data.itemData[i].Id)
            {
                return data.itemData[i];
            }
        }
        return null;
    }

}
[System.Serializable]
public class Data
{
    public ItemData[] itemData;
}
[System.Serializable]
public class ItemData
{
    public enum Type { Garbage = 0, Weapon = 1, Item = 2 }
    public int Id;
    public string Name = null;
    public Type type = Type.Garbage;
    public int Ammunition = 0;
    public float Cd;


    public ItemData(int id = -1)
    {
        this.Id = id;
    }
}

