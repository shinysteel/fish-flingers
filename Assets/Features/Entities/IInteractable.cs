using FishFlingers.UI;
using UnityEngine;

namespace FishFlingers.Entities
{
    public interface IInteractable
    {
        Transform transform { get; }
        IInteractableSettings Settings { get; }
        bool CanPrompt();
        WorldUI CreatePromptUI();
        bool CanInteract();
        void Interact();
    }
}