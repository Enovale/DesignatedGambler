using System.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using MainUI;
using UnityEngine;

namespace DesignatedGambler
{
    public class PluginBootstrap : MonoBehaviour
    {
        public static PluginBootstrap Instance;

        private static bool _gamblingAllowed = false;

        private KeyCode[] _keysToCheck = new[]
        {
            KeyCode.DownArrow,
            KeyCode.UpArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.B,
            KeyCode.A,
            KeyCode.Return
        };

        private KeyCode[] _correctSequence = new[]
        {
            KeyCode.UpArrow,
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.B,
            KeyCode.A,
            KeyCode.Return
        };

        private List<KeyCode> _sequenceBuffer = new();

        internal static void Setup()
        {
            ClassInjector.RegisterTypeInIl2Cpp<PluginBootstrap>();

            GameObject obj = new(MyPluginInfo.PLUGIN_GUID + "bootstrap");
            DontDestroyOnLoad(obj);
            obj.hideFlags |= HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<PluginBootstrap>();
        }

        private void Update()
        {
            if (GachaUIPresenter.Instance != null && GachaUIPresenter.Instance.GachaPanel.IsOpened)
            {
                foreach (var keyCode in _keysToCheck)
                {
                    if (Input.GetKeyUp(keyCode))
                    {
                        _sequenceBuffer.Add(keyCode);
                        if (_sequenceBuffer.Count > _correctSequence.Length)
                        {
                            _sequenceBuffer.RemoveAt(0);
                        }
                    }
                }

                var sequenceCorrect = true;

                if (_sequenceBuffer.Count != _correctSequence.Length)
                {
                    sequenceCorrect = false;
                }
                else
                {
                    for (var i = 0; i < _sequenceBuffer.Count; i++)
                    {
                        if (_correctSequence[i] != _sequenceBuffer[i])
                        {
                            sequenceCorrect = false;
                        }
                    }
                }

                if (sequenceCorrect)
                {
                    _gamblingAllowed = true;
                    _sequenceBuffer.Clear();
                }
            }
        }

        // Run in fixed update because we don't need frame by frame updates
        private void FixedUpdate()
        {
            if (GachaResultUI.Instance != null && GachaResultUI.Instance.IsOpened)
            {
                _gamblingAllowed = false;
            }
            
            if (GachaUIPresenter.Instance != null)
            {
                if (GachaUIPresenter.Instance.GachaPanel.IsOpened)
                {
                    if (GachaUIPresenter.Instance.GachaPanel.button_gachaOne.interactable != _gamblingAllowed)
                    {
                        GachaUIPresenter.Instance.GachaPanel.button_gachaOne.interactable = _gamblingAllowed;
                        GachaUIPresenter.Instance.GachaPanel.button_gachaTen.interactable = _gamblingAllowed;
                    }
                }
                else
                {
                    _gamblingAllowed = false;
                }
            }
        }
    }
}