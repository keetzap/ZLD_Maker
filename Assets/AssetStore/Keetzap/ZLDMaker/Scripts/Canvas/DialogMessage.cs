using TMPro;

namespace Keetzap.ZeldaMaker
{
    public class DialogMessage : InfoMessage
    {
        public TMP_Text TXT_title;

        public void PopulateTitle(string title)
        {
            TXT_title.text = title;
        }
    }
}
