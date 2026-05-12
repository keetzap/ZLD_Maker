using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionableType : ScriptableObject
{
    public Id Id;
}

//-------

public class CollectionableSO : ScriptableObject
{
    public CollectionableType CollectionableType;
    public Id Id;
}
    
public class CollectionableKey : CollectionableSO , CollectionableUniqueElement
{
    public Sprite Sprite { get; }
    public int Amount { get; } = 1;
}

public class Collecionable : MonoBehaviour
{
    public CollectionableSO Config;
}

public interface CollectionableUniqueElement
{
    public Sprite Sprite { get; }
    public int Amount { get; }
}


public class GAMEDATA
{
    private List<CollectionableList> CollectionableLists;

    public CollectionableList GetAllCollectionablesOfType(CollectionableType type)
    {
        return CollectionableLists.FirstOrDefault(el => el.Type == type);
    }

    //public CollectionableSO GetConfig(Id id)
    //{
    //    CollectionableLists.ForEach(collectionableList => collectionableList.Items.FirstOrDefault(element => element.Id == id));
    //}
}

public class CollectionableList : ScriptableObject
{
    public CollectionableType Type;
    public List<CollectionableSO> Items;
}


//SAVE DATA
public class PlayerStats
{
    public List<CollectionableSerializable> Collectionable;

    public void AddCollectionable(CollectionableSO so)
    {
        Id id;
        id = "asdfsadsa";
    }
}

[Serializable]
public class CollectionableSerializable
{
    public Id CollectionableTypeId;
    public Id Id;
    public int Amount;
}

[Serializable]
public class Id
{
    [SerializeField]
    private string _id;
    
    public static implicit operator Id(string newId) => new Id(newId);

    public override string ToString()
    {
        return _id;
    }

    public Id(string id)
    {
        _id = id;
    }
}

public class KeyId : Id
{
    public KeyId(string id) : base(id)
    {}
}


//UI
class CollectionableSectionAdapter
{
    [SerializeField]
    private List<CollectionableSectionUI> _uis;
    
    public void AddCollectionable(CollectionableSO collectionableSo)
    {
        var result = _uis.FirstOrDefault(elemetn => elemetn.CollectionableType == collectionableSo.CollectionableType);

        result.Add(collectionableSo);
    }
}

public abstract class CollectionableSectionUI
{
    public CollectionableType CollectionableType;
    public abstract void Add(CollectionableSO collectionableSo);
}

public class HeartsUI : CollectionableSectionUI
{

    public override void Add(CollectionableSO collectionableSo)
    {
        //var heartConfig = collectionableSo as HeartCollectionable;
        //heartConfig.Amount
    }
}

public class CollectionableUniqueElementUI : CollectionableSectionUI
{
    private CollectionableUniqueElement _element;
    
    public override void Add(CollectionableSO collectionableSo)
    {
        _element = collectionableSo as CollectionableUniqueElement;
        
        
        
        //var heartConfig = collectionableSo as HeartCollectionable;
        //heartConfig.Amount
    }
}

//Dropdown
//class LookFor : PropertyDrawer
//{
//    public override VisualElement CreatePropertyGUI(SerializedProperty property)
//    {
//        List<string> assets;
//        List<CollectionableSO> assetsSO;
    
//        var assets = AssetDatabase.FindAssets(typeof(CollectionableSO).ToString());
//        AssetDatabase.GUIDToAssetPath();
//        var collectionableSo = AssetDatabase.LoadAssetAtPath<CollectionableSO>("path");
    
//        assets.AddRange(collectionableSo.name);

//        var index = EditorGUILayout.;
//        value = assetsSO[index];
        
//        return base.CreatePropertyGUI(property);
//    }
//}


//SIMPLE DEPENDENCY SYSTEM
public class Ball
{
    [SerializeField]
    private BallGameManagerDependency _ballGameManagerDependency;

    void x()
    {
        
    }
}


public interface IBallGAmeManager
{
    void SpawnBall();
}

public class FAstGameBallGameManager : IBallGAmeManager
{
    public void SpawnBall()
    {
        throw new NotImplementedException();
    }
}

public class BallGameManager : IBallGAmeManager
{
    [SerializeField]
    private BallGameManagerDependency _ballGameManagerDependency;
    public void SpawnBall()
    {
        throw new NotImplementedException();
    }
}


public class Dependency<T> : ScriptableObject
{
    public T Instance { get; private set; }

    public void SetInstance(T inst)
    {
        Instance = inst;
    }
}
public class BallGameManagerDependency : Dependency<IBallGAmeManager>
{
}