using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class InteractableTrigger : Interactable
    {
        new public static class Fields
        {
            public static string KeyIsRequired => nameof(keyIsRequired);
            public static string Key => nameof(key);
            public static string ShowAdditionalInfo => nameof(showAdditionalInfo);
            public static string AdditionalMessage => nameof(additionalMessage);
        }

        [SerializeField] private bool keyIsRequired;
        [SerializeField] private TypeOfKey key;
        [SerializeField] private bool showAdditionalInfo;
        [SerializeField] private string additionalMessage;

        private bool _isLocked;

        public override void OnInteract()
        {
            if (!_isLocked)
            {
                base.OnInteract();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_hasBeenUsed) return;

            if (other.CompareTag(StringsData.PLAYER) && PlayerController.Instance.IsInteracting)
            {
                HideActionMessage();

                if (CheckIfItsLocked(other))
                {
                    if (showAdditionalInfo)
                    {
                        CanvasController.Instance.DisplayMessage(TypeOfMessage.Information, additionalMessage);
                        PauseCharacter();
                        HideWeapons();
                    }
                }
                else
                {
                    if (keyIsRequired)
                    {
                        _gameManager.SetKeyValue(key, -1);
                    }

                    OnTrigger(Collider, other);
                    DestroyAfterInteraction();
                }
            }
        }

        private bool CheckIfItsLocked(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER) && keyIsRequired)
            {
                _isLocked = _gameData.playerStats.GetKeyAmount(key) > 0 ? false : true;
            }

            return _isLocked;
        }
    }   
}
