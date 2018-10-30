using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FogProjector : MonoBehaviour
{
    public RenderTexture fogTexture;
    RenderTexture projecTexture;

    RenderTexture oldTexture;

    public Shader blurShader;

    [Range(1, 4)]
    public int upsample = 2;

    Material blurMaterial;
    public float blur=1;

    Projector projector;

    public float blendSpeed = 1;
    float blend;
    int blendNameId;

    void OnEnable()
    {
        projector = GetComponent<Projector>();

        blurMaterial = new Material(blurShader);
        blurMaterial.SetVector("_Parameter", new Vector4(blur, -blur, 0, 0));

        projecTexture = new RenderTexture(
                            fogTexture.width * upsample,
                            fogTexture.height * upsample,
                            0,
                            fogTexture.format) {filterMode = FilterMode.Bilinear};

        oldTexture = new RenderTexture(
                         fogTexture.width * upsample,
                         fogTexture.height * upsample,
                         0,
                         fogTexture.format) {filterMode = FilterMode.Bilinear};

        projector.material.SetTexture("_FogTex", projecTexture);
        projector.material.SetTexture("_OldFogTex", oldTexture);
        blendNameId = Shader.PropertyToID("_Blend");
        blend = 1;
        projector.material.SetFloat(blendNameId, blend);
        Graphics.Blit(fogTexture, projecTexture);
        UpdateFog();
    }

    public void UpdateFog()
    {
        Graphics.Blit(projecTexture, oldTexture);
        Graphics.Blit(fogTexture, projecTexture);

        RenderTexture temp = RenderTexture.GetTemporary(
            projecTexture.width,
            projecTexture.height,
            0,
            projecTexture.format);

        temp.filterMode = FilterMode.Bilinear;

        Graphics.Blit(projecTexture, temp, blurMaterial, 1);
        Graphics.Blit(temp, projecTexture, blurMaterial, 2);

        StartCoroutine(Blend());

        RenderTexture.ReleaseTemporary(temp);
    }

    IEnumerator Blend()
    {
        blend = 0;
        projector.material.SetFloat(blendNameId, blend);
        while (blend < 1)
        {
            blend = Mathf.MoveTowards(blend, 1, blendSpeed * Time.deltaTime);
            projector.material.SetFloat(blendNameId, blend);
            yield return null;
        }
    }
}