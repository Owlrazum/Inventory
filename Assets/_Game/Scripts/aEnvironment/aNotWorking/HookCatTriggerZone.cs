// using UnityEngine;

// using MoreMountains.Tools;
// using MoreMountains.CorgiEngine;

// public class HookCatTriggerZone : MonoBehaviour
// {
//     private HookCatAnchor _anchor;

//     private void Awake()
//     {
//         _anchor = transform.parent.GetComponent<HookCatAnchor>();
// #if UNITY_EDITOR
//         if (gameObject.layer != LayersManager.HOOK_CAT_ZONE_LAYER)
//         {
//             Debug.LogError("The hootcatzonelayerisnotequaltotheonespecifiedintthelayersmanager");
//         }
// #endif
//     }

//     public HookCatAnchor GetAnchor()
//     {
//         return _anchor;
//     }
// }