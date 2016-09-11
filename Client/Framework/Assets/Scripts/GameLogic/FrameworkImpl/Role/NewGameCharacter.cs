/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NewGameCharacter.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/5 10:42:29
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;
using UnityEngine;
using Air2000.Animator;
using Air2000.Character;

namespace GameLogic
{
    public class NewGameCharacter : Character
    {
        #region [Fields]
        public Player pPlayer;
        public Profession EnumProfession;

        [HideInInspector]
        public Air2000.Character.EffectRoot FootShadow;
        [HideInInspector]
        public Air2000.Character.EffectRoot SelfFlow;
        [HideInInspector]
        public Air2000.Character.EffectRoot Overlap;
        [HideInInspector]
        public GTAnimator Animator;
        [NonSerialized]
        public GameObject m_RealTransform;
        public Transform RealTransform
        {
            get
            {
                if (m_RealTransform == null)
                {
                    m_RealTransform = new GameObject("RealTransform");
                    m_RealTransform.transform.SetParent(transform);
                }
                Vector3 eulerAngles = new Vector3();
                Transform bodyTran = GetBodyTransform();
                if (bodyTran != null)
                {
                    eulerAngles.x = bodyTran.localEulerAngles.x;
                    eulerAngles.z = bodyTran.localEulerAngles.z;
                }
                eulerAngles.y = transform.eulerAngles.y;
                m_RealTransform.transform.eulerAngles = eulerAngles;
                m_RealTransform.transform.localPosition = Vector3.zero;
                return m_RealTransform.transform;
            }
        }
        #region stencil test
        [NonSerialized]
        public Shader StencilShader;
        // [NonSerialized]
        public Shader OriginalFaceShader;
        //   [NonSerialized]
        public Shader OriginalBodyShader;
        private bool OpenStencilTest = false;
        #endregion
        #endregion

        #region [Functions]
        public void Init(Color color)
        {
            GetAnimator();
            if (MotionCommander == null) GetMotionCommander();
            if (MotionCommander != null)
            {
                MotionCommander.OnChangeCommandDelegate += OnChangeCommand;
                MotionCommander.OnDisableDelegate += OnCommanderDisable;
            }
        }
        public GTAnimator GetAnimator()
        {
            if (Animator == null)
            {
                GTAnimator[] animators = GetComponentsInChildren<GTAnimator>(true);
                if (animators != null && animators.Length > 0)
                {
                    Animator = animators[0];
                }
            }
            return Animator;
        }
        public void AddMaterial(Color color) { }
        public void RemoveAllAdditiveMaterial() { }

        #region define attack function
        private void OnCommanderDisable(MotionCommander commander)
        {
            if (commander != null)
            {
                //为了防止角色缓存导致的角色隐藏的错误，这里移除监听
                //理论上，当角色被隐藏/重现 后，依旧可能出现问题...但是概率比较小
                commander.OnChangeCommandDelegate -= OnChangeCommand;
            }
        }
        private void OnChangeCommand(MotionCommander.Command command, MotionCommander.Command lastCommand)
        {
            if (command != null && command.Type == CharacterCommand.CC_Attack)
            {

            }
            if (lastCommand != null && lastCommand.Type == CharacterCommand.CC_BeAttack)
            {
                Disappear();//when beattack motion finish,set character to invisible
            }
        }
        #endregion
        public void Appear(Vector3 position, CharacterCommand command = CharacterCommand.CC_Appear)
        {
            Active(true, GameScene.LAYER.Player);
            SetPosition(position);
            ExecuteCommand(command);
        }
        public void Active(bool state, GameScene.LAYER layer = GameScene.LAYER.Player)
        {
            SetChActive(state);
            SetLocalScale(Vector3.one);
            SetBodyRotation(Quaternion.identity);
            SetRotation(Quaternion.identity);
            SetBodyLocalPosition(Vector3.zero);
            SetPosition(new Vector3(-1000, -1000, -1000));
            if (state)
            {
                SetLayer(layer);
            }
        }
        public void Disappear()
        {
            Active(false);
        }
        public void Recover()
        {
        }
        public void CleanUp()
        {
         
            SetBodyScale(new Vector3(0.13f, 0.13f, 0.13f));
            SetLocalScale(Vector3.one);
            SetBodyTilt(Quaternion.identity);
            SetBodyLocalPosition(Vector3.zero);
            if (Animator != null)
            {
                Animator.Stop(true);
            }
        }
        public void SetLayer(GameScene.LAYER layer)
        {
            if (gameObject != null)
            {
                Helper.SetLayer(gameObject, layer);
            }
        }
        public void EnableStencilTest()
        {
            if (OpenStencilTest) return;
            OpenStencilTest = true;

            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            if (renderers != null && renderers.Length > 0)
            {
                if (StencilShader == null)
                {
                    StencilShader = Shader.Find("Unlit/Transparent Cutout(Stencil Write 128)");
                }
                if (StencilShader != null)
                {
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        SkinnedMeshRenderer renderer = renderers[i];
                        if (renderer == null || renderer.gameObject.activeSelf==false || renderer.enabled == false) continue;
                        if (renderer.name.Contains("Face"))
                        {
                            OriginalFaceShader = renderer.material.shader;
                            renderer.material.shader = StencilShader;
                        }
                        else if (renderer.name.Contains("Avatar"))
                        {
                            OriginalBodyShader = renderer.material.shader;
                            renderer.material.shader = StencilShader;
                        }
                    }
                }
            }
        }
        public void DisableStencilTest()
        {
            OpenStencilTest = false;
            GL.Clear(true, true, Color.black);
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SkinnedMeshRenderer renderer = renderers[i];
                    if (renderer == null) continue;
                    if (renderer.name.Contains("Face"))
                    {
                        renderer.material.shader = OriginalFaceShader;
                    }
                    else if (renderer.name.Contains("Avatar"))
                    {
                        renderer.material.shader = OriginalBodyShader;
                    }
                }
            }
        }

        #endregion
    }
}
