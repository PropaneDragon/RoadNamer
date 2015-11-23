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
    public class UsedNamesPanel : UIPanel,IEventSubscriber
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

        private int titleOffset = 40;
        private UITitleBar m_panelTitle;
        public UIFastList scrollOptionsList = null;
        private Vector2 offset = Vector2.zero;

        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;
            this.width = 350;
            this.height = 600;

            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.title = "Existing names";
            m_panelTitle.iconAtlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_panelTitle.iconSprite = "ToolbarFGIcon";

            CreatePanelComponents();

            this.relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2) + width, Mathf.Floor((GetUIView().fixedHeight - height) / 2));
            this.backgroundSprite = "MenuPanel2";
            this.atlas = CustomUI.UIUtils.GetAtlas("Ingame");
        }

        private void CreatePanelComponents()
        {
            offset = new Vector2(m_UIPadding.left, titleOffset+m_UIPadding.top);
            scrollOptionsList = UIFastList.Create<UIUsedNameRowItem>(this);
            scrollOptionsList.backgroundSprite = "UnlockingPanel";
            scrollOptionsList.size = new Vector2(350-m_UIPadding.left-m_UIPadding.right, 600-titleOffset-m_UIPadding.top-m_UIPadding.bottom);        
            scrollOptionsList.canSelect = false;
            scrollOptionsList.relativePosition = offset;
            scrollOptionsList.rowHeight = 40f;
            scrollOptionsList.rowsData.Clear();
            scrollOptionsList.selectedIndex = -1;

            RefreshList();
        }

        public void RefreshList()
        {
            scrollOptionsList.rowsData.Clear();
            foreach (string usedName in RoadNameManager.Instance().m_usedNames)
            {
                scrollOptionsList.rowsData.Add(usedName);
            }
            scrollOptionsList.DisplayAt(0);
            scrollOptionsList.selectedIndex = 0;
        }

        public void onReceiveEvent(string eventName, object eventData)
        {
            LoggerUtilities.LogToConsole(eventName);
            string message = eventData as string;
            switch (eventName)
            {
                case "forceupdateroadnames":
                    RefreshList();
                    break;
                default:
                    break;
            }
        }
    }
}
