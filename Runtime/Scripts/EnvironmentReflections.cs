using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnvironmentReflections : MonoBehaviour
{
    ReflectionProbe baker;

    public void Start()
    {
        AddReflectionProbes();
        UpdateEnvironment();
    }

    public void AddReflectionProbes()
    {
        baker = gameObject.AddComponent<ReflectionProbe>();
        baker.cullingMask = 0;
        baker.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
        baker.mode = ReflectionProbeMode.Realtime;
        baker.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;

        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
    }

    public void UpdateEnvironment() {
        StartCoroutine(UpdateEnvironmentTask());
    }

    IEnumerator UpdateEnvironmentTask()
    {
        DynamicGI.UpdateEnvironment();
        baker.RenderProbe();
        yield return new WaitForEndOfFrame();
#if UNITY_2022_1_OR_NEWER
        RenderSettings.customReflectionTexture = baker.texture;
#else
        RenderSettings.customReflection = (Cubemap)baker.texture;
#endif
    }
}
