using ColossalFramework.UI;
using RoadNamer.CustomUI;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using UnityEngine;

namespace RoadNamer.Panels
{
    class RoadNamePanel : UIPanel
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

        private UITitleBar m_panelTitle;
        private UITextField m_textField;
        private string m_initialRoadName;

        public ushort m_netSegmentId = 0;
        public string initialRoadName
        {
            set
            {
                m_initialRoadName = value;
                
                if(m_textField != null)
                {
                    m_textField.text = value == null ? "" : value;
                }
            }
            get
            {
                return m_initialRoadName;
            }
        }

        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;
            this.width = 200;
            this.height = 100;

            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.title = "Name a road";
            m_panelTitle.iconAtlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_panelTitle.iconSprite = "ToolbarFGIcon";

            CreatePanelComponents();
            
            this.relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
            this.backgroundSprite = "MenuPanel2";
            this.atlas = CustomUI.UIUtils.GetAtlas("Ingame");
            this.eventKeyPress += RoadNamePanel_eventKeyPress;
        }

        private void RoadNamePanel_eventKeyPress(UIComponent component, UIKeyEventParameter eventParam)
        {
            if(eventParam.keycode == KeyCode.KeypadEnter || eventParam.keycode == KeyCode.Return)
            {
                SetRoadData();
            }
        }

        private void CreatePanelComponents()
        {
            m_textField = CustomUI.UIUtils.CreateTextField(this);
            m_textField.relativePosition = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_textField.width = this.width - m_UIPadding.left - m_UIPadding.right;
            m_textField.eventKeyDown += M_textField_eventKeyDown;

            UIButton nameRoadButton = CustomUI.UIUtils.CreateButton(this);
            nameRoadButton.text = "Set";
            nameRoadButton.size = new Vector2(60, 30);
            nameRoadButton.relativePosition = new Vector3(this.width - nameRoadButton.width - m_UIPadding.right, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            nameRoadButton.eventClicked += NameRoadButton_eventClicked;

            this.height = nameRoadButton.relativePosition.y + nameRoadButton.height + m_UIPadding.bottom;
        }

        private void M_textField_eventKeyDown(UIComponent component, UIKeyEventParameter eventParam)
        {
            if (eventParam.keycode == KeyCode.KeypadEnter || eventParam.keycode == KeyCode.Return)
            {
                SetRoadData();
            }
        }

        private void NameRoadButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            SetRoadData();
        }

        private void SetRoadData()
        {
            if (m_netSegmentId != 0)
            {
                string roadName = m_textField.text;

                if (roadName != null)
                {
                    RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName);
                    Hide();
                }
            }
        }
    }
}
