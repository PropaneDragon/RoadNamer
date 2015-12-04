using UnityEngine;
using ColossalFramework.UI;
using RoadNamer.Utilities;
using RoadNamer.Managers;

namespace RoadNamer.CustomUI
{
    class UIUsedNameRowItem : UIPanel, IUIFastListRow
    {
        private UIPanel background;
        private UILabel label;

        public override void Start()
        {
            base.Start();

            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = parent.width;
            height = 40;

            background = AddUIComponent<UIPanel>();
            background.width = width;
            background.height = 40;
            background.relativePosition = Vector2.zero;
            background.zOrder = 0;

            label = this.AddUIComponent<UILabel>();
            label.textScale = 0.6f;
            label.size = new Vector2(width, height);
            label.textColor = new Color32(180, 180, 180, 255);
            label.relativePosition = Vector2.zero;
            label.textAlignment = UIHorizontalAlignment.Left;
            label.processMarkup = true;
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);
            EventBusManager.Instance().Publish("updateroadnamepaneltext", label.text);
        }

        public void Display(object data, bool isRowOdd)
        {
            if (data != null)
            {
                string text = data as string;

                if (text != null && background != null)
                {
                    label.text = text;

                    if (isRowOdd)
                    {
                        background.backgroundSprite = "UnlockingItemBackground";
                        background.color = new Color32(0, 0, 0, 128);
                    }
                    else
                    {
                        background.backgroundSprite = null;
                    }
                }
            }
        }

        public void Select(bool isRowOdd)
        {
            if (background != null)
            {
                /*background.backgroundSprite = "ListItemHighlight";
                background.color = new Color32(255, 255, 255, 255);*/
            }
        }

        public void Deselect(bool isRowOdd)
        {
            if (background != null)
            {
                if (isRowOdd)
                {
                    background.backgroundSprite = "UnlockingItemBackground";
                    background.color = new Color32(0, 0, 0, 128);
                }
                else
                {
                    background.backgroundSprite = null;
                }
            }
        }

    }
}
