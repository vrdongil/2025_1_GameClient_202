#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System. IO;

using UnityEngine;
using Newtonsoft.Json;


public class JsonToScriptableConventor : EditorWindow
{
    private string jsonFilePath = "";
    private string outputFolder = "Assets/ScrriptableObjects/items";
    private bool createDatabase = true;

    [MenuItem("Tools/Json to Scripatable Objects")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConventor>( "JSON to Scriptable Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scripatble Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSONFile", "", "json");

        }
        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();
        outputFolder = EditorGUILayout.TextField("Output Folder :", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert to Scripatable Objects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a JSON file first!", "OK");
                return;
            }
            ConvertJsonToScripatbleObjects();
        }
    }

    private void ConvertJsonToScripatbleObjects()
    {
        if(!Directory. Exists(outputFolder))
            {
            Directory.CreateDirectory(outputFolder);

        }
        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {

            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSo> createditems = new List<ItemSo>();

            foreach (var ItemData in itemDataList)
            {
                ItemSo itemSo = ScriptableObject.CreateInstance<ItemSo>();

                itemSo.id = ItemData.id;
                itemSo.itemName = ItemData.itemName;
                itemSo.nameEng = ItemData.nameEng;
                itemSo.description = ItemData.description;

                if(System.Enum.TryParse(ItemData.itemTypeString, out ItemType parsedType))
                {
                    itemSo. itemType = parsedType;

                }
                else
                {
                    Debug.LogWarning($"������ '{ItemData.itemName}'�� ��ȿ���� ���� Ÿ��: {ItemData.itemTypeString} ");
                }
                itemSo.price = ItemData.price;
                itemSo.power = ItemData.power;
                itemSo.level = ItemData.level;
                itemSo.isStackable = ItemData.isStackable;

                if(!string.IsNullOrEmpty(ItemData.iconPath))
                {
                    itemSo.icon = AssetDatabase.LoadAssetAtPath < Sprite>($"Assets/Resources/{ItemData.iconPath}.png");

                    if(itemSo.icon == null)
                    {
                        Debug.LogWarning($"������ '{ItemData.nameEng}'�� �������� ã�� �� �����ϴ�. :{ItemData.iconPath}");
                    }
                }

                string assetPath = $"{outputFolder}/Item_{ItemData.id. ToString("D4")}_{ItemData.nameEng}.asset";
                AssetDatabase.CreateAsset(itemSo, assetPath);

                itemSo.name = $"item {ItemData.id.ToString("D4")}_{ItemData.nameEng}";
                createditems.Add(itemSo);

                EditorUtility.SetDirty(itemSo);
            }
            if (createDatabase && createditems.Count > 0)
            {
                ItemDataBaseSo database = ScriptableObject.CreateInstance<ItemDataBaseSo>();
                database.items = createditems;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/ItemDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createditems.Count} scriptable objects!", "OK");


        }


        catch ( System.Exception e )
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON ��ȯ ����: {e}");
        }


    }
}
#endif