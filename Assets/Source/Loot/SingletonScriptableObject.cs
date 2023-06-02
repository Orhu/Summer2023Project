using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>("Assets/Source/Loot/CardLootTable.asset");
                if (asset == null)
                {
                    throw new System.Exception("Could not find singleton object instance");
                }
                instance = asset;
            }
            return instance;
        }
    }
}
