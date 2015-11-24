﻿using System;
using ColossalFramework;
using ColossalFramework.UI;
using RoadNamer.CustomUI;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using UnityEngine;
using System.Text.RegularExpressions;

namespace RoadNamer.Panels
{
    public class RoadNamePanel : UIPanel, IEventSubscriber
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
            this.width = 250;
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
            m_textField = CustomUI.UIUtils.CreateTextField(this);
            m_textField.relativePosition = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_textField.height = 21;
            m_textField.width = this.width - m_UIPadding.left - (m_UIPadding.right * 2) - m_textField.height;
            m_textField.eventKeyDown += M_textField_eventKeyDown;
            m_textField.processMarkup = false; //Might re-implement this eventually (needs work to stop it screwing up with markup)
            
            UIButton randomNameButton = CustomUI.UIUtils.CreateButton(this);
            randomNameButton.text = "";
            randomNameButton.size = new Vector2(m_textField.height, m_textField.height);
            randomNameButton.relativePosition = new Vector3(m_textField.relativePosition.x + m_textField.width + m_UIPadding.left, m_textField.relativePosition.y);
            randomNameButton.atlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            randomNameButton.disabledBgSprite = "DiceIcon";
            randomNameButton.normalFgSprite = "DiceIcon";
            randomNameButton.focusedFgSprite = "DiceIcon";
            randomNameButton.hoveredFgSprite = "DiceIcon";
            randomNameButton.pressedFgSprite = "DiceIcon";
            randomNameButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            randomNameButton.eventClicked += RandomNameButton_eventClicked;
            
            UIPanel colourSelectorPinPanel = this.AddUIComponent<UIPanel>();
            colourSelectorPinPanel.relativePosition = new Vector3(m_UIPadding.left, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            
            m_colourSelector = CustomUI.UIUtils.CreateColorField(colourSelectorPinPanel);
            m_colourSelector.relativePosition = new Vector3(0, 0);
            m_colourSelector.pickerPosition = UIColorField.ColorPickerPosition.LeftBelow;
            m_colourSelector.eventColorChanged += ColourSelector_eventColorChanged;
            m_colourSelector.eventColorPickerClose += ColourSelector_eventColorPickerClose;
            m_colourSelector.tooltip = "Set the text colour";
            m_colourSelector.selectedColor = new Color(1, 1, 1);
            m_colourSelector.color = new Color(1, 1, 1);
            
            UIButton nameRoadButton = CustomUI.UIUtils.CreateButton(this);
            nameRoadButton.text = "Set";
            nameRoadButton.size = new Vector2(60, 30);
            nameRoadButton.relativePosition = new Vector3(this.width - nameRoadButton.width - m_UIPadding.right, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            nameRoadButton.eventClicked += NameRoadButton_eventClicked;
            nameRoadButton.tooltip = "Create the label";

            this.height = nameRoadButton.relativePosition.y + nameRoadButton.height + m_UIPadding.bottom;
        }

        private void RandomNameButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            string randomName = RandomNameManager.GenerateRandomRoadName(m_netSegmentId);

            if(randomName != null)
            {
                m_textField.text = randomName;
            }
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
                    roadName = StringUtilities.WrapNameWithColorTags(roadName, m_textField.textColor);
                    RoadRenderingManager roadRenderingManager = Singleton<RoadRenderingManager>.instance;
                    RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName, m_initialRoadName);
                    Hide();
                    EventBusManager.Instance().Publish("closeUsedNamePanel", null);
                    EventBusManager.Instance().Publish("forceupdateroadnames", null);
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

        public void onReceiveEvent(string eventName, object eventData)
        {
            LoggerUtilities.LogToConsole(eventName);
            string message = eventData as string;
            switch (eventName)
            {
                case "updateroadnamepaneltext":
                    if (message != null){
                        m_textField.text = message;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
