﻿#if UNITY_2017_1_OR_NEWER

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UTJ.Alembic
{
    public class AlembicShotAsset : PlayableAsset, ITimelineClipAsset
    {
        AlembicStreamPlayer m_Stream;

        [Tooltip("Alambic asset to play")]
        public ExposedReference<AlembicStreamPlayer> m_StreamPlayer;

        [Tooltip("Amount of time to clip off the start of the alembic asset from playback.")]
        [SerializeField] public float m_StartOffset;

        [Tooltip("Amount of time to clip off the end of the alembic asset from playback.")]
        [SerializeField] public float m_EndOffset;

        [Tooltip("Use to compress/dilute time play back.")]
        [SerializeField] public float m_TimeScale = 1f;

		[Tooltip("Controls how playback cycles throught the stream.")]
		[SerializeField] public AlembicPlaybackSettings.CycleType m_Cycle = AlembicPlaybackSettings.CycleType.Hold;

        [Tooltip("Portion, in seconds, of the alembic stream used by the shot.")]
        [ReadOnly] public float m_AlembicLength = 0;

        public ClipCaps clipCaps { get { return ClipCaps.None;  } }

        public override PlayableHandle CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var handle = graph.CreateScriptPlayable<AlembicShotPlayable>();

            var playable = handle.GetObject<AlembicShotPlayable>();
            m_Stream = m_StreamPlayer.Resolve(graph.resolver);
            playable.streamPlayer = m_Stream;
            playable.m_StartTimeOffset = m_StartOffset;
            playable.m_EndTimeClipOff = m_EndOffset;
            playable.m_TimeScale = m_TimeScale;
            playable.m_Cycle = m_Cycle;
            return handle;
        }

        public override double duration
        {
            get
            {
                var t = m_Stream == null ? 0 : m_Stream.m_PlaybackSettings.m_duration;
                m_AlembicLength = t;
                if (m_Cycle == AlembicPlaybackSettings.CycleType.Hold || m_Cycle == AlembicPlaybackSettings.CycleType.Reverse)
                    return (t - m_StartOffset - m_EndOffset) * m_TimeScale;
                else
                {
                    return base.duration;
                }
            }
        }

    }
}

#endif