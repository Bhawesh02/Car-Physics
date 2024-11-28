using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// From https://github.com/JulienHeijmans/EditorScripts/blob/master/Scripts/Utility/Editor/LODExtendedUtility.cs
/// </summary>
public static class LODGroupExt
{
    #region Public Methods

    /// <summary>
    ///     Return the LODGroup component with a renderer pointing to a specific GameObject. If the GameObject is not part of a
    ///     LODGroup, returns null
    /// </summary>
    /// <param name="GO"></param>
    /// <returns></returns>
    public static LODGroup GetParentLODGroupComponent(GameObject GO)
    {
        LODGroup LODGroupParent = GO.GetComponentInParent<LODGroup>();
        if (LODGroupParent == null)
        {
            return null;
        }
        LOD[] LODs = LODGroupParent.GetLODs();

        var FoundLOD = LODs.Where(lod => lod.renderers.Where(renderer => renderer == GO.GetComponent<Renderer>()).ToArray().Count() > 0).ToArray();
        if (FoundLOD != null && FoundLOD.Count() > 0)
        {
            return LODGroupParent;
        }

        return null;
    }

    /// <summary>
    ///     Return the GameObject of the LODGroup component with a renderer pointing to a specific GameObject. If the
    ///     GameObject is not part of a LODGroup, returns null.
    /// </summary>
    /// <param name="GO"></param>
    /// <returns></returns>
    public static GameObject GetParentLODGroupGameObject(GameObject GO)
    {
        var LODGroup = GetParentLODGroupComponent(GO);

        return LODGroup == null ? null : LODGroup.gameObject;
    }

    /// <summary>
    ///     Get the LOD # of a selected GameObject. If the GameObject is not part of any LODGroup returns -1.
    /// </summary>
    /// <param name="GO"></param>
    /// <returns></returns>
    public static int GetLODid(GameObject GO)
    {
        LODGroup LODGroupParent = GO.GetComponentInParent<LODGroup>();
        if (LODGroupParent == null)
        {
            return -1;
        }
        LOD[] LODs = LODGroupParent.GetLODs();

        var index = Array.FindIndex(LODs, lod => lod.renderers.Where(renderer => renderer == GO.GetComponent<Renderer>()).ToArray().Count() > 0);
        return index;
    }

    /// <summary>
    ///     returns the currently visible LOD level of a specific LODGroup, from a specific camera. If no camera is define,
    ///     uses the Camera.current.
    /// </summary>
    /// <param name="lodGroup"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static int GetVisibleLOD(LODGroup lodGroup, Camera camera = null)
    {
        var lods           = lodGroup.GetLODs();
        var relativeHeight = GetRelativeHeight(lodGroup, camera ?? Camera.current);

        int lodIndex = GetMaxLOD(lodGroup);
        for (var i = 0; i < lods.Length; i++)
        {
            var lod = lods[i];

            if (relativeHeight >= lod.screenRelativeTransitionHeight)
            {
                lodIndex = i;
                break;
            }
        }

        return lodIndex;
    }

    /// <summary>
    ///     returns the currently visible LOD level of a specific LODGroup, from a the SceneView Camera.
    /// </summary>
    /// <param name="lodGroup"></param>
    /// <returns></returns>
    public static int GetVisibleLODSceneView(LODGroup lodGroup)
    {
        Camera camera = SceneView.lastActiveSceneView.camera;
        return GetVisibleLOD(lodGroup, camera);
    }

    /// <summary>
    ///     Gets the maximum LOD level
    /// </summary>
    /// <param name="lodGroup"></param>
    /// <returns></returns>
    public static int GetMaxLOD(LODGroup lodGroup)
    {
        return lodGroup.lodCount - 1;
    }

    /// <summary>
    ///     Gets the size in world space
    /// </summary>
    /// <param name="lodGroup"></param>
    /// <returns></returns>
    public static float GetWorldSpaceSize(LODGroup lodGroup)
    {
        return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
    }

    #endregion

    #region Private Methods

    private static float GetRelativeHeight(LODGroup lodGroup, Camera camera)
    {
        var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
        return DistanceToRelativeHeight(camera, distance / QualitySettings.lodBias, GetWorldSpaceSize(lodGroup));
    }

    private static float DistanceToRelativeHeight(Camera camera, float distance, float size)
    {
        if (camera.orthographic)
        {
            return size * 0.5F / camera.orthographicSize;
        }

        var halfAngle      = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
        var relativeHeight = size * 0.5F / (distance * halfAngle);
        return relativeHeight;
    }

    private static float GetWorldSpaceScale(Transform t)
    {
        var   scale       = t.lossyScale;
        float largestAxis = Mathf.Abs(scale.x);
        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
        return largestAxis;
    }

    #endregion
}