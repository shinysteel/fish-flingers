using ShinyOwl.Common.Framework;
using UnityEngine;

namespace FishFlingers.Pools
{
    [CreateAssetMenu(fileName = "TypedPoolableScanner", menuName = "Scanners/TypedPoolableScanner")]
    public class TypedPoolableScanner : AssetScanner<ITypedPoolable>
    { }
}