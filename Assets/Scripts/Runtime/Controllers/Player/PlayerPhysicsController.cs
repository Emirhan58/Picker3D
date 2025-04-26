using Runtime.Managers;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Controllers.Player
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private PlayerManager manager;
        [SerializeField] private new Collider collider;
        [SerializeField] private new Rigidbody rigidbody;
        
        #endregion

        #region Private Variables

        private readonly string _stageArea = "StageArea";
        private readonly string _finishArea = "FinishArea";
        private readonly string _miniGameArea = "MiniGameArea";

        #endregion
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_stageArea))
            {
                manager.ForceCommand.Execute();
                CoreGameSignals.Instance.onStageAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();
                
                //Stage Area Kontrol Süreci
            }

            if (other.CompareTag(_finishArea))
            {
                CoreGameSignals.Instance.onFinishAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();
                CoreGameSignals.Instance.onLevelSuccessful?.Invoke();
                return;
            }
            
            if(other.CompareTag(_miniGameArea))
            {
                // Write the MiniGame Mechanics
            }
        }

        public void OnReset()
        {
            
        }
    }
}