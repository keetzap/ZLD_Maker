using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Keetzap.ZeldaMaker
{
    public sealed class CanvasMainMenu : CanvasBase
    {
        [SerializeField] private Button btn_DemoScene;
        [SerializeField] private Button btn_ShowcaseScene;
        [SerializeField] private Button btn_Help;
        [SerializeField] private Button btn_Contact;

        void Awake()
        {
            btn_DemoScene.onClick.AddListener(DemoScene);
            btn_ShowcaseScene.onClick.AddListener(ShowcaseScene);
            btn_Help.onClick.AddListener(Help);
            btn_Contact.onClick.AddListener(Contact);
        }

       
        public override void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(btn_DemoScene.gameObject);
            btn_DemoScene.GetComponent<GenericButtonController>().OnSelect();

            base.OnEnable();
        }

        private void DemoScene()
        {
        }

        private void ShowcaseScene()
        {
        }

        private void Help()
        {
        }

        private void Contact()
        {
        }
    }
}
