using System;
using UnityEngine.Serialization;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public sealed partial class PlanarReflectionProbe : IVersionable<PlanarReflectionProbe.PlanarProbeVersion>
    {
        enum PlanarProbeVersion
        {
            Initial,
            First = 2,
            CaptureSettings,
            ProbeSettings
        }

        [SerializeField, FormerlySerializedAs("version"), FormerlySerializedAs("m_Version")]
        int m_PlanarProbeVersion;
        PlanarProbeVersion IVersionable<PlanarProbeVersion>.version { get => (PlanarProbeVersion)m_PlanarProbeVersion; set => m_PlanarProbeVersion = (int)value; }

        static readonly MigrationDescription<PlanarProbeVersion, PlanarReflectionProbe> k_PlanarProbeMigration = MigrationDescription.New(
            MigrationStep.New(PlanarProbeVersion.CaptureSettings, (PlanarReflectionProbe p) =>
            {
#pragma warning disable 618
                if (p.m_ObsoleteCaptureSettings == null)
                    p.m_ObsoleteCaptureSettings = new ObsoleteCaptureSettings();
                if (p.m_ObsoleteOverrideFieldOfView)
                    p.m_ObsoleteCaptureSettings.overrides |= ObsoleteCaptureSettingsOverrides.FieldOfview;
                p.m_ObsoleteCaptureSettings.fieldOfView = p.m_ObsoleteFieldOfViewOverride;
                p.m_ObsoleteCaptureSettings.nearClipPlane = p.m_ObsoleteCaptureNearPlane;
                p.m_ObsoleteCaptureSettings.farClipPlane = p.m_ObsoleteCaptureFarPlane;
#pragma warning restore 618
            }),
            MigrationStep.New(PlanarProbeVersion.ProbeSettings, (PlanarReflectionProbe p) =>
            {
                k_Migration.ExecuteStep(p, Version.ProbeSettings);

                // Migrate mirror position
                // Previously, the mirror as at the influence position and face the Y axis.
                // Now, the mirror is defined in proxy space and faces the Z axis.

                var mirrorPositionWS = p.transform.position;
                var mirrorRotationWS = p.transform.rotation * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                var worldToProxy = p.proxyToWorld.inverse;
                var mirrorPositionPS = worldToProxy * mirrorPositionWS;
                var mirrorRotationPS = worldToProxy.rotation * mirrorRotationWS;
                p.m_ProbeSettings.proxySettings.mirrorPositionProxySpace = mirrorPositionPS;
                p.m_ProbeSettings.proxySettings.mirrorRotationProxySpace = mirrorRotationPS;
            })
        );

        // Obsolete Properties
        [SerializeField, FormerlySerializedAs("m_CaptureNearPlane"), Obsolete("For data migration")]
        float m_ObsoleteCaptureNearPlane = ObsoleteCaptureSettings.@default.nearClipPlane;
        [SerializeField, FormerlySerializedAs("m_CaptureFarPlane"), Obsolete("For data migration")]
        float m_ObsoleteCaptureFarPlane = ObsoleteCaptureSettings.@default.farClipPlane;

        [SerializeField, FormerlySerializedAs("m_OverrideFieldOfView"), Obsolete("For data migration")]
        bool m_ObsoleteOverrideFieldOfView;
        [SerializeField, FormerlySerializedAs("m_FieldOfViewOverride"), Obsolete("For data migration")]
        float m_ObsoleteFieldOfViewOverride = ObsoleteCaptureSettings.@default.fieldOfView;
    }
}
