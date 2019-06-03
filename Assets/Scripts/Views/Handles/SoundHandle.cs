using strange.extensions.mediation.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace onur.pool.views
{
    public class SoundHandle : View
    {
        [SerializeField] private AudioClip[] m_clips;
        [SerializeField] private AudioSource m_source;


        public void PlayHitSound(float volume)
        {
            m_source.PlayOneShot(m_clips[1], volume);
        }

    }
}
