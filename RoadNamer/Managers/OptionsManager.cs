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
    public class OptionsManager : MonoBehaviour
    {
        private static RoadCheckBoxOption[] checkboxOptions = new RoadCheckBoxOption[]
        {
            new RoadCheckBoxOption() { uniqueName = "showCamera", readableName = "Show road names in camera mode", value = false, enabled = true }
        };

        private static RoadSliderOption[] sliderOptions = new RoadSliderOption[]
        {
            new RoadSliderOption() { uniqueName = "textDisappearDistance", readableName = "Rendering distance", min = 100f, max = 2000f, value = 1000f, step = 10f, enabled = true },
            new RoadSliderOption() { uniqueName = "textScale", readableName = "Text scale", min = 0.2f, max = 2f, value = 0.6f, step = 0.1f, enabled = true }
        };

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

        public static void SaveOptions()
        {
            SavedOptionManager.Instance().SetCheckBoxOptions(checkboxOptions);
            SavedOptionManager.Instance().SetSliderOptions(sliderOptions);
            SavedOptionManager.SaveOptions();
        }

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

        public static void UpdateEverything()
        {
            RoadRenderingManager renderingManager = Singleton<RoadRenderingManager>.instance;

            if(renderingManager != null)
            {
                GetCheckBoxValue("showCamera", ref renderingManager.m_alwaysShowText);
                GetSliderValue("textDisappearDistance", ref renderingManager.m_renderHeight);
                GetSliderValue("textScale", ref renderingManager.m_textScale);
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
