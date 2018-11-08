using System;
using UnityEngine.Serialization;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public abstract partial class HDProbe : IVersionable<HDProbe.Version>
    {
        protected enum Version
        {
            Initial,
            ProbeSettings
        }

        protected static readonly MigrationDescription<Version, HDProbe> k_Migration = MigrationDescription.New(
            MigrationStep.New(Version.ProbeSettings, (HDProbe p) =>
            {
#pragma warning disable 618
                p.m_ProbeSettings.proxySettings.useInfluenceVolumeAsProxyVolume = !p.m_ObsoleteInfiniteProjection;
                p.m_ProbeSettings.influence = p.m_ObsoleteInfluenceVolume;
                p.m_ProbeSettings.camera.frameSettings = p.m_ObsoleteFrameSettings;
                p.m_ProbeSettings.lighting.multiplier = p.m_ObsoleteMultiplier;
                p.m_ProbeSettings.lighting.weight = p.m_ObsoleteWeight;
                p.m_ProbeSettings.lighting.lightLayer = p.m_ObsoleteLightLayers;
                p.m_ProbeSettings.mode = p.m_ObsoleteMode;

                // Migrating Capture Settings
                p.m_ProbeSettings.camera.bufferClearing.clearColorMode = p.m_ObsoleteCaptureSettings.clearColorMode;
                p.m_ProbeSettings.camera.bufferClearing.backgroundColorHDR = p.m_ObsoleteCaptureSettings.backgroundColorHDR;
                p.m_ProbeSettings.camera.bufferClearing.clearDepth = p.m_ObsoleteCaptureSettings.clearDepth;
                p.m_ProbeSettings.camera.culling.cullingMask = p.m_ObsoleteCaptureSettings.cullingMask;
                p.m_ProbeSettings.camera.culling.useOcclusionCulling = p.m_ObsoleteCaptureSettings.useOcclusionCulling;
                p.m_ProbeSettings.camera.frustum.nearClipPlane = p.m_ObsoleteCaptureSettings.nearClipPlane;
                p.m_ProbeSettings.camera.frustum.farClipPlane = p.m_ObsoleteCaptureSettings.farClipPlane;
                p.m_ProbeSettings.camera.volumes.layerMask = p.m_ObsoleteCaptureSettings.volumeLayerMask;
                p.m_ProbeSettings.camera.volumes.anchorOverride = p.m_ObsoleteCaptureSettings.volumeAnchorOverride;
                p.m_ProbeSettings.camera.frustum.fieldOfView = p.m_ObsoleteCaptureSettings.fieldOfView;
                p.m_ProbeSettings.camera.renderingPath = p.m_ObsoleteCaptureSettings.renderingPath;
#pragma warning restore 618
            })
        );

        [SerializeField]
        Version m_HDProbeVersion;
        Version IVersionable<Version>.version { get => m_HDProbeVersion; set => m_HDProbeVersion = value; }

        // Legacy fields for HDProbe
        [SerializeField, FormerlySerializedAs("m_InfiniteProjection"), Obsolete("For Data Migration")]
        protected bool m_ObsoleteInfiniteProjection = true;

        [SerializeField, FormerlySerializedAs("m_InfluenceVolume"), Obsolete("For Data Migration")]
        protected InfluenceVolume m_ObsoleteInfluenceVolume;

        [SerializeField, FormerlySerializedAs("m_FrameSettings"), Obsolete("For Data Migration")]
        FrameSettings m_ObsoleteFrameSettings = null;

        [SerializeField, FormerlySerializedAs("m_Multiplier"), FormerlySerializedAs("dimmer")]
        [FormerlySerializedAs("m_Dimmer"), FormerlySerializedAs("multiplier"), Obsolete("For Data Migration")]
        protected float m_ObsoleteMultiplier = 1.0f;
        [SerializeField, FormerlySerializedAs("m_Weight"), FormerlySerializedAs("weight")]
        [Obsolete("For Data Migration")]
        [Range(0.0f, 1.0f)]
        protected float m_ObsoleteWeight = 1.0f;

        [SerializeField, FormerlySerializedAs("m_Mode"), Obsolete("For Data Migration")]
        protected ProbeSettings.Mode m_ObsoleteMode = ProbeSettings.Mode.Baked;

        [SerializeField, FormerlySerializedAs("lightLayer"), Obsolete("For Data Migration")]
        LightLayerEnum m_ObsoleteLightLayers = LightLayerEnum.LightLayerDefault;

        [SerializeField, FormerlySerializedAs("m_CaptureSettings"), Obsolete("For Data Migration")]
        protected ObsoleteCaptureSettings m_ObsoleteCaptureSettings;
    }
}
