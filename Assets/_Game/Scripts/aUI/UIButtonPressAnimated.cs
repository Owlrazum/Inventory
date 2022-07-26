using System.Collections;
using UnityEngine;

public class UIButtonPressAnimated : UIButton
{
    [SerializeField]
    private Color _pressColor = Color.white;

    [SerializeField]
    private Color _idleColor = Color.white;

    [SerializeField]
    private float _lerpColorHalfSpeed;

    public override void OnPointerTouch()
    {
        base.OnPointerTouch();

        StartCoroutine(PressAnimation());
    }

    private IEnumerator PressAnimation()
    {
        float lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += _lerpColorHalfSpeed * Time.deltaTime;
            _image.color = Color.Lerp(_idleColor, _pressColor, lerpParam);
            yield return null;
        }

        lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += _lerpColorHalfSpeed * Time.deltaTime;
            _image.color = Color.Lerp(_pressColor, _idleColor, lerpParam);
            yield return null;
        }
    }
}