using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー中心で鳴る音の制御
public class PlayerSoundScript : MonoBehaviour
{
    //public AudioClip fly_mp3; //飛行時のSE
    //public AudioClip bgm_mp3; //飛行時のBGM

    AudioSource[] audioSource;

    [SerializeField] PlayerScript player_sc;

    void Start()
    {
        audioSource = gameObject.GetComponents<AudioSource>();

        //飛行音とBGMは常に鳴らす
        //audioSource.PlayOneShot(fly_mp3, 0.75f);
        //audioSource.PlayOneShot(bgm_mp3, 1.0f);

        audioSource[0].Play();
        audioSource[1].Play();

        audioSource[0].loop = true;
        audioSource[1].loop = true;

    }

    void Update()
    {

        //ブレーキ時にミュート
        if (player_sc.brake_on)
        {
            audioSource[1].mute = true;
        }
        else
        {
            audioSource[1].mute = false;
        }
        
    }
}
