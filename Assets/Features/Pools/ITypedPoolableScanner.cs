using ShinyOwl.Common.Framework;
using UnityEngine;

namespace FishFlingers.Pools
{
    [CreateAssetMenu(fileName = "ITypedPoolableScanner", menuName = "Scanners/ITypedPoolableScanner")]
    public class ITypedPoolableScanner : AssetScanner<ITypedPoolable>
    { }
}