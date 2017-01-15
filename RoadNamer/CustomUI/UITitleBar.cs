using UnityEngine;
using ColossalFramework.UI;
using System;
using RoadNamer.Managers;
using System.Collections.Generic;

namespace RoadNamer.CustomUI
{
    class UITitleBar : UIPanel
    {
        private UISprite m_icon;
        private UILabel m_title;
        private UIButton m_close;
        private UIDragHandle m_drag;

        public List<String> m_closeActions = new List<string>();

        public string iconSprite
        {
            get { return m_icon.spriteName; }
            set
            {
                if (m_icon != null)
                {
                    m_icon.spriteName = value;

                    if (m_icon.atlas == null)
                    {
                        m_icon.atlas = UIUtils.defaultAtlas;
                    }

                    if (m_icon.spriteInfo != null)
                    {
                        m_icon.size = m_icon.spriteInfo.pixelSize;
                        UIUtils.ResizeIcon(m_icon, new Vector2(32, 32));
                        m_icon.relativePosition = new Vector3(10, 5);
                    }
                }
            }
        }

        public UITextureAtlas iconAtlas
        {
            get { return m_icon.atlas; }
            set { m_icon.atlas = value; }
        }

        public UIButton closeButton
        {
            get { return m_close; }
        }

        public string title
        {
            get { return m_title.text; }
            set { m_title.text = value; }
        }

        public override void Awake()
        {
            base.Awake();

            m_icon = AddUIComponent<UISprite>();
            m_title = AddUIComponent<UILabel>();
            m_close = AddUIComponent<UIButton>();
            m_drag = AddUIComponent<UIDragHandle>();

            height = 40;
            width = 450;
            title = "(None)";
            iconSprite = "";
        }

        public override void Start()
        {
            base.Start();

            width = parent.width;
            relativePosition = Vector3.zero;
            isVisible = true;
            canFocus = true;
            isInteractive = true;

            m_drag.width = width - 50;
            m_drag.height = height;
            m_drag.relativePosition = Vector3.zero;
            m_drag.target = parent;

            m_icon.spriteName = iconSprite;
            m_icon.relativePosition = new Vector3(10, 5);

            m_title.relativePosition = new Vector3(50, 13);
            m_title.text = title;
            m_title.textAlignment = UIHorizontalAlignment.Center;

            m_close.atlas = UIUtils.defaultAtlas;
            m_close.relativePosition = new Vector3(width - 35, 2);
            m_close.normalBgSprite = "buttonclose";
            m_close.hoveredBgSprite = "buttonclosehover";
            m_close.pressedBgSprite = "buttonclosepressed";
            m_close.eventClick += CloseButton_clickedEventHandler;

            m_title.width = parent.width - relativePosition.x - m_close.width - 10;
        }

        private void CloseButton_clickedEventHandler(UIComponent component, UIMouseEventParameter eventParam)
        {
            parent.Hide();
            foreach (string closeAction in m_closeActions)
            {
                EventBusManager.Instance().Publish(closeAction, null);
            }
        }
    }
}
