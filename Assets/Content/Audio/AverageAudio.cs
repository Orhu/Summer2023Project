using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class AverageAudio : MonoBehaviour
    {

        public List<Projectile> projectiles = new List<Projectile>();

        public AudioClip averageAudioClip;


        void Start()
        {

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = averageAudioClip;
            audioSource.loop = true;
            //audioSource.playOnAwake = true; 
            audioSource.spatialBlend = 1f;
            audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
            audioSource.Play();

        }

        public Vector2 TryGetAveragePos()
        {
            float totalX = 0;
            float totalY = 0;

            foreach (var projectile in projectiles)
            {
                if (projectile != null)
                {
                    totalX += projectile.transform.position.x;
                    totalY += projectile.transform.position.y;
                }
            }

            if (totalX == 0 && totalY == 0)
            {
                AudioManager.KillAverageAudio(this);
            }

            return new Vector2(totalX / projectiles.Count, totalY / projectiles.Count);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            transform.position = TryGetAveragePos();

        }

        public void SetProjectiles(List<Projectile> setProjectiles)
        {
            this.projectiles = setProjectiles;
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            this.averageAudioClip = audioClip;
        }
    }
    
}