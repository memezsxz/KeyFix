﻿using UnityEngine;

public class CharacterSkinController : MonoBehaviour
{
    public enum EyePosition
    {
        normal,
        happy,
        angry,
        dead
    }

    public Texture2D[] albedoList;

    [ColorUsage(true, true)] public Color[] eyeColors;

    public EyePosition eyeState;
    private Animator animator;
    private Renderer[] characterMaterials;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterMaterials = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //ChangeMaterialSettings(0);
            ChangeEyeOffset(EyePosition.normal);
            ChangeAnimatorIdle("normal");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //ChangeMaterialSettings(1);
            ChangeEyeOffset(EyePosition.angry);
            ChangeAnimatorIdle("angry");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //ChangeMaterialSettings(2);
            ChangeEyeOffset(EyePosition.happy);
            ChangeAnimatorIdle("happy");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //ChangeMaterialSettings(3);
            ChangeEyeOffset(EyePosition.dead);
            ChangeAnimatorIdle("dead");
        }
    }

    private void ChangeAnimatorIdle(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    private void ChangeMaterialSettings(int index)
    {
        for (var i = 0; i < characterMaterials.Length; i++)
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetColor("_EmissionColor", eyeColors[index]);
            else
                characterMaterials[i].material.SetTexture("_MainTex", albedoList[index]);
    }

    private void ChangeEyeOffset(EyePosition pos)
    {
        var offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.normal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.dead:
                offset = new Vector2(.33f, .66f);
                break;
        }

        for (var i = 0; i < characterMaterials.Length; i++)
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetTextureOffset("_MainTex", offset);
    }
}