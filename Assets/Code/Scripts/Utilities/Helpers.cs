using UnityEngine;

/// <summary>
///     A static class for general helpful methods
/// </summary>
public static class Helpers
{
    /// <summary>
    ///     Destroy all child objects of this transform (Unintentionally evil sounding).
    ///     Use it like so:
    ///     <code>
    /// transform.DestroyChildren();
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    #region Used in for player movement

    private static Matrix4x4
        _isoMatrix =
            Matrix4x4.Rotate(Quaternion.Euler(0, 45,
                0)); // 45 is the rotation of the camera pivot object, the camera inside it has a transform of 0,0,-10

    public static Vector3 ToIso(this Vector3 input)
    {
        return _isoMatrix.MultiplyPoint3x4(input);
    }

    #endregion
}