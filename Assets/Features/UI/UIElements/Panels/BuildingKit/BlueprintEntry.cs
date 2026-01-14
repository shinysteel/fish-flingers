using FishFlingers.Entities;
using FishFlingers.Localisation;
using FishFlingers.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class BlueprintEntry : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        
        private LocalisationManager _localisationManager;

        private void Awake()
        {
            _localisationManager = GameManager.Instance.Get<LocalisationManager>();
        }

        public void Setup(StructureData data)
        {
            _image.sprite = data.Sprite;
            _nameText.text = _localisationManager.GetString(data.NameTerm);
            _descriptionText.text = _localisationManager.GetString(data.DescriptionTerm);
        }

        public void OnReturnedToPool()
        { }

        public void OnTakenFromPool()
        { }
    }
}