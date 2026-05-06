using ShinyOwl.Common.Framework;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "IEntityScanner", menuName = "Scanners/IEntityScanner")]
    public class IEntityScanner : AssetScanner<IEntity>
    { }
}