using Runtime.Commands.Player;
using Runtime.Controllers.Player;
using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using Runtime.Keys;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        #region Self Variables
        
        #region Public Variables

        public byte StageValue;

        internal ForceBallsToPoolCommand ForceCommand;

        #endregion

        #region Serialized Variables

        [SerializeField] private PlayerMovementController movementController;
        [SerializeField] private PlayerMeshController meshController;
        [SerializeField] private PlayerPhysicsController physicsController;

        #endregion

        #region Private Variables

        private PlayerData _data;

        #endregion

        #endregion

        private void Awake()
        {
            _data = GetPlayerData();
            SendDataToControllers();
            Init();
        }
        private PlayerData GetPlayerData()
        {
            return Resources.Load<CD_Player>("Data/CD_Player").Data;
        }
        private void SendDataToControllers()
        {
            movementController.SetData(_data.MovementData);
            meshController.SetData(_data.MeshData);
        }
        private void Init()
        {
            ForceCommand = new ForceBallsToPoolCommand(this, _data.ForceData);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            InputSignals.Instance.onInputTaken += OnInputTaken;
            InputSignals.Instance.onInputReleased += OnInputReleased;
            InputSignals.Instance.onInputDragged += OnInputDragged;
            UISignals.Instance.onPlay += OnPlay;
            CoreGameSignals.Instance.onLevelSuccessful += OnLevelSuccessful;
            CoreGameSignals.Instance.onLevelFailed += OnLevelFailed;
            CoreGameSignals.Instance.onStageAreaEntered += OnStageAreaEntered;
            CoreGameSignals.Instance.onStageAreaSuccessFul += OnStageAreaSuccessFul;
            CoreGameSignals.Instance.onFinishAreaEntered += OnFinishAreaEntered;
            CoreGameSignals.Instance.onReset += OnReset;
        }
        
        private void OnPlay()
        {
            movementController.IsReadyToPlay(true);
        }
        
        private void OnInputTaken()
        {
            movementController.IsReadyToMove(true);
        }
        
        private void OnInputDragged(HorizontalInputParams inputParams)
        {
            movementController.UpdateInputParams(inputParams);
        }

        private void OnInputReleased()
        {
            movementController.IsReadyToMove(false);
        }

        private void OnStageAreaEntered()
        {
            movementController.IsReadyToPlay(false);
        }
        
        private void OnStageAreaSuccessFul(byte value)
        {
            StageValue = ++value;
        }
        
        private void OnFinishAreaEntered()
        {
            CoreGameSignals.Instance.onLevelSuccessful?.Invoke();
            // Mini game yazılmalı
        }

        private void OnLevelFailed()
        {
            movementController.IsReadyToPlay(false);
        }

        private void OnLevelSuccessful()
        {
            movementController.IsReadyToPlay(false);
        }

        private void OnReset()
        {
            StageValue = 0;
            movementController.OnReset();
            physicsController.OnReset();
            meshController.OnReset();
        }

        private void UnsubscribeEvents()
        {
            InputSignals.Instance.onInputTaken -= OnInputTaken;
            InputSignals.Instance.onInputReleased -= OnInputReleased;
            InputSignals.Instance.onInputDragged -= OnInputDragged;
            UISignals.Instance.onPlay -= OnPlay;
            CoreGameSignals.Instance.onLevelSuccessful -= OnLevelSuccessful;
            CoreGameSignals.Instance.onLevelFailed -= OnLevelFailed;
            CoreGameSignals.Instance.onStageAreaEntered -= OnStageAreaEntered;
            CoreGameSignals.Instance.onStageAreaSuccessFul -= OnStageAreaSuccessFul;
            CoreGameSignals.Instance.onFinishAreaEntered -= OnFinishAreaEntered;
            CoreGameSignals.Instance.onReset -= OnReset;
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
    }
}