using System;
using System.Collections;
using UnityEngine;

public interface IStarter
{
    public IEnumerator Initialize(MonoBehaviour mb);

    public Type Type()
    {
        return GetType();
    }
}
