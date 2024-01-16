using System;
using UnityEngine;

public static class JsonHelper
{
    
    public static T FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }
    
    public static string ToJson<T>(T array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    } 
    
    public static T[] FromJsonArray<T>(string json)
    {
        WrapperArray<T> wrapperArray = JsonUtility.FromJson<WrapperArray<T>>(json);
        return wrapperArray.items;
    }

    public static string ToJsonArray<T>(T[] array)
    {
        WrapperArray<T> wrapperArray = new WrapperArray<T>();
        wrapperArray.items = array;
        return JsonUtility.ToJson(wrapperArray);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        WrapperArray<T> wrapperArray = new WrapperArray<T>();
        wrapperArray.items = array;
        return JsonUtility.ToJson(wrapperArray, prettyPrint);
    }

    [Serializable]
    private class WrapperArray<T>
    {
        public T[] items;
    }
    
    [Serializable]
    private class Wrapper<T>
    {
        public T items;
    }
}