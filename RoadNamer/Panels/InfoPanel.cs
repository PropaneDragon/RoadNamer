using ColossalFramework.UI;
using RoadNamer.CustomUI;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoadNamer.Panels
{
    public class InfoPanel : UIPanel, IEventSubscriber
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);
        private UITitleBar m_panelTitle;
        private UILabel m_infoLabel;

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
            EventBusManager.Instance().Subscribe("closeAll", this);

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.title = "I've updated!";
            /*m_panelTitle.iconAtlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_panelTitle.iconSprite = "ToolbarFGIcon";*/

            m_infoLabel = this.AddUIComponent<UILabel>();
            m_infoLabel.width = 200 - m_UIPadding.left - m_UIPadding.right;
            m_infoLabel.wordWrap = true;
            m_infoLabel.processMarkup = true;
            m_infoLabel.autoHeight = true;
            m_infoLabel.text = "<color#c8f582>Click here</color> to see what's changed";
            m_infoLabel.textScale = 0.6f;
            m_infoLabel.relativePosition = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_infoLabel.eventClicked += M_infoLabel_eventClicked;

            this.atlas = CustomUI.UIUtils.GetAtlas("Ingame");
            this.backgroundSprite = "InfoBubble";
            this.height = m_infoLabel.relativePosition.y + m_infoLabel.height + m_UIPadding.bottom + 20;
        }

        private void M_infoLabel_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            ShowUpdateInfo();
        }

        private void ShowUpdateInfo()
        {
            float lastHeight = m_infoLabel.height;

            m_infoLabel.text = "- <color#c8f582>Text now aligns to roads!</color> That's right, text no longer follows your screen and sticks to roads. There's some more options to play with too.\n\n";
                /*"- <color#c8f582>Make your own random road names!</color> Visit the workshop page to find out how to make your own.\n\n" +
                "- <color#c8f582>US style</color> random road names added. Change the random names in the options panel.\n\n" +
                "- Ability to <color#c8f582>disable all text rendering</color> added to the options panel.\n\n" +
                "- Default road names are now white. (They can be changed using the colour selector)";*/

            float heightDifference = m_infoLabel.height - lastHeight;

            this.height = m_infoLabel.relativePosition.y + m_infoLabel.height + m_UIPadding.bottom + 20;
            this.relativePosition -= new Vector3(0, heightDifference);
        }

        public void onReceiveEvent(string eventName, object eventData)
        {
            string message = eventData as string;
            switch (eventName)
            {
                case "closeAll":
                    Hide();
                    break;
                default:
                    break;
            }
        }
    }
}
