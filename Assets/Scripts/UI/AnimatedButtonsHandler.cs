using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedButtonsHandler : MonoBehaviour, IAnimatedButtonsHandler
{
    private Button[] children;

    private void Start()
    {
        children = GetComponentsInChildren<Button>();
    }

    public void ChangeInteractableState(int i)
    {
        bool x = Convert.ToBoolean(i);
        foreach (Button button in children)
        {
            button.interactable = x;
            button.transition = x ? Selectable.Transition.ColorTint : Selectable.Transition.None;
        }
    }
}
