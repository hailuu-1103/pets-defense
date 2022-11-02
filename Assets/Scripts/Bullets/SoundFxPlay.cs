using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFxPlay : MonoBehaviour
{

    AudioSource fireEffect;

    // Start is called before the first frame update
    void Start()
    {
        fireEffect = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
