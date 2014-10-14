using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

namespace P1
{
  public class Utils
  {
    public enum Dimension
    {
      X,
      Y,
      Z}
    ;
    
    public static bool Elapsed(float startTime, float duration)
    {
      return (startTime + duration < Time.time);
    }

    public static bool Elapsed(float startTime, float duration, UnityEngine.Object item)
    {
      if (item == null)
        return false;
      return Elapsed(startTime, duration);
    }

    #region smoothing

    public static float ExponentialSmoothing(float oldValue, float newValue, float smoothing)
    {
      if (oldValue == newValue)
        return newValue;
      smoothing = Mathf.Clamp(smoothing, 0f, 1f);
      return oldValue * smoothing + newValue * (1 - smoothing);
    }
  
    public static float ExponentialSmoothing(float oldValue, float newValue, float smoothingIn, float smoothingOut)
    {
      return ExponentialSmoothing(oldValue, newValue, ((newValue > oldValue) ? smoothingIn : smoothingOut));
    }
#endregion

#region count
/**
 these methods are sugar on "++"/"--" type incrementing
*/
    public static int AddCount(int value)
    {
      return ++value;
    }

    public static int AddCount(int value, int max)
    {
      return Mathf.Clamp(value + 1, 0, max);
    }
    
    public static int SubCount(int value)
    {
      return Mathf.Max(value - 1, 0);
    }

    public static int SubCount(int value, int max)
    {
      return Mathf.Clamp(value - 1, 0, max);
    }
#endregion

#region file io
    
    // Use this for initialization
    public static JSONNode FileToJSON(string filename, string directory = ".")
    {
      if (directory == ".")
      {
        directory = Environment.CurrentDirectory + "/config/";
      }
      return JSON.Parse(File.ReadAllText(directory + filename));
    }

#endregion

#region colors
    
    static  Dictionary<string, Color>  colors_ = new Dictionary<string, Color> ();
    
    public static void ReadColors()
    {
      ReadColors("Assets/Blockform/colors.json");
    }

    public static void ReadColors(string path)
    {
      JSONNode JSONroot = FileToJSON(path);
      int len = JSONroot ["colors"].Count;
      //   Debug.Log ("count of colors: " + len);

      colors_.Clear();
      
      for (int i = 0; i < len; ++i) {
        JSONNode color = JSONroot ["colors"] [i];
        string n = color ["name"].Value;
        colors_.Add(n, new Color (color ["red"].AsFloat, color ["green"].AsFloat, color ["blue"].AsFloat));
      }
    }

    public static bool GetColorFromConfig(string name, out Color namedColor)
    {
      namedColor = Color.white; // ensuring a value is retuned;
      if (colors_.Count == 0) {
        ReadColors();
      }

      if (colors_.ContainsKey(name)) {
        namedColor = colors_ [name];
        return true;
      } else {
        return false;
      }
    }

#endregion

    #region Vector3 
    public static Vector3 CloneV3(Vector3 v) { return new Vector3(v.x, v.y, v.z); }
#endregion

    #region mainDim
    public static Dimension MainDim(Vector3 a, Vector3 b)
    {
      return MainDim(a - b);
    }

    /**
     * returns the dimension with the largest absolute value;
     * in the case of a tie perfers X , Y, and Z in that order
    */

    public static Dimension MainDim(Vector3 a)
    {
      if (Mathf.Abs(a.x) >= Mathf.Abs(a.y)) {
        if (Mathf.Abs(a.x) >= Mathf.Abs(a.z)) {
          return Dimension.X;
        } else {
          return Dimension.Z;
        }
      } else if (Mathf.Abs(a.y) >= Mathf.Abs(a.z)) {
        return Dimension.Y;
      } else {
        return Dimension.Z;
      }
    }
      #endregion
      
  }
  
  
}