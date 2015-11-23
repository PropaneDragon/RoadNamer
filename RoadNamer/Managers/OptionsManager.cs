using ColossalFramework.UI;
using ICities;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using ColossalFramework;

namespace RoadNamer.Managers
{
    /// <summary>
    /// Manages all ingame options. Handles the interface between ingame options
    /// storage and saving/loading from disk.
    /// </summary>
    public class OptionsManager : MonoBehaviour
    {
        public static bool m_isIngame = false;

        /// <summary>
        /// Contains all options that can be set using a checkbox. These
        /// options automatically generate a checkbox in the options panel
        /// </summary>
        private static RoadCheckBoxOption[] checkboxOptions = new RoadCheckBoxOption[]
        {
            new RoadCheckBoxOption() { uniqueName = "showCamera", readableName = "Show road names in camera mode", value = false, enabled = true }
        };

        /// <summary>
        /// Contains all options that can be set using a slider. These
        /// options automatically generate a slider in the options panel
        /// </summary>
        private static RoadSliderOption[] sliderOptions = new RoadSliderOption[]
        {
            new RoadSliderOption() { uniqueName = "textDisappearDistance", readableName = "Rendering distance", min = 100f, max = 2000f, value = 1000f, step = 10f, enabled = true },
            new RoadSliderOption() { uniqueName = "textScale", readableName = "Text scale", min = 0.2f, max = 2f, value = 0.5f, step = 0.1f, enabled = true }
        };

        /// <summary>
        /// Creates options on a panel using the helper
        /// </summary>
        /// <param name="helper">The UIHelper to put the options on</param>
        public void CreateOptions(UIHelperBase helper)
        {
            UIHelperBase optionGroup = helper.AddGroup("Road Namer Options");

            foreach(RoadCheckBoxOption checkboxOption in checkboxOptions)
            {
                UICheckBox checkBox = optionGroup.AddCheckbox(checkboxOption.readableName, checkboxOption.value, OptionChanged) as UICheckBox;
                checkBox.readOnly = !checkboxOption.enabled;
                checkBox.name = checkboxOption.uniqueName;
                checkBox.eventCheckChanged += CheckBox_eventCheckChanged;
            }

            foreach(RoadSliderOption sliderOption in sliderOptions)
            {
                UISlider slider = optionGroup.AddSlider(sliderOption.readableName, sliderOption.min, sliderOption.max, sliderOption.step, sliderOption.value, OptionChanged) as UISlider;
                slider.enabled = sliderOption.enabled;
                slider.name = sliderOption.uniqueName;
                slider.eventValueChanged += Slider_eventValueChanged;
                slider.tooltip = sliderOption.value.ToString();
            }

            UIButton saveButton = optionGroup.AddButton("Apply", SaveButtonClicked) as UIButton;
        }

        private void Slider_eventValueChanged(UIComponent component, float value)
        {
            UISlider slider = component as UISlider;

            if (slider != null) //Should bloody well not be null!
            {
                RoadSliderOption foundOption = null;

                foreach (RoadSliderOption option in sliderOptions)
                {
                    if (slider.name == option.uniqueName)
                    {
                        foundOption = option;
                    }
                }

                if (foundOption != null)
                {
                    foundOption.value = value;
                    slider.tooltip = value.ToString();
                    slider.RefreshTooltip();
                }
            }
        }

        private void CheckBox_eventCheckChanged(UIComponent component, bool value)
        {
            UICheckBox checkBox = component as UICheckBox;

            if(checkBox != null) //Should bloody well not be null!
            {
                RoadCheckBoxOption foundOption = null;

                foreach(RoadCheckBoxOption option in checkboxOptions)
                {
                    if(checkBox.name == option.uniqueName)
                    {
                        foundOption = option;
                    }
                }

                if(foundOption != null)
                {
                    foundOption.value = value;
                }
            }
        }

        private void OptionChanged(bool value)
        {
        }

        private void OptionChanged(float value)
        {
        }

        private void SaveButtonClicked()
        {
            SaveOptions();
            UpdateEverything();
        }

        /// <summary>
        /// Gets a bool from an option that was set using a checkbox
        /// </summary>
        /// <param name="uniqueName">The unique name of the checkbox option</param>
        /// <param name="returnValue">The value to replace</param>
        /// <returns>Whether the value was found and set</returns>
        public static bool GetCheckBoxValue(string uniqueName, ref bool returnValue)
        {
            bool successful = false;

            foreach(RoadCheckBoxOption option in checkboxOptions)
            {
                if(option.uniqueName == uniqueName)
                {
                    returnValue = option.value;
                    successful = true;
                }
            }

            return successful;
        }

        /// <summary>
        /// Gets a float from an option that was set using a slider
        /// </summary>
        /// <param name="uniqueName">The unique name of the slider option</param>
        /// <param name="returnValue">The value to replace</param>
        /// <returns>Whether the value was found and set</returns>
        public static bool GetSliderValue(string uniqueName, ref float returnValue)
        {
            bool successful = false;

            foreach (RoadSliderOption option in sliderOptions)
            {
                if (option.uniqueName == uniqueName)
                {
                    Debug.Log(option.value);
                    returnValue = option.value;
                    successful = true;
                }
            }

            return successful;
        }

        /// <summary>
        /// Save options to disk
        /// </summary>
        public static void SaveOptions()
        {
            SavedOptionManager.Instance().SetCheckBoxOptions(checkboxOptions);
            SavedOptionManager.Instance().SetSliderOptions(sliderOptions);
            SavedOptionManager.SaveOptions();
        }

        /// <summary>
        /// Load options from disk and replace default values
        /// with stored ones.
        /// </summary>
        public static void LoadOptions()
        {
            SavedOptionManager.LoadOptions();

            if (SavedOptionManager.Instance().m_toggles != null)
            {
                foreach (StoredCheckBox storedCheckBox in SavedOptionManager.Instance().m_toggles)
                {
                    foreach (RoadCheckBoxOption option in checkboxOptions)
                    {
                        if (option.uniqueName == storedCheckBox.linkedOption)
                        {
                            option.value = storedCheckBox.data;
                        }
                    }
                }
            }

            if (SavedOptionManager.Instance().m_sliders != null)
            {
                foreach (StoredSlider storedSlider in SavedOptionManager.Instance().m_sliders)
                {
                    foreach (RoadSliderOption option in sliderOptions)
                    {
                        if (option.uniqueName == storedSlider.linkedOption)
                        {
                            if (storedSlider.data <= option.max && storedSlider.data >= option.min)
                            {
                                option.value = storedSlider.data;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates everything ingame. Should be used when one of the options has updated
        /// and ingame elements need immediately refreshing.
        /// </summary>
        public static void UpdateEverything()
        {
            if (m_isIngame)
            {
                RoadRenderingManager renderingManager = Singleton<RoadRenderingManager>.instance;

                if (renderingManager != null)
                {
                    GetCheckBoxValue("showCamera", ref renderingManager.m_alwaysShowText);
                    GetSliderValue("textDisappearDistance", ref renderingManager.m_renderHeight);
                    GetSliderValue("textScale", ref renderingManager.m_textScale);

                    renderingManager.ForceUpdate();
                }
            }
        }
    }

    public class RoadCheckBoxOption
    {
        public string uniqueName = "";
        public string readableName = "";
        public bool value = false;
        public bool enabled = false;
    }

    public class RoadSliderOption
    {
        public string uniqueName = "";
        public string readableName = "";
        public float min = 0f;
        public float max = 1f;
        public float value = 0f;
        public float step = 1f;
        public bool enabled = false;
    }
}
