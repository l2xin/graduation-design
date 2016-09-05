using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class TexturePostprocessor : AssetPostprocessor
{
    private Dictionary<string, int> mNeedARGB32Dic;
    private Dictionary<string, int> mCustomDic;

    private bool NeedARGB32Asset(string varPath)
    {
        if (string.IsNullOrEmpty(varPath) == false)
        {
            if (null == mNeedARGB32Dic)
            {
                InitNeedList();
            }
            if (null == mCustomDic)
            {
                InitCustomList();
            }
            int temp;
            if (mNeedARGB32Dic.TryGetValue(varPath, out temp))
            {
                return true;
            }
        }
        return false;
    }

    private bool NeedCustomARGB32(string varPath)
    {
        if (string.IsNullOrEmpty(varPath) == false)
        {
            //TODO;
            if (assetPath.Contains("Assets/Resources/npc"))
            {
                return true;
            }


            if (null == mCustomDic)
            {
                InitCustomList();
            }
            if (null == mNeedARGB32Dic)
            {
                InitNeedList();
            }
            int temp;
            if (mCustomDic.TryGetValue(varPath, out temp))
            {
                return true;
            }
        }
        return false;
    }

    private void InitCustomList()
    {
        if (null == mCustomDic)
        {
            mCustomDic = new Dictionary<string, int>();
        }

        mCustomDic.Add("Assets/Resources/ui/Ancient_GodHavoc/Altas/Ancient_GodHavoc.png", 0);

        mCustomDic.Add("Assets/Resources/ui/IconAtlas/IconAltas_1.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Public/PublicDynamic.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Activity/Altas/Activity2.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Altar/Atlas/Altar.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Battle/Atlas/Battle Altas.png", 0);
        mCustomDic.Add("Assets/Resources/ui/Scene_Battle/Atlas/BattleWordsAtlas.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_BattleEnd/Altlas/BattleEnd_1.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Chests/Atlas/Chests.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_BlackMarket/Atlas/BlackMarket.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Duobao/Atlas/Duobao.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Guide/Atlas/guide.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Load/Atlas/Load.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Map/Altas/Map.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Resoul/Altas/UI_JianTou.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_SeaTrial/Altas/SealTrial Altas.png", 0);
        mCustomDic.Add("Assets/Resources/ui/Scene_SeaTrial/Altas/SealTrial Altas_1.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Sell/Atlas/Sell.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Shop/Atlas/shop.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_SyetemSetting/Altas/SyetemSetting.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_TeamCombat/Atlas/TeamCombat.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_Tribe/Atlas/Tribe.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Scene_WonderFul_Activity/altlas/Wonderful Activity.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Sence_FirstRecharge/Altalas/FirstRecharge.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Sence_TribalWar/Altalas/TribalWar.png", 0);

        mCustomDic.Add("Assets/Resources/ui/Sence_WotLK/Altalas/WotLK.png", 0);

    }

    private void InitNeedList()
    {
        if (null == mNeedARGB32Dic)
        {
            mNeedARGB32Dic = new Dictionary<string, int>();
        }

        //--Other;

        mNeedARGB32Dic.Add("Assets/Resources/majorUI/login/Atlas/effect.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/Font/FightValueFont/FightValue.png", 0);
        mNeedARGB32Dic.Add("Assets/Resources/Font/ArtFont1/ArtFont1_0.png", 0);


        //----UI;
        mNeedARGB32Dic.Add("Assets/Resources/ui/Ancient_GodHavoc/Altas/Ancient_GodHavoc.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/IconAtlas/IconAltas_1.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Public/Public.png", 0);
        mNeedARGB32Dic.Add("Assets/Resources/ui/Public/PublicDynamic.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Activity/Altas/Activity2.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Altar/Atlas/Altar.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Battle/Atlas/Battle Altas.png", 0);
        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Battle/Atlas/BattleWordsAtlas.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_BattleEnd/Altlas/BattleEnd_1.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Chests/Atlas/Chests.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_BlackMarket/Atlas/BlackMarket.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Duobao/Atlas/Duobao.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Guide/Atlas/guide.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Load/Atlas/Load.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Main/Atlas/main.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Map/Altas/Map.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Pet/Altas/pet.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Resoul/Altas/UI_JianTou.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_SeaTrial/Altas/SealTrial Altas.png", 0);
        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_SeaTrial/Altas/SealTrial Altas_1.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Sell/Atlas/Sell.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Shop/Atlas/shop.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Skill/Atlas/skill.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_SyetemSetting/Altas/SyetemSetting.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_TeamCombat/Atlas/TeamCombat.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_Tribe/Atlas/Tribe.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_WonderFul_Activity/altlas/Wonderful Activity.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Scene_ZX/Altalas/ZX_altalas.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Sence_FirstRecharge/Altalas/FirstRecharge.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Sence_TribalWar/Altalas/TribalWar.png", 0);

        mNeedARGB32Dic.Add("Assets/Resources/ui/Sence_WotLK/Altalas/WotLK.png", 0);

    }


    void OnPreprocessTexture()
    {
        return;
        Debug.Log("import asset:" + assetPath);

        TextureImporter ti = (TextureImporter)assetImporter;

        if (assetPath.Contains("Assets/Resources/npc"))
        {
            ti.textureType = TextureImporterType.Advanced;
            ti.npotScale = TextureImporterNPOTScale.None;

            ti.isReadable = false;
            ti.mipmapEnabled = false;

            ti.anisoLevel = 0;

            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;

            ti.maxTextureSize = 512;

            ti.textureFormat = TextureImporterFormat.ARGB32;

            LoadTextureAsset(ti);
            return;
        }


        if (assetPath.Contains("anim"))
        {
            ti.textureType = TextureImporterType.Advanced;
            ti.npotScale = TextureImporterNPOTScale.None;

            ti.isReadable = false;
            ti.mipmapEnabled = true;

            ti.wrapMode = TextureWrapMode.Repeat;
            ti.filterMode = FilterMode.Bilinear;

            ti.textureFormat = TextureImporterFormat.AutomaticCompressed;

            if (assetPath.Contains("anim/character/base"))
            {
                int tempIndex = assetPath.LastIndexOf("/");
                if (tempIndex > 0)
                {
                    string tempName = assetPath.Substring(tempIndex + 1);

                    if (tempName.StartsWith("5") || tempName.StartsWith("999"))
                    {
                        /// 可破坏物和怪物size 256
                        ti.maxTextureSize = 256;
                    }
                    else
                    {
                        /// npc，宠物，怪物
                        ti.maxTextureSize = 512;
                    }

                }

            }

            if (assetPath.Contains("anim/scene object/yun") == true)
            {
                ti.maxTextureSize = 512;
            }
            else
            {
                ti.maxTextureSize = 256;
            }

            LoadTextureAsset(ti);
            return;

        }

        if (assetPath.Contains("Effect") || assetPath.Contains("model/drop item"))
        {
            ti.textureType = TextureImporterType.Advanced;
            ti.npotScale = TextureImporterNPOTScale.None;

            ti.isReadable = false;
            ti.mipmapEnabled = true;

            ti.wrapMode = TextureWrapMode.Repeat;
            ti.filterMode = FilterMode.Bilinear;

            ti.maxTextureSize = 256;

            if (assetPath.Contains("Texture"))
            {
                ti.textureFormat = TextureImporterFormat.AutomaticCompressed;
            }

            LoadTextureAsset(ti);
            return;
        }

        if (assetPath.Contains("model/SceneBase"))
        {
            ti.textureType = TextureImporterType.Image;
            ti.wrapMode = TextureWrapMode.Repeat;
            ti.filterMode = FilterMode.Bilinear;
            ti.maxTextureSize = 2048;
            ti.textureFormat = TextureImporterFormat.AutomaticCompressed;

            LoadTextureAsset(ti);
            return;
        }

        if (assetPath.Contains("/Scene/"))
        {
            int tempIndex = assetPath.LastIndexOf("/");
            if (tempIndex > 0)
            {
                string tempName = assetPath.Substring(tempIndex + 1);
                tempName = tempName.ToLower();
                if (tempName.StartsWith("lightmap"))
                {
                    ti.maxTextureSize = 512;
                    ti.textureFormat = TextureImporterFormat.AutomaticCompressed;
                }
            }

            LoadTextureAsset(ti);
            return;
        }

        if (assetPath.Contains("/ui/") || assetPath.Contains("/Font/"))
        {

            ti.npotScale = TextureImporterNPOTScale.None;
            ti.generateCubemap = TextureImporterGenerateCubemap.None;
            ti.isReadable = true;
            ti.normalmap = false;
            ti.lightmap = false;
            ti.mipmapEnabled = false;
            ti.mipmapEnabled = false;


            ti.filterMode = FilterMode.Bilinear;
            ti.anisoLevel = 0;


            if (assetPath.ToLower().Contains(".jpg") == true)
            {
                ti.textureFormat = TextureImporterFormat.AutomaticCompressed;

                ti.maxTextureSize = 2048;

                LoadTextureAsset(ti);

                return;
            }



            bool tempFrag = NeedARGB32Asset(assetPath);

            if (tempFrag == true)
            {
                ti.textureFormat = TextureImporterFormat.ARGB32;
            }
            else
            {
                ti.textureFormat = TextureImporterFormat.ARGB16;
            }


            ti.maxTextureSize = 2048;

            LoadTextureAsset(ti);

            return;
        }

        if (assetPath.Contains("/majorUI/login/Atlas/"))
        {
            ti.textureType = TextureImporterType.Advanced;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.generateCubemap = TextureImporterGenerateCubemap.None;
            ti.isReadable = true;
            ti.normalmap = false;
            ti.lightmap = false;
            ti.mipmapEnabled = false;
            ti.mipmapEnabled = false;


            ti.filterMode = FilterMode.Bilinear;
            ti.anisoLevel = 0;


            if (assetPath.ToLower().Contains(".jpg") == true)
            {
                ti.textureFormat = TextureImporterFormat.AutomaticCompressed;

                ti.maxTextureSize = 1024;

                LoadTextureAsset(ti);

                return;
            }

            ti.textureFormat = TextureImporterFormat.ARGB32;

            ti.maxTextureSize = 1024;

            LoadTextureAsset(ti);

            return;
        }


        if (assetPath.Contains("Role"))
        {
            //ti.textureType = TextureImporterType.Advanced;
            //ti.npotScale = TextureImporterNPOTScale.None;
            //ti.generateCubemap = TextureImporterGenerateCubemap.None;
            //ti.isReadable = true;
            //ti.normalmap = false;
            //ti.lightmap = false;
            //ti.mipmapEnabled = false;
            //ti.mipmapEnabled = false;


            //ti.filterMode = FilterMode.Bilinear;
            //ti.anisoLevel = 0;


            //if (assetPath.ToLower().Contains(".jpg") == true)
            //{
            //    ti.textureFormat = TextureImporterFormat.AutomaticCompressed;

            //    ti.maxTextureSize = 2048;

            //    LoadTextureAsset(ti);

            //    return;
            //}

            //ti.textureFormat = TextureImporterFormat.AutomaticCompressed;

            //ti.maxTextureSize = 2048;

            //LoadTextureAsset(ti);

            return;
        }

        Debug.Log("import texture");
        ti.isReadable = false;
        ti.maxTextureSize = 2048;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;

        LoadTextureAsset(ti);
    }

    private void LoadTextureAsset(TextureImporter ti)
    {
        Texture textureAsset = (Texture)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture));

        if (textureAsset == null)
        {
            return;
        }

        if (textureAsset.height >= 512 || textureAsset.width >= 512)
        {
            Debug.LogError("!!!texture size is much large:" + assetPath);
        }

        if ((ti.textureFormat != TextureImporterFormat.AutomaticCompressed) || (ti.compressionQuality != (int)TextureCompressionQuality.Best))
        {
            ti.compressionQuality = (int)TextureCompressionQuality.Best;
        }
    }


    void OnPostprocessTexture(Texture2D texture)
    {
        bool tempFrag = NeedCustomARGB32(assetPath);

        if (tempFrag == true)
        {
            Custom(texture);
        }

    }


    private void Custom(Texture2D texture)
    {
        var texw = texture.width;
        var texh = texture.height;

        var pixels = texture.GetPixels();
        var offs = 0;

        var k1Per15 = 1.0f / 15.0f;
        var k1Per16 = 1.0f / 16.0f;
        var k3Per16 = 3.0f / 16.0f;
        var k5Per16 = 5.0f / 16.0f;
        var k7Per16 = 7.0f / 16.0f;

        for (var y = 0; y < texh; y++)
        {
            for (var x = 0; x < texw; x++)
            {
                float a = pixels[offs].a;
                float r = pixels[offs].r;
                float g = pixels[offs].g;
                float b = pixels[offs].b;

                var a2 = Mathf.Clamp01(Mathf.Floor(a * 16) * k1Per15);
                var r2 = Mathf.Clamp01(Mathf.Floor(r * 16) * k1Per15);
                var g2 = Mathf.Clamp01(Mathf.Floor(g * 16) * k1Per15);
                var b2 = Mathf.Clamp01(Mathf.Floor(b * 16) * k1Per15);

                var ae = a - a2;
                var re = r - r2;
                var ge = g - g2;
                var be = b - b2;

                pixels[offs].a = a2;
                pixels[offs].r = r2;
                pixels[offs].g = g2;
                pixels[offs].b = b2;

                var n1 = offs + 1;
                var n2 = offs + texw - 1;
                var n3 = offs + texw;
                var n4 = offs + texw + 1;

                if (x < texw - 1)
                {
                    pixels[n1].a += ae * k7Per16;
                    pixels[n1].r += re * k7Per16;
                    pixels[n1].g += ge * k7Per16;
                    pixels[n1].b += be * k7Per16;
                }

                if (y < texh - 1)
                {
                    pixels[n3].a += ae * k5Per16;
                    pixels[n3].r += re * k5Per16;
                    pixels[n3].g += ge * k5Per16;
                    pixels[n3].b += be * k5Per16;

                    if (x > 0)
                    {
                        pixels[n2].a += ae * k3Per16;
                        pixels[n2].r += re * k3Per16;
                        pixels[n2].g += ge * k3Per16;
                        pixels[n2].b += be * k3Per16;
                    }

                    if (x < texw - 1)
                    {
                        pixels[n4].a += ae * k1Per16;
                        pixels[n4].r += re * k1Per16;
                        pixels[n4].g += ge * k1Per16;
                        pixels[n4].b += be * k1Per16;
                    }
                }

                offs++;
            }
        }

        texture.SetPixels(pixels);
        EditorUtility.CompressTexture(texture, TextureFormat.RGBA4444, TextureCompressionQuality.Best);

    }

}
