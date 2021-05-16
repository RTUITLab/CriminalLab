using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public ThreadData[] ThreadDatas { get; set; }
    public NoteData[] NoteDatas { get; set; }
}

[Serializable]
public class ThreadData
{
    public string TagFrom { get; set; }
    public string TagTo { get; set; }
}

[Serializable]
public class NoteData
{
    public SerializedVector3 NotePosition { get; set; }
    public string AudioClipPath { get; set; }
    public string Tag { get; set; }
}

[Serializable]
public class SerializedVector3
{
    public float[] position = new float[3];

    public SerializedVector3(Vector3 vector3)
    {
        position[0] = vector3.x;
        position[1] = vector3.y;
        position[2] = vector3.z;
    }

    public static void DeserializedVector3(ref Vector3 vector3, SerializedVector3 serializedVector3)
    {
        vector3.x = serializedVector3.position[0];
        vector3.y = serializedVector3.position[1];
        vector3.z = serializedVector3.position[2];
    }
}