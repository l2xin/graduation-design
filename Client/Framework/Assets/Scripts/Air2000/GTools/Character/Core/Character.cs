/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Character.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 12:47:33
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace GTools.Character
{
    public class Character : MonoBehaviour
    {
        #region [Fields]
        [HideInInspector]
        public MotionMachine MotionMachine;
        [HideInInspector]
        public MotionCommander MotionCommander;
        [HideInInspector]
        public MotionCrossfader MotionCrossfader;
        [HideInInspector]
        public Vector3 pTargetPos;
        [HideInInspector]
        public Quaternion pTargetRotation;
        [HideInInspector]
        public float Radius = 0.5f;
        [HideInInspector]
        public float Height = 2f;
        [HideInInspector]
        public Character Enemy;
        [NonSerialized]
        public Animation Animation;
        [NonSerialized]
        public Renderer[] Renderers;
        #endregion

        #region [Functions]

        #region monobehaviour functions
        void Awake()
        {
            if (AllCharacters == null) AllCharacters = new List<Character>();
            AllCharacters.Add(this);
            GetAnimation();
            GetMotionMachine();
            GetMotionCommander();
            GetMotionCrossfader();
            OnChAwake();
        }
        void Start() { OnChStart(); }
        void OnEnable()
        {
            //if (GetAnimation() != null)
            //{
            //    Animation.cullingType = AnimationCullingType.AlwaysAnimate;
            //}
            OnChEnable();
        }
        void OnDisable() { OnChDisable(); }
        void Update() { OnChUpdate(); }
        void LateUpdate() { OnChLateUpdate(); }
        void OnDestroy()
        {
            if (AllCharacters != null)
            {
                AllCharacters.Remove(this);
            }
            OnChDestroy();
        }
        #endregion

        #region character behaviour
        public virtual void OnChAwake() { }
        public virtual void OnChStart() { }
        public virtual void OnChEnable() { }
        public virtual void OnChDisable() { }
        public virtual void OnChUpdate() { }
        public virtual void OnChLateUpdate() { }
        public virtual void OnChDestroy() { }
        public bool ExecuteCommand(CharacterCommand command)
        {
            if (MotionCommander == null) return false;
            return MotionCommander.ExecuteCommand(command);
        }
        #endregion

        #region set & get
        public Animation GetAnimation()
        {
            if (Animation == null)
            {
                Animation = GetComponent<Animation>();
            }
            if (Animation == null)
            {
                Animation[] animations = GetComponentsInChildren<Animation>(true);
                if (animations != null && animations.Length > 0)
                {
                    Animation = animations[0];
                }
                if (Animation != null)
                {
                    Animation.playAutomatically = false;
                    Animation.cullingType = AnimationCullingType.AlwaysAnimate;
                }
            }
            return Animation;
        }
        public MotionMachine GetMotionMachine()
        {
            if (MotionMachine == null)
            {
                MotionMachine[] machines = GetComponentsInChildren<MotionMachine>(true);
                if (machines != null && machines.Length > 0)
                {
                    MotionMachine = machines[0];
                }
            }
            return MotionMachine;
        }
        public MotionCommander GetMotionCommander()
        {
            if (MotionCommander == null)
            {
                MotionCommander[] commanders = GetComponentsInChildren<MotionCommander>(true);
                if (commanders != null && commanders.Length > 0)
                {
                    MotionCommander = commanders[0];
                }
            }
            return MotionCommander;
        }
        public MotionCrossfader GetMotionCrossfader()
        {
            if (MotionCrossfader == null)
            {
                MotionCrossfader[] crossfaders = GetComponentsInChildren<MotionCrossfader>(true);
                if (crossfaders != null && crossfaders.Length > 0)
                {
                    MotionCrossfader = crossfaders[0];
                }
            }
            return MotionCrossfader;
        }
        public Transform GetBodyTransform()
        {
            if (Animation == null)
            {
                Animation = GetAnimation();
            }
            if (Animation == null) return null;
            return Animation.transform;
        }
        public virtual void SetToonColor(Color varColor)
        {
            Renderers = GetRenderers();
            if (Renderers == null || Renderers.Length == 0)
            {
                return;
            }
            for (int i = 0; i < Renderers.Length; i++)
            {
                Renderer render = Renderers[i];
                if (render == null)
                {
                    continue;
                }
                Material mat = render.sharedMaterial;
                if (mat == null)
                {
                    return;
                }
                if (mat.HasProperty("_ToonColor"))
                {
                    mat.SetColor("_ToonColor", varColor);
                }
            }
        }
        public virtual void SetToonWidth(float width)
        {
            Renderer[] renderers = GetRenderers();
            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    Renderer renderer = renderers[i];
                    if (renderer == null)
                    {
                        continue;
                    }
                    Material[] mats = renderer.sharedMaterials;
                    if (mats != null && mats.Length > 0)
                    {
                        for (int j = 0; j < mats.Length; j++)
                        {
                            Material mat = mats[j];
                            if (mat == null)
                            {
                                continue;
                            }
                            Shader shader = mat.shader;
                            if (shader == null)
                            {
                                continue;
                            }
                            if (IsShaderIllegal(shader))
                            {
                                continue;
                            }
                            mat.SetFloat("_Outline", width);
                        }
                    }

                }
            }
        }
        private bool IsShaderIllegal(Shader shader)
        {
            //if (shader == null)
            //{
            //    return true;
            //}
            //if (shader.name.StartsWith("Air2000/ToonShading/NormalOutline"))
            //{
            //    return false;
            //}
            return true;
        }
        public virtual void SetMaterial(string varRenderRootPath, Material varMat)
        {
            if (varMat == null)
            {
                return;
            }
            Transform body = GetBodyTransform();
            if (body == null)
            {
                return;
            }
            Transform renderTran = body.Find(varRenderRootPath);
            if (renderTran == null)
            {
                return;
            }
            SetMaterial(renderTran, varMat);
        }
        public virtual void SetMaterial(Transform varRenderTran, Material varMat)
        {
            if (varRenderTran == null)
            {
                return;
            }
            if (varMat == null)
            {
                return;
            }
            Renderer render = varRenderTran.GetComponent<Renderer>();
            if (render == null)
            {
                return;
            }
            render.sharedMaterial = varMat;
        }
        public virtual void SetTexture(Material varMat, string varItemName, Texture varTexture)
        {
            if (varMat == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(varItemName))
            {
                return;
            }
            if (varTexture == null)
            {
                return;
            }
            varMat.SetTexture(varItemName, varTexture);
        }
        public virtual void SetChParent(Transform parent)
        {
            if (parent == null)
            {
                return;
            }
            if (transform != null)
            {
                transform.SetParent(parent);
            }
        }
        public virtual void SetChActive(bool state)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(state);
            }
        }
        public virtual void SetBodyActive(bool state)
        {
            Transform bodyTran = GetBodyTransform();
            if (bodyTran != null && bodyTran.gameObject != null)
            {
                bodyTran.gameObject.SetActive(state);
            }
        }
        public virtual Renderer[] GetRenderers()
        {
            if (Renderers == null || Renderers.Length == 0)
            {
                Renderers = transform.GetComponentsInChildren<Renderer>(true);
            }
            return Renderers;
        }
        public virtual Transform GetRendererTransform(string rootpath)
        {
            if (string.IsNullOrEmpty(rootpath))
            {
                return null;
            }
            Transform bodyTran = GetBodyTransform();
            if (bodyTran == null)
            {
                return null;
            }
            return bodyTran.Find(rootpath);
        }
        public virtual Renderer GetRendererByRootpath(string rootpath)
        {
            Transform rendererTrans = GetRendererTransform(rootpath);
            if (rendererTrans == null) return null;
            return rendererTrans.GetComponent<Renderer>();
        }
        public virtual void SetRenderPower(float value)
        {
            Renderers = GetRenderers();
            if (Renderers != null && Renderers.Length > 0)
            {
                for (int i = 0; i < Renderers.Length; i++)
                {
                    Renderer renderer = Renderers[i];
                    if (renderer == null)
                    {
                        continue;
                    }
                    SetRenderPower(renderer, value);
                }
            }
        }
        public virtual void SetRenderPower(Renderer varRender, float value)
        {
            if (varRender == null)
            {
                return;
            }
            Material mat = varRender.sharedMaterial;
            if (mat == null)
            {
                return;
            }
            if (mat.HasProperty("_Strength"))
            {
                mat.SetFloat("_Strength", value);
            }
        }
        public virtual Transform GetTransformByRootPath(string rootpath)
        {
            if (string.IsNullOrEmpty(rootpath))
            {
                return GetBodyTransform();
            }
            Transform bodyTran = GetBodyTransform();
            if (bodyTran == null) return null;
            return bodyTran.Find(rootpath);
        }
        public virtual void SetPosition(Vector3 position)
        {
            if (transform != null)
            {
                transform.position = position;
            }
        }
        public virtual void SetLocalPosition(Vector3 position)
        {
            if (transform != null)
            {
                transform.localPosition = position;
            }
        }
        public virtual void SetBodyPosition(Vector3 position)
        {
            Transform bodyTran = GetBodyTransform();
            if (bodyTran != null && bodyTran.gameObject != null)
            {
                bodyTran.position = position;
            }
        }
        public virtual void SetBodyLocalPosition(Vector3 position)
        {
            Transform bodyTran = GetBodyTransform();
            if (bodyTran != null && bodyTran.gameObject != null)
            {
                bodyTran.localPosition = position;
            }
        }
        public virtual void SetRotation(Quaternion rotation)
        {
            if (transform != null)
            {
                transform.rotation = rotation;
            }
        }
        public virtual void SetLocalRotation(Quaternion rotation)
        {
            if (transform != null)
            {
                transform.localRotation = rotation;
            }
        }
        public virtual void SetBodyTilt(Quaternion rotation)
        {
            SetBodyRotation(rotation);
        }
        public virtual void SetBodyRotation(Quaternion rotation)
        {
            Transform bodyTran = GetBodyTransform();
            if (bodyTran != null && bodyTran.gameObject != null)
            {
                bodyTran.localRotation = rotation;
            }
        }
        public virtual void SetLocalScale(Vector3 scale)
        {
            if (transform != null)
            {
                transform.localScale = scale;
            }
        }
        public virtual void SetBodyScale(Vector3 scale)
        {
            Transform bodyTran = GetBodyTransform();
            if (bodyTran != null && bodyTran.gameObject != null)
            {
                bodyTran.localScale = scale;
            }
        }
        public virtual void TurnToTargetDirection()
        {
            TurnToTargetDirection(pTargetPos);
        }
        public virtual void TurnToTargetDirection(Vector3 target)
        {
            Vector3 dir = target - transform.position;
            dir.Normalize();
            Quaternion rotation = Quaternion.LookRotation(dir);
            SetRotation(Quaternion.Euler(0f, rotation.eulerAngles.y, 0f));
        }
        #endregion

        #region listenr of iTween
        public void OnTweenEnd(object obj)
        {
            if (obj != null)
            {
                MethodInfo info = obj.GetType().GetMethod("OnTweenEnd");
                if (info != null)
                {
                    info.Invoke(obj, new object[] { });
                }
            }
        }
        #endregion

        #region attack and beattack function
        public void TriggerAttack(Character enemy)
        {
            Enemy = enemy;
            if (Enemy == null)
            {
                return;
            }
            Vector3 dir = Enemy.transform.position - transform.position;
            dir.Normalize();
            Quaternion rotation = Quaternion.LookRotation(dir);
            SetRotation(rotation);
            SetBodyRotation(Quaternion.identity);

            Enemy.transform.forward = -transform.forward;
            Enemy.SetBodyTilt(Quaternion.identity);

            ExecuteCommand(CharacterCommand.CC_Attack);
        }
        public void Attack(Character beAttacker = null)
        {
            if (beAttacker == null)
            {
                beAttacker = Enemy;
            }
            if (beAttacker != null)
            {
                beAttacker.ExecuteCommand(CharacterCommand.CC_BeAttack);
            }
        }
        public void BeAttack(Character attacker) { }
        #endregion

        #endregion

        #region [Static]
        public static List<Character> AllCharacters = new List<Character>();
        public static Character GetCharacter() { return null; }
        public static List<Character> GetAllCharacter()
        {
            List<Character> Chs = new List<Character>();
            if (AllCharacters == null || AllCharacters.Count == 0) return null;
            for (int i = 0; i < AllCharacters.Count; i++)
            {
                Character ch = AllCharacters[i];
                if (ch == null || ch.gameObject.activeSelf == false) continue;
                Chs.Add(ch);
            }
            return Chs;
        }
        public static void Pool(Character character) { }
        #endregion
    }
}
