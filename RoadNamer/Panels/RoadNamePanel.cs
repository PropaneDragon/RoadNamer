using ColossalFramework;
using ColossalFramework.UI;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using UnityEngine;
using CimTools.v2.Utilities;
using CimTools.v2.Panels;
using CimTools.v2.Elements;
using CimTools.v2.Workshop;
using System.Collections.Generic;
using System;

namespace RoadNamer.Panels
{
    public class RoadNamePanel : UIPanel, IEventSubscriber
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

        private UpdatePanel m_infoPanel;
        private UITitleBar m_panelTitle;
        private UITextField m_textField;
        private UILabel m_roadNameLabel;

        private UIColorField m_colourSelector;
        private UIButton m_randomNameButton;
        private string m_initialRoadName;

        public ushort m_netSegmentId = 0;
        public List<ushort> m_netSegmentIds = new List<ushort>();
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
            this.height = 350;

            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            m_infoPanel = this.AddUIComponent<UpdatePanel>();
            m_infoPanel.Initialise(CimToolHolder.toolBase);
            m_infoPanel.Hide();

            Changelog changelogDownloader = m_infoPanel.m_changelogDownloader;

            if(changelogDownloader != null && !changelogDownloader.DownloadComplete && !changelogDownloader.DownloadInProgress)
            {
                changelogDownloader.DownloadChangelogAsync();
            }

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.Initialise(CimToolHolder.toolBase);
            m_panelTitle.title = "Set a name";
            m_panelTitle.iconAtlas = CimToolHolder.toolBase.SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_panelTitle.iconSprite = "ToolbarFGIcon";

            CreatePanelComponents();
            CreateUpdatePanel();

            this.relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
            this.backgroundSprite = "MenuPanel2";
            this.atlas = CimToolHolder.toolBase.UIUtilities.defaultAtlas;

