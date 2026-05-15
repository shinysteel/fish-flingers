using FishFlingers.UI;
using UnityEngine;

namespace FishFlingers.Entities
{
    public interface IInteractable
    {
        Transform transform { get; }
        InteractableSettings Settings { get; }
        bool CanPrompt();
        WorldUI CreatePromptUI();
        void Interact();
    }
}