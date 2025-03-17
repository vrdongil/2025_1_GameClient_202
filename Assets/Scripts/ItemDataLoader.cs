using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Text;

public class ItemDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "items";
    private List<ItemData> itemList;

    // Start is called before the first frame update
    void Start()
    {
        LoadItemData();
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
    
    void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        
        if(jsonFile != null)
        {
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correntText = Encoding.UTF8.GetString(bytes);

            itemList = JsonConvert.DeserializeObject<List<ItemData>>(correntText);

            Debug.Log($"로드된 아이템 수 : {itemList.Count}");

            foreach(var item in itemList)
            {
                Debug.Log($"아이템: {EncodeKorean(item.itemName)}, 설명 : {EncodeKorean(item.description)}");

            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다.: {jsonFileName}");
        }
    }
}
