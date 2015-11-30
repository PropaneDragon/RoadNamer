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
    public class RoadNamePanel : UIPanel, IEventSubscriber
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

        private InfoPanel m_infoPanel;
        private UITitleBar m_panelTitle;
        private UITextField m_textField;
        private UIDropDown m_routeTypeDropdown;
        private UITextField m_routeNumField;
        private UILabel m_roadNameLabel;
        private UILabel m_routeLabel;

        private UIColorField m_colourSelector;
        private string m_initialRoadName;
       

        public ushort m_netSegmentId = 0;
        public string m_netSegmentName;
        public string m_initialRouteNum = null;
        public string m_initialRoutePrefix = null;

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

            m_infoPanel = this.AddUIComponent<InfoPanel>();
            m_infoPanel.Hide();

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.title = "Set a name";
            m_panelTitle.iconAtlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_panelTitle.iconSprite = "ToolbarFGIcon";

            CreatePanelComponents();
            CreateUpdatePanel();

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
            m_infoPanel.relativePosition = new Vector3(this.width - 20, -(m_infoPanel.height - 20));

            m_roadNameLabel = this.AddUIComponent<UILabel>();
            m_roadNameLabel.textScale = 1f;
            m_roadNameLabel.size = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_roadNameLabel.textColor = new Color32(180, 180, 180, 255);
            m_roadNameLabel.relativePosition = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_roadNameLabel.textAlignment = UIHorizontalAlignment.Left;
            m_roadNameLabel.text = "Road Name";

            m_textField = CustomUI.UIUtils.CreateTextField(this);
            m_textField.relativePosition = new Vector3(m_UIPadding.left, m_roadNameLabel.relativePosition.y + m_roadNameLabel.height + m_UIPadding.bottom);
            m_textField.height = 25;
            m_textField.width = this.width - m_UIPadding.left - (m_UIPadding.right * 2) - m_textField.height;
            m_textField.eventKeyDown += m_textField_eventKeyDown;
            m_textField.processMarkup = false; //Might re-implement this eventually (needs work to stop it screwing up with markup)
            m_textField.textColor = Color.white;

            m_routeLabel = this.AddUIComponent<UILabel>();
            m_routeLabel.textScale = 1f;
            m_routeLabel.size = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_routeLabel.textColor = new Color32(180, 180, 180, 255);
            m_routeLabel.relativePosition = new Vector3(m_UIPadding.left, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            m_routeLabel.textAlignment = UIHorizontalAlignment.Left;
            m_routeLabel.text = "Road Name";

            m_routeTypeDropdown = CustomUI.UIUtils.CreateDropDown(this, new Vector2(((this.width - m_UIPadding.left - 2 * m_UIPadding.right) / 2f), 25));
            //TODO: Replace with Random namer values
            m_routeTypeDropdown.AddItem("Route ");
            m_routeTypeDropdown.AddItem("Hwy ");
            m_routeTypeDropdown.AddItem("I-");
            m_routeTypeDropdown.AddItem("M-");
            m_routeTypeDropdown.AddItem("A-");
            m_routeTypeDropdown.AddItem("E-");
            m_routeTypeDropdown.selectedIndex = 0;
            m_routeTypeDropdown.relativePosition = new Vector3(m_UIPadding.left, m_routeLabel.relativePosition.y + m_routeLabel.height + m_UIPadding.bottom);

            m_routeNumField = CustomUI.UIUtils.CreateTextField(this);
            m_routeNumField.relativePosition = new Vector3(m_UIPadding.left + m_routeTypeDropdown.width + m_UIPadding.right, m_routeLabel.relativePosition.y + m_routeLabel.height + m_UIPadding.bottom);
            m_routeNumField.height = 25;
            m_routeNumField.width = (this.width - m_UIPadding.left - 2 * m_UIPadding.right) / 2f;
            m_routeNumField.processMarkup = false; //Might re-implement this eventually (needs work to stop it screwing up with markup)
            m_routeNumField.textColor = Color.white;
            m_routeNumField.numericalOnly = true;
            m_routeNumField.maxLength = 3;

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
            colourSelectorPinPanel.relativePosition = new Vector3(m_UIPadding.left, m_routeNumField.relativePosition.y + m_routeNumField.height + m_UIPadding.bottom);

            m_colourSelector = CustomUI.UIUtils.CreateColorField(colourSelectorPinPanel);
            m_colourSelector.pickerPosition = UIColorField.ColorPickerPosition.LeftBelow;
            m_colourSelector.eventColorChanged += ColourSelector_eventColorChanged;
            m_colourSelector.eventColorPickerClose += ColourSelector_eventColorPickerClose;
            m_colourSelector.tooltip = "Set the text colour";
            m_colourSelector.relativePosition = new Vector3(0, 0);

            UIButton nameRoadButton = CustomUI.UIUtils.CreateButton(this);
            nameRoadButton.text = "Set";
            nameRoadButton.size = new Vector2(60, 30);
            nameRoadButton.relativePosition = new Vector3(this.width - nameRoadButton.width - m_UIPadding.right, m_routeNumField.relativePosition.y + m_routeNumField.height + m_UIPadding.bottom);
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
            string randomName = RandomNameManager.GenerateRandomRoadName(m_netSegmentId);

            if (randomName != null)
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
                int routeNum = -1;
                bool validRouteNum = Int32.TryParse(m_routeNumField.text, out routeNum);
                if (roadName != null)
                {
                    roadName = StringUtilities.WrapNameWithColorTags(roadName, m_textField.textColor);
                    RoadRenderingManager roadRenderingManager = Singleton<RoadRenderingManager>.instance;
                    if(validRouteNum)
                    {
                        RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName, m_initialRoadName, m_routeTypeDropdown.selectedValue, routeNum);
                    }
                    else
                    {
                        RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName, m_initialRoadName);
                    }
                    Hide();
                    EventBusManager.Instance().Publish("closeUsedNamePanel", null);
                    EventBusManager.Instance().Publish("forceupdateroadnames", null);
                    roadRenderingManager.ForceUpdate();
                }
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

            if(m_initialRouteNum != null && m_initialRoutePrefix != null)
            {
                int initialRouteType = 0;
                for(int i=0; i<m_routeTypeDropdown.items.Length; i++)
                {
                    if( m_routeTypeDropdown.items[i].ToLower() == m_initialRoutePrefix.ToLower())
                    {
                        initialRouteType = i;
                        break;
                    }
                }
                m_routeTypeDropdown.selectedIndex = initialRouteType;
                m_routeNumField.text = m_initialRouteNum;
            }
            else
            {
                m_routeTypeDropdown.selectedIndex = 0;
                m_routeNumField.text = "";
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
                case "updateroutepaneltext":
                    if (message != null)
                    {

                        string[] routeValues = message.Split('/');
#if DEBUG
                        LoggerUtilities.LogToConsole(routeValues[1]);
#endif
                        int routeType = 0;
                        for (int i = 0; i < m_routeTypeDropdown.items.Length; i++)
                        {
                            if (m_routeTypeDropdown.items[i].ToLower() == routeValues[0].ToLower())
                            {
                                routeType = i;
                                break;
                            }
                        }
                        m_routeTypeDropdown.selectedIndex = routeType;
                        m_routeNumField.text = routeValues[1];
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
