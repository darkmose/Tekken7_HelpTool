using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{ 
    public static ObjectPooler instance;
    [SerializeField] private Transform rootOfPooledGameobjects;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    [SerializeField] private List<Pool> pools;

    [System.Serializable]
    class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    private void Start()
    {
        PreparePoolDictionary();
    }

    private void PreparePoolDictionary()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objects = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.SetParent(rootOfPooledGameobjects);
                obj.SetActive(false);
                objects.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objects);
        }
    }

    public static GameObject GetPooledGameObject(string tag)
    {
        if (instance.poolDictionary.ContainsKey(tag))
        {
            if (instance.poolDictionary[tag].Count <= 0)
            {
                return null;
            }
            Queue<GameObject> pool = instance.poolDictionary[tag];
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.parent = instance.transform;
            instance.poolDictionary[tag].Enqueue(obj);
            return obj;
        }
        Debug.Log($"'{tag}' tag doesn't exist");
        return null;
    }
}