            this.eventKeyPress += RoadNamePanel_eventKeyPress;
            this.eventVisibilityChanged += RoadNamePanel_eventVisibilityChanged;
        }

        private void RoadNamePanel_eventVisibilityChanged(UIComponent component, bool visible)
        {
            if( !visible)
            {
                EventBusManager.Instance().Publish("closeAll", null);
            }
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
            m_infoPanel.SetPositionSpeakyPoint(new Vector2(this.width - 20, -20));

            m_roadNameLabel = this.AddUIComponent<UILabel>();
            m_roadNameLabel.textScale = 1f;
            m_roadNameLabel.size = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_roadNameLabel.textColor = new Color32(180, 180, 180, 255);
            m_roadNameLabel.relativePosition = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_roadNameLabel.textAlignment = UIHorizontalAlignment.Left;
            m_roadNameLabel.text = "Road Name";

            m_textField = CimToolHolder.toolBase.UIUtilities.CreateTextField(this);
            m_textField.relativePosition = new Vector3(m_UIPadding.left, m_roadNameLabel.relativePosition.y + m_roadNameLabel.height + m_UIPadding.bottom);
            m_textField.height = 25;
            m_textField.width = this.width - m_UIPadding.left - (m_UIPadding.right * 2) - m_textField.height;
            m_textField.eventKeyDown += m_textField_eventKeyDown;
            m_textField.processMarkup = false; //Might re-implement this eventually (needs work to stop it screwing up with markup)
            m_textField.textColor = Color.white;
            
            m_randomNameButton = CimToolHolder.toolBase.UIUtilities.CreateButton(this);
            m_randomNameButton.text = "";
            m_randomNameButton.size = new Vector2(m_textField.height, m_textField.height);
            m_randomNameButton.relativePosition = new Vector3(m_textField.relativePosition.x + m_textField.width + m_UIPadding.left, m_textField.relativePosition.y);
            m_randomNameButton.atlas = CimToolHolder.toolBase.SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_randomNameButton.disabledBgSprite = "DiceIcon";
            m_randomNameButton.normalFgSprite = "DiceIcon";
            m_randomNameButton.focusedFgSprite = "DiceIcon";
            m_randomNameButton.hoveredFgSprite = "DiceIcon";
            m_randomNameButton.pressedFgSprite = "DiceIcon";
            m_randomNameButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            //m_randomNameButton.tooltip = RandomNameManager.m_fileName;
            m_randomNameButton.eventClicked += RandomNameButton_eventClicked;
            
            UIPanel colourSelectorPinPanel = this.AddUIComponent<UIPanel>();
            colourSelectorPinPanel.relativePosition = new Vector3(m_UIPadding.left, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            
            m_colourSelector = CimToolHolder.toolBase.UIUtilities.CreateColorField(colourSelectorPinPanel);
            m_colourSelector.pickerPosition = UIColorField.ColorPickerPosition.LeftBelow;
            m_colourSelector.eventColorChanged += ColourSelector_eventColorChanged;
            m_colourSelector.eventColorPickerClose += ColourSelector_eventColorPickerClose;
            m_colourSelector.tooltip = "Set the text colour";
            m_colourSelector.relativePosition = new Vector3(0, 0);

            UIButton nameRoadButton = CimToolHolder.toolBase.UIUtilities.CreateButton(this);
            nameRoadButton.text = "Set";
            nameRoadButton.size = new Vector2(60, 30);
            nameRoadButton.relativePosition = new Vector3(this.width - nameRoadButton.width - m_UIPadding.right, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            nameRoadButton.eventClicked += NameRoadButton_eventClicked;
            nameRoadButton.tooltip = "Create the label";

            this.height = nameRoadButton.relativePosition.y + nameRoadButton.height + m_UIPadding.bottom;
        }

        private void CreateUpdatePanel()
        {
            if (OptionsManager.m_versionStringFull != SavedOptionManager.Instance().m_lastSavedVersion)
            {
                OptionsManager.SaveOptions();
                m_infoPanel.Show();
            }
        }

        private void RandomNameButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            
            string randomName = RandomNameManager.GenerateRandomRoadName(m_netSegmentIds.GetEnumerator().Current);

            if (randomName != null)
            {
                m_randomNameButton.tooltip = "";
                m_textField.text = randomName;
            }
            else
            {
                m_randomNameButton.tooltip = "Could not find any road names :(";
                m_randomNameButton.RefreshTooltip();
                m_randomNameButton.bringTooltipToFront = true;
                m_randomNameButton.tooltipBox.Show();
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

        private void m_textField_eventKeyDown(UIComponent component, UIKeyEventParameter eventParam)
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

        /// <summary>
        /// Gets the colour from the panel and sets it to be rendered/saved
        /// </summary>
        private void SetRoadData()
        {
            if (m_netSegmentId != 0)
            {
                string roadName = m_textField.text;
                if (roadName != null)
                {
                    roadName = StringUtilities.WrapNameWithColorTags(roadName, m_textField.textColor);
                    RoadRenderingManager roadRenderingManager = RoadRenderingManager.instance;
                    doSetSegmentData(roadName);
                    Hide();
                    EventBusManager.Instance().Publish("closeUsedNamePanel", null);
                    EventBusManager.Instance().Publish("forceupdateroadnames", null);
                    roadRenderingManager.ForceUpdate();
                }
            }
        }

        private void doSetSegmentData(string roadName)
        {
            if( m_netSegmentIds.Count > 1)
            {
                foreach(ushort segmentId in m_netSegmentIds)
                {
                    string oldName = RoadNameManager.Instance().RoadExists(segmentId) ? RoadNameManager.Instance().m_roadDict[segmentId].m_roadName : null;
                    RoadNameManager.Instance().SetRoadName(segmentId, roadName, oldName);

                }
            }else
            {
                RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName, m_initialRoadName);
            }
        }

        /// <summary>
        /// Sets the panel text field to road data. Converts colours.
        /// </summary>
        /// <param name="text"></param>
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
            string message = eventData as string;
            switch (eventName)
            {
                case "updateroadnamepaneltext":
                    if (message != null)
                    {
                        m_textField.text = message;
                    }
                    break;
                case "closeAll":
                    Hide();
                    break;
                default:
                    break;
            }
        }
    }
}
