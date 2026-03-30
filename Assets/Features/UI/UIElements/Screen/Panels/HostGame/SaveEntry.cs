using FishFlingers.Networking;
using FishFlingers.Pools;
using FishFlingers.Saving;
using ShinyOwl.Common;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveEntry : MonoBehaviour, IPoolable
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _thumbnailImage;
    [SerializeField] private AspectRatioFitter _thumbnailAspectRatioFitter;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _plusText;

    private SaveManager _saveManager;
    private LobbyManager _lobbyManager;

    private SaveFile _saveFile;

    private void Awake()
    {
        _saveManager = GameManager.Instance.Get<SaveManager>();
        _lobbyManager = GameManager.Instance.Get<LobbyManager>();
    }

    private void Start()
    {
        _button.onClick.AddListener(Pressed);
    }

    private void Pressed()
    {
        SaveFile file = _saveFile;

        if (file == null)
        {
            file = new SaveFile(null);
            _saveManager.AddSaveFile(file);
        }

        _saveManager.SelectSaveFile(file);

        _ = _lobbyManager.CreateLobbyAsync();
    }

    public void Setup(SaveFile file)
    {
        _saveFile = file;

        _thumbnailImage.gameObject.SetActive(file != null);
        _nameText.gameObject.SetActive(file != null);
        _plusText.gameObject.SetActive(file == null);

        if (file != null)
        {
            _nameText.text = Path.GetFileName(file.FolderPath);

            byte[] bytes = File.ReadAllBytes(file.ThumbnailPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            _thumbnailImage.sprite = sprite;
            _thumbnailAspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
        }
    }

    public void OnReturnedToPool()
    {
        _saveFile = null;
    }

    public void OnTakenFromPool()
    { }
}
