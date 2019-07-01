using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景角色类
/// </summary>
public class SceneCharacter : ISceneCharacter
{
    public GameObject gameObject { get; set; }

    public Transform transform { get; set; }

    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Vector3 Rotation
    {
        get { return transform.localRotation.eulerAngles; }
        set
        {
            Quaternion quaternion = Quaternion.Euler(value.x, value.y, value.z);
            transform.localRotation = quaternion;
        }
    }

    public Vector3 Direction
    {
        get { return transform.forward; }
        set { transform.forward = value; }
    }

    public bool Visible
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    /// <summary>
    /// 构造函数私有化，外部只能使用工厂方法接口创建
    /// </summary>
    private SceneCharacter(GameObject entity)
    {
        gameObject = entity;
        transform = entity.transform;
    }

    /// <summary>
    /// 工厂方法，创建ISceneCharacter
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static ISceneCharacter CreateSceneCharacterInf(string path)
    {
        return CreateSceneCharacter(path);
    }

    /// <summary>
    /// 工厂方法，创建SceneCharacter
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SceneCharacter CreateSceneCharacter(string path)
    {
        GameObject prefab = AssetLoader.Load<GameObject>(path);
        GameObject Entity = CommonHelper.InstantiateGoByPrefab(prefab, null);
        return new SceneCharacter(Entity);
    }

    void ISceneCharacter.SetPosition2D(float x, float z)
    {
        Position.Set(x, Position.y, z);
    }

    void ISceneCharacter.SetRotation2D(float x, float z)
    {
        transform.forward = new Vector3(x, 0, z);
    }

    void ISceneCharacter.Release()
    {
        //暂时先直接删除，后期要替换成回收到对象池
        Position = Vector3.zero;
        Rotation = Vector3.zero;
        Direction = Vector3.zero;
        transform = null;

        GameObject.Destroy(gameObject);
        gameObject = null;
    }
}
