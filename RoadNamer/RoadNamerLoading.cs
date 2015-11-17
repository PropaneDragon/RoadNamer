using ColossalFramework.UI;
using ICities;
using RoadNamer.Tools;
using System;
using UnityEngine;

namespace RoadNamer
{
    public class RoadNamerLoading : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewMap || mode == LoadMode.LoadMap)
            {
                UIView view = UIView.GetAView();
                UITabstrip tabStrip;

                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
                {
                    tabStrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
                }
                else
                {
                    tabStrip = UIView.Find<UITabstrip>("MainToolstrip");
                }

                ToolController toolController = ToolsModifierControl.toolController;

                if (!toolController.GetComponent<RoadSelectTool>())
                {
                    ToolsModifierControl.toolController.gameObject.AddComponent<RoadSelectTool>();
                }

                GameObject buttonGameObject = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
                GameObject pageGameObject = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
                UIButton tabButton = tabStrip.AddTab("Road Namer", buttonGameObject, pageGameObject, new Type[] { }) as UIButton;

                tabButton.normalFgSprite = "InfoIconEducation";
                tabButton.focusedFgSprite = "InfoIconEducation";
                tabButton.hoveredFgSprite = "InfoIconEducation";
                tabButton.disabledFgSprite = "InfoIconEducation";
                tabButton.tooltip = "Road Namer";
                tabButton.eventClicked += TabButton_eventClicked;
            }
        }

        private void TabButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            Debug.Log("Clicked");
            
            RoadSelectTool roadSelectTool = ToolsModifierControl.SetTool<RoadSelectTool>();

            if(!roadSelectTool)
            {
                Debug.Log("Tool failed to initialise!");
            }
        }

        public override void OnLevelUnloading()
        {
        }

        public override void OnReleased()
        {
        }
    }
}
