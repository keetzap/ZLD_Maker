using UnityEngine;
using TMPro;

namespace Keetzap.ZeldaMaker
{
    public class InfoMessage : MonoBehaviour
    {
        public TMP_Text TXT_message;

        public void PopulateMessage(string message)
        {
            TXT_message.text = message;
        }
    }
}
