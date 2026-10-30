using UnityEngine;

public static class InputManager
{
    public static PlayerInputActions Actions;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Actions = new PlayerInputActions();
        Actions.Enable();
    }
}