#if UNITY_EDITOR
using System.Reflection;
#endif

using UnityEngine;

public static class CustomDebug
{

    public static void ClearLog()
    { 
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }

    public static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color,bool drawGizmo=false)
	{	
		if (drawGizmo) 
		{
			Debug.DrawRay (rayOriginPoint, rayDirection * rayDistance, color);
		}
		return Physics2D.Raycast(rayOriginPoint,rayDirection,rayDistance,mask);		
	}
}

