using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

[Serializable]
public class Wall_data
{
    public float start_x;
    public float start_y;
    public float end_x;
    public float end_y;
}

[Serializable]
public class Stimuli_dir
{
    public string stimuli_path;
}


[Serializable]
public class Direction_data
{
    public bool is_horizontal;
}

[Serializable]
public class Start_position
{
    public float start_x;
    public float start_y;
}

[Serializable]
public class Maze_data
{
    public Wall_data[] walls;
    public Direction_data[] directions;
    public Start_position start_position;
    public Stimuli_dir[] long_stimuli_dir;
    public Stimuli_dir[] medium_stimuli_dir;
    public Stimuli_dir[] short_stimuli_dir;
}