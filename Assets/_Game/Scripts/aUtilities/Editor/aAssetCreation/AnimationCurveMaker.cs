using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates AnimationCurveForFormTransitioning
/// </summary>
public class AnimationCurveMaker : MonoBehaviour
{
    [SerializeField]
    private float _radius = 1;

    // [SerializeField]
    // private float _lerpSpeed = 1;

    [SerializeField]
    private Vector3 _sphericalStateScale = Vector3.one * 0.75f;

    [SerializeField]
    private int _keyFramesAmount = 20;

    private enum CreateMode
    { 
        ToHumanoid,
        ToSpherical
    }

    [SerializeField]
    private CreateMode _createMode;

    [SerializeField]
    private AnimationClip _toSphericalClip;

    private List<Transform> _plates;
    
    private List<float> _humPlateAngles;
    private List<float> _humPlateRadii;

    private List<float> _spherePlateAngles;

    private Vector3 _humanoidStateScale;

    // 0 for humanoid, 1 for spherical
    private float _lerpParam;

    private void Awake()
    {
        _plates = new List<Transform>();
        _humPlateAngles = new List<float>();
        _humPlateRadii = new List<float>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform plate = transform.GetChild(i);
            _plates.Add(plate);

            float atan2Angle = Mathf.Atan2(plate.localPosition.y, plate.localPosition.x);
            float angle = CustomMath.ConvertAtan2AngleTo360(atan2Angle);
            _humPlateAngles.Add(angle);
            _humPlateRadii.Add(plate.localPosition.magnitude);
        }

        _humanoidStateScale = _plates[0].localScale;

        int lastPlateIndex = _humPlateAngles.Count - 1;
        float deltaAngleDeg = 360 / _plates.Count;

        _spherePlateAngles = new List<float>();
        for (int i = 0; i < _plates.Count; i++)
        {
            float angle = deltaAngleDeg * i;
            _spherePlateAngles.Add(angle);
        }

        if (_createMode == CreateMode.ToSpherical)
        {
            CreateSphericalAnimationClip();
        }
        else if (_createMode == CreateMode.ToHumanoid)
        {
            CreateHumanoidAnimationClip();
        }
    }

    private void CreateSphericalAnimationClip()
    { 
        _lerpParam = 0;

        AnimationClip spherical = new AnimationClip();
        List<AnimationCurve> sphericalCurves = GenerateSphericalAnimationCurves();

        int curveIndex = 0;
        for (int i = 0; i < _plates.Count; i++)
        { 
            spherical.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localPosition.x", sphericalCurves[curveIndex++]);
            spherical.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localPosition.y", sphericalCurves[curveIndex++]);
            spherical.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localPosition.z", sphericalCurves[curveIndex++]);
 
            spherical.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localScale.x", sphericalCurves[curveIndex++]);
            spherical.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localScale.y", sphericalCurves[curveIndex++]);
            spherical.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localScale.z", sphericalCurves[curveIndex++]);
        }

        AssetDatabase.CreateAsset(spherical, "Assets/aGame/Animations/ToSphericalClip.anim");
        AssetDatabase.SaveAssets();
    }

    private List<AnimationCurve> GenerateSphericalAnimationCurves()
    {
        List<AnimationCurve> curves = new List<AnimationCurve>();
        for (int i = 0; i < _plates.Count * 6; i++)
        {
            curves.Add(new AnimationCurve());
        }
        _lerpParam = 0;
        float deltaKey = 1.0f / _keyFramesAmount;
        for (int k = 0; k < _keyFramesAmount; k++)
        {
            _lerpParam = k * deltaKey;
            float easedLerpParam = CustomMath.EaseOut(_lerpParam);
            int curveIndex = 0;
            for (int i = 0; i < _plates.Count; i++)
            {
                float angle360 = Mathf.Lerp(_humPlateAngles[i], _spherePlateAngles[i], easedLerpParam);
                float atan2Angle = CustomMath.ConvertAngle360ToAtan2(angle360);

                float lerpedRadius = Mathf.Lerp(_humPlateRadii[i], _radius, easedLerpParam);
                Vector3 localPosition = CustomMath.ComputePointOnCircleXY(lerpedRadius, atan2Angle);
                Vector3 localScale = Vector3.Lerp(_humanoidStateScale, _sphericalStateScale, easedLerpParam);
                curves[curveIndex++].AddKey(_lerpParam, localPosition.x);
                curves[curveIndex++].AddKey(_lerpParam, localPosition.y);
                curves[curveIndex++].AddKey(_lerpParam, localPosition.z);

                curves[curveIndex++].AddKey(_lerpParam, localScale.x);
                curves[curveIndex++].AddKey(_lerpParam, localScale.y);
                curves[curveIndex++].AddKey(_lerpParam, localScale.z);
            }
        }

        return curves;
    }

    private void CreateHumanoidAnimationClip()
    { 
        _lerpParam = 0;

        AnimationClip humanoidClip = new AnimationClip();
        List<AnimationCurve> humanoidCurves = GenerateHumanoidAnimationCurves();

        int curveIndex = 0;
        for (int i = 0; i < _plates.Count; i++)
        { 
            humanoidClip.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localPosition.x", humanoidCurves[curveIndex++]);
            humanoidClip.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localPosition.y", humanoidCurves[curveIndex++]);
            humanoidClip.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localPosition.z", humanoidCurves[curveIndex++]);

            humanoidClip.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localScale.x", humanoidCurves[curveIndex++]);
            humanoidClip.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localScale.y", humanoidCurves[curveIndex++]);
            humanoidClip.SetCurve("Sphere (" + (i + 1) + ")", typeof(Transform), "localScale.z", humanoidCurves[curveIndex++]);
        }

        AssetDatabase.CreateAsset(humanoidClip, "Assets/aGame/Animations/ToHumanoidClip.anim");
        AssetDatabase.SaveAssets();
    }

    private List<AnimationCurve> GenerateHumanoidAnimationCurves()
    {
        List<AnimationCurve> curves = new List<AnimationCurve>();
        for (int i = 0; i < _plates.Count * 6; i++)
        {
            curves.Add(new AnimationCurve());
        }

        float deltaKeyTime = 1.0f / _keyFramesAmount;
        float sourceTime = 1 - deltaKeyTime;
        float destinationTime = 0;

        for (int k = 0; k < _keyFramesAmount; k++)
        {
            _toSphericalClip.SampleAnimation(gameObject, sourceTime);
            int curveIndex = 0;
            for (int i = 0; i < _plates.Count; i++)
            {
                curves[curveIndex++].AddKey(destinationTime, gameObject.transform.GetChild(i).localPosition.x);
                curves[curveIndex++].AddKey(destinationTime, gameObject.transform.GetChild(i).localPosition.y);
                curves[curveIndex++].AddKey(destinationTime, gameObject.transform.GetChild(i).localPosition.z);

                curves[curveIndex++].AddKey(destinationTime, gameObject.transform.GetChild(i).localScale.x);
                curves[curveIndex++].AddKey(destinationTime, gameObject.transform.GetChild(i).localScale.y);
                curves[curveIndex++].AddKey(destinationTime, gameObject.transform.GetChild(i).localScale.z);
            }

            sourceTime -= deltaKeyTime;
            destinationTime += deltaKeyTime;
        }

        return curves;
    }
}