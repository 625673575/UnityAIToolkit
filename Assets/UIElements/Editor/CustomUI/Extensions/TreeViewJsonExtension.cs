using System.Collections.Generic;
using UnityEngine.UIElements;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class TreeViewJsonExtension
{
    public static string LoadJson(this TreeView treeView, string json, string rootName = "root", bool fixPath = true, System.Action<object> onChosen = null)
    {
        void AddObjectNodes(JObject @object, string name, ref int itemIndex, List<TreeViewItemData<string>> parent)
        {
            ++itemIndex;
            var node = new List<TreeViewItemData<string>>();
            parent.Add(new TreeViewItemData<string>(itemIndex, name, node));
            foreach (var property in @object.Properties())
            {
                AddTokenNodes(property.Value, property.Name, ref itemIndex, node);
            }
        }
        void AddArrayNodes(JArray array, string name, ref int itemIndex, List<TreeViewItemData<string>> parent)
        {
            var node = new List<TreeViewItemData<string>>();
            parent.Add(new TreeViewItemData<string>(itemIndex, name, node));
            for (var i = 0; i < array.Count; i++)
            {
                ++itemIndex;
                AddTokenNodes(array[i], string.Format("[{0}]", i), ref itemIndex, node);
            }
        }
        void AddTokenNodes(JToken token, string name, ref int itemIndex, List<TreeViewItemData<string>> parent)
        {
            ++itemIndex;
            if (token is JValue)
            {
                parent.Add(new TreeViewItemData<string>(itemIndex, string.Format("{0}: {1}", name, ((JValue)token).Value)));
            }
            else if (token is JArray)
            {
                AddArrayNodes((JArray)token, name, ref itemIndex, parent);
            }
            else if (token is JObject)
            {
                AddObjectNodes((JObject)token, name, ref itemIndex, parent);
            }
        }
        if (treeView.makeItem == null || treeView.bindItem == null)
        {
            treeView.makeItem = () => new Label();
            treeView.bindItem = (e, i) =>
            {
                var item = treeView.GetItemDataForIndex<string>(i);
                (e as Label).text = item;
            };
        }
        if (onChosen != null)
            treeView.selectionChanged += onChosen;

        var items = new List<TreeViewItemData<string>>(110);
        for (var i = 0; i < 10; i++)
        {
            var itemIndex = i * 10 + i;

            var treeViewSubItemsData = new List<TreeViewItemData<string>>(10);
            for (var j = 0; j < 10; j++)
                treeViewSubItemsData.Add(new TreeViewItemData<string>(itemIndex + j + 1, (j + 1).ToString()));

            var treeViewItemData = new TreeViewItemData<string>(itemIndex, (i + 1).ToString(), treeViewSubItemsData);
            items.Add(treeViewItemData);
        };
        try
        {
            if (fixPath)
            {
                json = json.Replace("\\", "/");
                json = json.Replace("//", "/");
            }
            if (json.StartsWith('['))
            {
                json = "{\"array\":" + json + "}";
            }
            var obj = JObject.Parse(json);
            var jsonTree = new List<TreeViewItemData<string>>();
            int rootIndex = -1;
            AddObjectNodes(obj, rootName ?? "Root", ref rootIndex, jsonTree);
            treeView.SetRootItems(jsonTree);
            treeView.Rebuild();
        }
        catch (JsonReaderException e)
        {
            Debug.LogError(e.Message);
        }
        return json;
    }
}
