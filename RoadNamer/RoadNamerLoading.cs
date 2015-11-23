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
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer
{
    public class RoadNamerLoading : LoadingExtensionBase
    {
        private GameObject m_roadNamePanelObject;
        private GameObject m_usedNamesPanelObject;
        private RoadNamePanel m_roadNamePanel;
        private UsedNamesPanel m_usedNamesPanel;
    
        private RoadNamerSerialiser m_saveUtility = new RoadNamerSerialiser();
        private UIButton m_tabButton = null;
        private RoadRenderingManager m_roadRenderingManager = null;

        public override void OnCreated(ILoading loading)
        {
            try //So we don't fuck up loading the city
            {
                LoadSprites();
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewMap || mode == LoadMode.LoadMap)
            {
                UIView view = UIView.GetAView();
                UITabstrip tabStrip = null;

                m_roadNamePanelObject = new GameObject("RoadNamePanel");
                m_roadNamePanel = m_roadNamePanelObject.AddComponent<RoadNamePanel>();
                m_roadNamePanel.transform.parent = view.transform;
                m_roadNamePanel.Hide();

                m_usedNamesPanelObject = new GameObject("UsedNamesPanel");
                m_usedNamesPanel = m_usedNamesPanelObject.AddComponent<UsedNamesPanel>();
                m_usedNamesPanel.transform.parent = view.transform;
                m_usedNamesPanel.Hide();

                EventBusManager.Instance().Subscribe("forceupdateroadnames", m_usedNamesPanel);
                EventBusManager.Instance().Subscribe("updateroadnamepaneltext", m_roadNamePanel);

                RandomNameConfiguration.Load();

                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
                {
                    tabStrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
                }
                else
                {
                    tabStrip = UIView.Find<UITabstrip>("MainToolstrip");
                }

                if (m_tabButton == null)
                {
                    GameObject buttonGameObject = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
                    GameObject pageGameObject = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
                    m_tabButton = tabStrip.AddTab("Road Namer", buttonGameObject, pageGameObject, new Type[] { }) as UIButton;

                    UITextureAtlas atlas = SpriteUtilities.GetAtlas("RoadNamerIcons");

                    m_tabButton.eventClicked += TabButton_eventClicked;
                    m_tabButton.tooltip = "Road Namer";
                    m_tabButton.foregroundSpriteMode = UIForegroundSpriteMode.Fill;

                    if (atlas != null)
                    {
                        m_tabButton.atlas = atlas;
                        m_tabButton.normalFgSprite = "ToolbarFGIcon";
                        m_tabButton.focusedFgSprite = "ToolbarFGIcon";
                        m_tabButton.hoveredFgSprite = "ToolbarFGIcon";
                        m_tabButton.disabledFgSprite = "ToolbarFGIcon";
                        m_tabButton.pressedFgSprite = "ToolbarFGIcon";
                        m_tabButton.focusedBgSprite = "ToolbarBGFocused";
                        m_tabButton.hoveredBgSprite = "ToolbarBGHovered";
                        m_tabButton.pressedBgSprite = "ToolbarBGPressed";
                    }
                    else
                    {
                        Debug.LogError("Road Namer: Could not find atlas.");
                    }
                }

                m_roadRenderingManager = Singleton<RoadRenderingManager>.instance;
                m_roadRenderingManager.enabled = true;

                if (m_roadRenderingManager != null && !m_roadRenderingManager.m_registered)
                {
                    RenderManager.RegisterRenderableManager(m_roadRenderingManager);
                    m_roadRenderingManager.m_registered = true;
                }
            }
        }

        private void LoadSprites()
        {
            bool atlasSuccess = SpriteUtilities.InitialiseAtlas("Icons/UIIcons.png", "RoadNamerIcons");

            if (atlasSuccess)
            {
                bool spriteSuccess = true;

                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 2), new Vector2(36, 36)), "ToolbarFGIcon", "RoadNamerIcons") && spriteSuccess;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(40, 2), new Vector2(43, 49)), "ToolbarBGPressed", "RoadNamerIcons") && spriteSuccess;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(85, 2), new Vector2(43, 49)), "ToolbarBGHovered", "RoadNamerIcons") && spriteSuccess;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(130, 2), new Vector2(43, 49)), "ToolbarBGFocused", "RoadNamerIcons") && spriteSuccess;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 53), new Vector2(42, 42)), "DiceIcon", "RoadNamerIcons") && spriteSuccess;

                if (!spriteSuccess)
                {
                    Debug.LogError("Road Namer: Failed to load some sprites!");
                }
            }
            else
            {
                Debug.LogError("Road Namer: Failed to load the atlas!");
            }
        }

        private void TabButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            ToolController toolController = ToolsModifierControl.toolController;

            if (toolController != null)
            {
                if (toolController.GetComponent<RoadSelectTool>() == null)
                {
                    ToolsModifierControl.toolController.gameObject.AddComponent<RoadSelectTool>();

                    //I stole this from Traffic++ for now, until I can figure some things out. Quick fix!
                    FieldInfo toolControllerField = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (toolControllerField != null)
                        toolControllerField.SetValue(toolController, toolController.GetComponents<ToolBase>());
                    FieldInfo toolModifierDictionary = typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic);
                    if (toolModifierDictionary != null)
                        toolModifierDictionary.SetValue(null, null); // to force a refresh
                }
                
                RoadSelectTool roadSelectTool = ToolsModifierControl.SetTool<RoadSelectTool>();

                if (roadSelectTool == null)
                {
                    Debug.Log("Tool failed to initialise!");
                }
                else
                {
                    roadSelectTool.m_roadNamePanel = m_roadNamePanel;
                    roadSelectTool.m_usedNamesPanel = m_usedNamesPanel;
                }
            }
        }

        public override void OnLevelUnloading()
        {
            EventBusManager.Instance().UnSubscribe("forceUpdate", m_usedNamesPanel);
            EventBusManager.Instance().UnSubscribe("updateText", m_roadNamePanel);
        }

        public override void OnReleased()
        {
        }
    }
}
