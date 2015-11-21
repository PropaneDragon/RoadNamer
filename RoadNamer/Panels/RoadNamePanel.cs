using System;
using ColossalFramework;
using ColossalFramework.UI;
using RoadNamer.CustomUI;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using UnityEngine;
using System.Text.RegularExpressions;

namespace RoadNamer.Panels
{
    public class RoadNamePanel : UIPanel
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

        private UITitleBar m_panelTitle;
        private UITextField m_textField;
        private UIColorField m_colourSelector;
        private string m_initialRoadName;

        public ushort m_netSegmentId = 0;
        public string m_netSegmentName;

        public string initialRoadName
        {
            set
            {
                m_initialRoadName = value;

                if (m_textField != null)
                {
                    UpdateTextField(value);
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
            this.width = 350;
            this.height = 100;

            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.title = "Set a name";
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
            if (eventParam.keycode == KeyCode.KeypadEnter || eventParam.keycode == KeyCode.Return)
            {
                SetRoadData();
            }
        }

        private void CreatePanelComponents()
        {
            m_colourSelector = CustomUI.UIUtils.CreateColorField(this);
            m_colourSelector.relativePosition = new Vector3(m_UIPadding.left, 0);
            m_colourSelector.pickerPosition = UIColorField.ColorPickerPosition.LeftBelow;
            m_colourSelector.eventColorChanged += ColourSelector_eventColorChanged;
            m_colourSelector.eventColorPickerClose += ColourSelector_eventColorPickerClose;
            m_colourSelector.tooltip = "Set the label colour";
            m_colourSelector.PerformLayout();

            m_textField = CustomUI.UIUtils.CreateTextField(this);
            m_textField.relativePosition = new Vector3((m_UIPadding.left * 2) + m_colourSelector.width, m_panelTitle.height + m_UIPadding.bottom);
            m_textField.width = this.width - m_textField.relativePosition.x - m_UIPadding.right;
            m_textField.eventKeyDown += M_textField_eventKeyDown;
            m_textField.processMarkup = false; //Might re-implement this eventually (needs work to stop it screwing up with markup)

            UIButton nameRoadButton = CustomUI.UIUtils.CreateButton(this);
            nameRoadButton.text = "Set";
            nameRoadButton.size = new Vector2(60, 30);
            nameRoadButton.relativePosition = new Vector3(this.width - nameRoadButton.width - m_UIPadding.right, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            nameRoadButton.eventClicked += NameRoadButton_eventClicked;
            nameRoadButton.tooltip = "Create the label";

            UIButton randomRoadNameButton = CustomUI.UIUtils.CreateButton(this);
            randomRoadNameButton.text = "Random Road";
            randomRoadNameButton.size = new Vector2(120, 30);
            randomRoadNameButton.relativePosition = new Vector3(this.width - nameRoadButton.width - 2 * m_UIPadding.right - randomRoadNameButton.width, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            randomRoadNameButton.eventClicked += RandomRoadNameButton_eventClicked;
            randomRoadNameButton.tooltip = "Generate a random name for the road";

            UIButton randomRouteNameButton = CustomUI.UIUtils.CreateButton(this);
            randomRouteNameButton.text = "Random Route";
            randomRouteNameButton.size = new Vector2(120, 30);
            randomRouteNameButton.relativePosition = new Vector3(this.width - nameRoadButton.width - 3 * m_UIPadding.right - randomRoadNameButton.width - randomRouteNameButton.width, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            randomRouteNameButton.eventClicked += RandomRouteNameButton_eventClicked;
            randomRouteNameButton.tooltip = "Generate a random route name for the road";

            this.height = nameRoadButton.relativePosition.y + nameRoadButton.height + m_UIPadding.bottom;
        }

        private void ColourSelector_eventColorPickerClose(UIColorField dropdown, UIColorPicker popup, ref bool overridden)
        {
            m_textField.textColor = popup.color;
        }

        private void ColourSelector_eventColorChanged(UIComponent component, Color32 value)
        {
            m_textField.textColor = value;
        }

        private void M_textField_eventKeyDown(UIComponent component, UIKeyEventParameter eventParam)
        {
            if (eventParam.keycode == KeyCode.KeypadEnter || eventParam.keycode == KeyCode.Return)
            {
                SetRoadData();
            }
        }

        private void RandomRoadNameButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            GetRandomName();
        }

        private void RandomRouteNameButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            GetRandomRoute();
        }

        private void NameRoadButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            SetRoadData();
        }

        private void GetRandomName()
        {
            string randomName = "";
            //TODO: Check against usedName hashset to determine random name uniqueness
            randomName = Utilities.RandomNameUtilities.GenerateRoadName(m_netSegmentName);
            m_textField.text = randomName;
        }

        private void GetRandomRoute()
        {
            string randomName = "";
            //TODO: Check against usedName hashset to determine random name uniqueness
            randomName = Utilities.RandomNameUtilities.GenerateRouteName(m_netSegmentName);
            m_textField.text = randomName;
        }

        private void SetRoadData()
        {
            if (m_netSegmentId != 0)
            {
                string roadName = m_textField.text;

                if (roadName != null)
                {
                    string hexColour = UIMarkupStyle.ColorToHex(m_textField.textColor);
                    roadName = "<color" + hexColour + ">" + roadName + "</color>";

                    RoadRenderingManager roadRenderingManager = Singleton<RoadRenderingManager>.instance;
                    RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName);
                    Hide();

                    roadRenderingManager.ForceUpdate();
                }
            }
        }

        private void UpdateTextField(string text)
        {
            if (text != null)
            {
                Color textFieldColour = StringUtilities.ExtractColourFromTags(text, new Color(1, 1, 1));
                string sanitisedLabel = StringUtilities.RemoveTags(text);

                m_textField.textColor = textFieldColour;
                m_textField.text = sanitisedLabel;

                m_colourSelector.selectedColor = textFieldColour;
            }
            else
            {
                m_textField.text = "";
            }
        }
    }
}
