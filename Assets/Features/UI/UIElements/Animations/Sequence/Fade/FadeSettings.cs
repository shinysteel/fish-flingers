using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeSettings", menuName = "Settings/UI/UIAnimations/FadeSettings")]
public class FadeSettings : ScriptableObject
{
    [SerializeField] private float _duration = 0.1f;
    [SerializeField] private Ease _ease;

    public float Duration => _duration;
    public Ease Ease => _ease;
}
