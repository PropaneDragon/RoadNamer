using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using RoadNamer.Managers;
using RoadNamer.Panels;
using RoadNamer.Tools;
using RoadNamer.Utilities;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer
{
    public class RoadNamerLoading : LoadingExtensionBase
    {
        private GameObject m_roadNamePanelObject;
        private RoadNamePanel m_roadNamePanel;

        public override void OnCreated(ILoading loading)
        {
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewMap || mode == LoadMode.LoadMap)
            {
                RoadNameManager.Load();
                LoadSprites();

                UIView view = UIView.GetAView();
                UITabstrip tabStrip = null;

                m_roadNamePanelObject = new GameObject("RoadNamePanel");
                m_roadNamePanel = m_roadNamePanelObject.AddComponent<RoadNamePanel>();
                m_roadNamePanel.transform.parent = view.transform;
                m_roadNamePanel.Hide();

                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
                {
                    tabStrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
                }
                else
                {
                    tabStrip = UIView.Find<UITabstrip>("MainToolstrip");
                }

                ToolController toolController = ToolsModifierControl.toolController;

                if (toolController.GetComponent<RoadSelectTool>() == null)
                {
                    ToolsModifierControl.toolController.gameObject.AddComponent<RoadSelectTool>();                    
                }

                GameObject buttonGameObject = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
                GameObject pageGameObject = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
                UIButton tabButton = tabStrip.AddTab("Road Namer", buttonGameObject, pageGameObject, new Type[] { }) as UIButton;

                UITextureAtlas atlas = SpriteUtilities.GetAtlas("RoadNamerIcons");

                tabButton.eventClicked += TabButton_eventClicked;
                tabButton.tooltip = "Road Namer";
                tabButton.foregroundSpriteMode = UIForegroundSpriteMode.Fill;

                if (atlas != null)
                {
                    tabButton.atlas = atlas;
                    tabButton.normalFgSprite = "ToolbarFGIcon";
                    tabButton.focusedFgSprite = "ToolbarFGIcon";
                    tabButton.hoveredFgSprite = "ToolbarFGIcon";
                    tabButton.disabledFgSprite = "ToolbarFGIcon";
                    tabButton.pressedFgSprite = "ToolbarFGIcon";
                    tabButton.focusedBgSprite = "ToolbarBGFocused";
                    tabButton.hoveredBgSprite = "ToolbarBGHovered";
                    tabButton.pressedBgSprite = "ToolbarBGPressed";
                }
                else
                {
                    Debug.LogError("Could not find atlas.");
                }

                RoadRenderingManager roadRenderingManager = Singleton<RoadRenderingManager>.instance;
                roadRenderingManager.enabled = true;
                RenderManager.RegisterRenderableManager(roadRenderingManager);
            }
        }

        private void LoadSprites()
        {
            bool atlasSuccess = SpriteUtilities.InitialiseAtlas("Icons/UIIcons.png", "RoadNamerIcons");

            if (atlasSuccess)
            {
                bool spriteSuccess = true;

                spriteSuccess = spriteSuccess && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 2), new Vector2(36, 36)), "ToolbarFGIcon", "RoadNamerIcons");
                spriteSuccess = spriteSuccess && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(40, 2), new Vector2(43, 49)), "ToolbarBGPressed", "RoadNamerIcons");
                spriteSuccess = spriteSuccess && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(85, 2), new Vector2(43, 49)), "ToolbarBGHovered", "RoadNamerIcons");
                spriteSuccess = spriteSuccess && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(130, 2), new Vector2(43, 49)), "ToolbarBGFocused", "RoadNamerIcons");

                if (!spriteSuccess)
                {
                    Debug.LogError("Failed to load some sprites!");
                }
            }
            else
            {
                Debug.LogError("Failed to load the atlas!");
            }
        }

        private void TabButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIView view = UIView.GetAView();
            RoadSelectTool roadSelectTool = ToolsModifierControl.SetTool<RoadSelectTool>();

            if (roadSelectTool == null)
            {
                Debug.Log("Tool failed to initialise!");
            }
            else
            {
                roadSelectTool.m_roadNamePanel = m_roadNamePanel;
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
