using UnityEngine;
using System.Collections;

public static class Utilities
{
    public static void ParticleColorOverLifetime(this GameObject obj, bool enabled)
    {
        if (obj != null)
        {
            var colorOvertime = obj.GetComponent<ParticleSystem>().colorOverLifetime;
            colorOvertime.enabled = enabled;
            if (enabled)
                colorOvertime.color = new ParticleSystem.MinMaxGradient(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }
    }

    public static void ParticleColorOverLifetime(this GameObject obj, Color color)
    {
        if (obj != null)
        {
            var colorOvertime = obj.GetComponent<ParticleSystem>().colorOverLifetime;
            colorOvertime.enabled = true;
            colorOvertime.color = new ParticleSystem.MinMaxGradient(color);
        }
    }

    public static void ParticleColorOverLifetime(this GameObject obj, string color)
    {
        if (obj != null)
        {
            var colorOvertime = obj.GetComponent<ParticleSystem>().colorOverLifetime;
            colorOvertime.enabled = true;
            Color newColor;
            if (ColorUtility.TryParseHtmlString(color, out newColor))
                colorOvertime.color = new ParticleSystem.MinMaxGradient(newColor);
        }
    }

    public static void Hide(this GameObject obj)
    {
        obj.transform.localScale = new Vector3(0,0,0);
    }

    public static void Hide(this string ctrlName)
    {
        GameObject.Find(ctrlName).Hide();
    }

    public static void Show(this GameObject obj)
    {
        obj.transform.localScale = new Vector3(1,1,1);
    }

    public static void Show(this string ctrlName)
    {
        GameObject.Find(ctrlName).Show();
    }

    public static bool AlmostEquals(this float double1, float double2, float precision = 0.0000001f)
    {
        return (System.Math.Abs(double1 - double2) <= precision);
    }
}
