using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//サウンドの制御
public class SoundManageScript : MonoBehaviour
{
    public AudioClip getItem_mp3; //アイテム獲得時のSE

    AudioSource audioSource;

    [SerializeField] ItemManageScript itemMana_sc;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        //アイテム獲得時に鳴らす
        if (itemMana_sc.itemSound_shot)
        {
            audioSource.PlayOneShot(getItem_mp3);
            itemMana_sc.itemSound_shot = false;
        }
    }
}
