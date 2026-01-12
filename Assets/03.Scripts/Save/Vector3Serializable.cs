using System;

[Serializable]
public class Vector3Serializable 
{
    public float X, Y, Z;

    public Vector3Serializable(float x, float y, float z)
    {
        X = x; 
        Y = y; 
        Z = z;
    }
    public Vector3Serializable()
    {

    }
}
