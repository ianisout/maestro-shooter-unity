using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSorting : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] AudioClip getHitBack;
    [SerializeField] [Range(0f, 5f)] float hitVolume = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 cameraPos = Camera.main.transform.position;

        float randomPitch = Random.Range(-3, 3);

        string[] arrivingNoteProjectile = other.name.Split("_");

        if (arrivingNoteProjectile.Length >=1 && other.name != "Maestro")
        {
            string noteNameToPlay = arrivingNoteProjectile[1].Split('(')[0].TrimEnd().ToLower();
            string instrumentName = this.name.Replace(" ", "-").Split('(')[0].TrimEnd().ToLower().Replace("enemy-", "");
            string nameForAudioClip = instrumentName + "-" + noteNameToPlay;
            
            for (int i = 0; i < this.audioClips.Count; i++)
            {
                if (this.audioClips[i].name != null && nameForAudioClip == this.audioClips[i].name && this.audioClips[i] != null)
                {
                    AudioSource.PlayClipAtPoint(this.audioClips[i], cameraPos, hitVolume);

                }
            }
        }

        if (other.name == "Projectile_Sharp(Clone)" || other.name == "Projectile_Flat(Clone)")
        {
            AudioSource.PlayClipAtPoint(getHitBack, cameraPos, hitVolume);
        }

    }
}
