using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class CustomProperty<T>
{
    [JsonProperty]
    private T _value;
    public T Value
    {
        get { return _value; }
        set 
        { 
            _value = value;
            foreach(var action in _actions)
            {
                action?.Invoke(_value);
            }
        }
    }

    private List<Action<T>> _actions = new List<Action<T>>();

    public void Subscribe(Action<T> action)
    {
        _actions.Add(action);
    }

    public void Unsubscribe(Action<T> action)
    {
        _actions.Remove(action);
    }
}

[Serializable]
public class CustomPropertyN : CustomProperty<int> { }

[Serializable]
public class CustomPropertyF : CustomProperty<float> { }
