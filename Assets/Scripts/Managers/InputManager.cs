using System.Collections.Generic;
using Data.UnityObjects;
using Data.ValueObjects;
using Keys;
using Signals;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        #region Self Variables

        #region Private Variables

        private InputData _data;
        private bool _isAvailableForTouch, _isFirstTimeTouchTaken, _isTouching;

        private float _currentVelocity;
        private float3 _moveVector;
        private Vector2? _mousePosition;

        #endregion

        #endregion

        private void Awake()
        {
            _data = GetInputData();
        }

        private InputData GetInputData()
        {
            return Resources.Load<CD_Input>("Data/CD_Input").Data;
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onReset += OnReset;
            InputSignals.Instance.onEnableInput += OnEnableInput;
            InputSignals.Instance.onDisableInput += OnDisableInput;
        }

        private void OnDisableInput()
        {
            _isAvailableForTouch = false;
        }

        private void OnEnableInput()
        {
            _isAvailableForTouch = true;
        }

        private void OnReset()
        {
            _isAvailableForTouch = false;
            // _isFirstTimeTouchTaken = false;
            _isTouching = false;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onReset -= OnReset;
            InputSignals.Instance.onEnableInput -= OnEnableInput;
            InputSignals.Instance.onDisableInput -= OnDisableInput;
        }

        private void Update()
        {
            if (!_isAvailableForTouch) return;

            // Dokunma bırakıldığında ve UI üzerine dokunulmamışsa:
            if (Input.GetMouseButtonUp(0) && !IsPointerOverUIElement())
            {
                _isTouching = false;
                InputSignals.Instance.onInputReleased?.Invoke();
                Debug.LogWarning("Executed ---> onInputReleased");
            }

            // İlk dokunma anı yakalandığında ve UI üzerine dokunulmamışsa:
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                _isTouching = true; 
                InputSignals.Instance.onInputTaken?.Invoke(); 
                Debug.LogWarning("Executed ---> onInputTaken");

                // İlk kez dokunuluyorsa özel bir sinyali gönder.
                if (!_isFirstTimeTouchTaken)
                {
                    _isFirstTimeTouchTaken = true;
                    InputSignals.Instance.onFirstTimeTouchTaken?.Invoke();
                    Debug.LogWarning("Executed ---> onFirstTimeTouchTaken");
                }

                // Başlangıç pozisyonunu kaydet.
                _mousePosition = (Vector2)Input.mousePosition;
            }

            // Dokunma sürdürülüyorsa ve UI üzerine dokunulmamışsa:
            if (Input.GetMouseButton(0) && !IsPointerOverUIElement())
            {
                if (_isTouching)
                {
                    // Başlangıç pozisyonu boş değilse sürükleme işlemi başlatılır.
                    if (_mousePosition != null)
                    {
                        // Şu anki pozisyon ile önceki pozisyon arasındaki fark hesaplanır.
                        Vector2 mouseDeltaPos = (Vector2)Input.mousePosition - _mousePosition.Value;

                        // Sağa doğru yeterli miktarda sürüklendiyse:
                        if (mouseDeltaPos.x > _data.HorizontalInputSpeed)
                        {
                            _moveVector.x = _data.HorizontalInputSpeed / 10f * mouseDeltaPos.x;
                        }
                        // Sola doğru yeterli miktarda sürüklendiyse:
                        else if (mouseDeltaPos.x < -_data.HorizontalInputSpeed)
                        {
                            _moveVector.x = _data.HorizontalInputSpeed / 10f * mouseDeltaPos.x;
                        }
                        else
                        {
                            // Sürükleme miktarı küçükse veya hareketsizse hareketi yumuşakça durdur.
                            _moveVector.x = Mathf.SmoothDamp(-_moveVector.x, 0, ref _currentVelocity, _data.HorizontalInputSpeed);
                        }

                        // Bir sonraki frame için şu anki pozisyon güncellenir.
                        _mousePosition = (Vector2)Input.mousePosition;

                        // Sürükleme sinyalini tetikle ve gerekli parametreleri ilet.
                        InputSignals.Instance.onInputDragged?.Invoke(new HorizontalInputParams()
                        {
                            HorizontalValue = _moveVector.x,
                            ClampValues = (float2)_data.ClampValues
                        });
                    }
                }
            }
        }


        private bool IsPointerOverUIElement()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = (Vector2)Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
    }
}