using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class HistoricObjectField : ObjectField
{
    public new class UxmlFactory : UxmlFactory<HistoricObjectField, UxmlTraits>
    {
    }

    public new class UxmlTraits : ObjectField.UxmlTraits
    {
        private UxmlStringAttributeDescription m_editorPrefKey = new UxmlStringAttributeDescription
        {
            name = "editor-pref-key",
            defaultValue = "editorPrefKey"
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            HistoricObjectField of= ((HistoricObjectField)ve);
            of.editorPrefKey = m_editorPrefKey.GetValueFromBag(bag, cc);


            string path = EditorPrefs.GetString(of.editorPrefKey);
            if (!string.IsNullOrEmpty(path))
            {
                Object setupAsset = AssetDatabase.LoadAssetAtPath(path, of.objectType);
                if (setupAsset != null)
                {
                    ((HistoricObjectField)ve).SetValueWithoutNotify(setupAsset);
                }
            }
        }
    }

    public string editorPrefKey { get; set; }
    public HistoricObjectField(string key, string label) : base(label)
    {
        editorPrefKey = key; 
    }
    public HistoricObjectField() : this(null, null) { }

    public override void SetValueWithoutNotify(Object newValue)
    {
        base.SetValueWithoutNotify(newValue);
        string assetPath = AssetDatabase.GetAssetPath(newValue);
        if (assetPath != null)
        {
            EditorPrefs.SetString(editorPrefKey, assetPath);
        }
    }
}
