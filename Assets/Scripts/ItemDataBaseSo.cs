using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDataBaseSo : ScriptableObject
{
    public List<ItemSo> items = new List<ItemSo>();

    private Dictionary<int, ItemSo> itemsByld;
    private Dictionary<string, ItemSo> itemsByName;

    public void Initialize()
    {
        itemsByld = new Dictionary<int, ItemSo>();
        itemsByName = new Dictionary<string, ItemSo>();

        foreach ( var item in items)
        {
            itemsByld[item.id] = item;
            itemsByName[item.itemName] = item;
        }
    }

    public ItemSo GetItemByld(int id)
    {
        if(itemsByld == null)
        {
            Initialize();
        }
        if (itemsByld.TryGetValue(id, out ItemSo item))
            return item;
        return null;
    }

    public ItemSo GetItemByName(string name)
    {
        if (itemsByld == null)
        {
            Initialize();

        }
        if (itemsByName.TryGetValue(name, out ItemSo item))
            return item;
        return null;
    }

    public List<ItemSo> GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }
}
