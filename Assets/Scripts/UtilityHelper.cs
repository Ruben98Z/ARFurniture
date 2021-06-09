using UnityEngine;

public static class UtilityHelper
{



    public static void EnableRendererColliderCanvas(GameObject gameObject, bool enable)
    {
        var rendererComponents = gameObject.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = gameObject.GetComponentsInChildren<Collider>(true);
        var canvasComponents = gameObject.GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = enable;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = enable;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = enable;
    }
}
