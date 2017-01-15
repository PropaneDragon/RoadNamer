using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Managers
{
    /// <summary>
    /// Handles all saving and loading from disk.
    /// </summary>
    [Serializable()]
    [XmlRoot(ElementName = "SavedOptions")]
    public class SavedOptionManager
    {
        private static SavedOptionManager instance = null;

        [XmlArray("CheckBoxOptions")]
        [XmlArrayItem("CheckBoxOption", typeof(StoredCheckBox))]
        public StoredCheckBox[] m_toggles = null;

        [XmlArray("StoredSliderOptions")]
        [XmlArrayItem("StoredSliderOption", typeof(StoredSlider))]
        public StoredSlider[] m_sliders = null;

        [XmlArray("StoredDropdownOptions")]
        [XmlArrayItem("StoredDropdownOption", typeof(StoredDropdown))]
        public StoredDropdown[] m_dropdowns = null;

        [XmlElement("SavedVersion", IsNullable = false)]
        public string m_lastSavedVersion = "";

        public static SavedOptionManager Instance()
        {
            if(instance == null)
            {
                instance = new SavedOptionManager();
            }

            return instance;
        }

        /// <summary>
        /// Change the instance used for the options.
        /// </summary>
        /// <param name="optionManager">The SavedOptionManager to replace the existing manager with.</param>
        public static void SetInstance(SavedOptionManager optionManager)
        {
            if(optionManager != null)
            {
                instance = optionManager;
            }
        }

        /// <summary>
        /// Saves all options to the disk. Make sure you've updated the options first.
        /// </summary>
        public static void SaveOptions()
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(SavedOptionManager));
            StreamWriter writer = new StreamWriter("RoadNameOptions.xml");

            xmlSerialiser.Serialize(writer, Instance());
            writer.Close();

            Debug.Log("Road Namer: Saved options file.");
        }

        /// <summary>
        /// Load all options from the disk.
        /// </summary>
        public static void LoadOptions()
        {
            if (File.Exists("RoadNameOptions.xml"))
            {
                XmlSerializer xmlSerialiser = new XmlSerializer(typeof(SavedOptionManager));
                StreamReader reader = new StreamReader("RoadNameOptions.xml");

                SavedOptionManager savedOptions = xmlSerialiser.Deserialize(reader) as SavedOptionManager;
                reader.Close();

                if(savedOptions != null)
                {
                    SetInstance(savedOptions);

                    Debug.Log("Road Namer: Loaded options file.");
                }
                else
                {
                    Debug.LogError("Road Namer: Created options class is invalid!");
                }
            }
            else
            {
                Debug.LogWarning("Road Namer: Could not load the options file!");
            }
        }

        /// <summary>
        /// Sets all internal checkbox option values for saving.
        /// </summary>
        /// <param name="options">Checkbox options which are going to be saved to disk</param>
        public void SetCheckBoxOptions(RoadCheckBoxOption[] options)
        {
            m_toggles = new StoredCheckBox[options.Length];

            for(int index = 0; index < options.Length; ++index)
            {
                m_toggles[index] = new StoredCheckBox()
                {
                    linkedOption = options[index].uniqueName,
                    data = options[index].value
                };
            }
        }

        /// <summary>
        /// Sets all internal slider option values for saving.
        /// </summary>
        /// <param name="options">Slider options which are going to be saved to disk</param>
        public void SetSliderOptions(RoadSliderOption[] options)
        {
            m_sliders = new StoredSlider[options.Length];

            for (int index = 0; index < options.Length; ++index)
            {
                m_sliders[index] = new StoredSlider()
                {
                    linkedOption = options[index].uniqueName,
                    data = options[index].value
                };
            }
        }

        /// <summary>
        /// Sets all internal slider option values for saving.
        /// </summary>
        /// <param name="options">Slider options which are going to be saved to disk</param>
        public void SetDropdownOptions(RoadDropdownOption[] options)
        {
            m_dropdowns = new StoredDropdown[options.Length];

            for (int index = 0; index < options.Length; ++index)
            {
                m_dropdowns[index] = new StoredDropdown()
                {
                    linkedOption = options[index].uniqueName,
                    data = options[index].value
                };
            }
        }
    }

    /// <summary>
    /// All checkbox data that gets saved to XML
    /// </summary>
    [Serializable()]
    public class StoredCheckBox
    {
        [XmlElement("LinkedOption", IsNullable = false)]
        public string linkedOption = "";

        [XmlElement("OptionData", IsNullable = false)]
        public bool data = false;
    }

    /// <summary>
    /// All slider data that gets saved to XML
    /// </summary>
    [Serializable()]
    public class StoredSlider
    {
        [XmlElement("LinkedOption", IsNullable = false)]
        public string linkedOption = "";

        [XmlElement("OptionData", IsNullable = false)]
        public float data = .0f;
    }

    /// <summary>
    /// All slider data that gets saved to XML
    /// </summary>
    [Serializable()]
    public class StoredDropdown
    {
        [XmlElement("LinkedOption", IsNullable = false)]
        public string linkedOption = "";

        [XmlElement("OptionData", IsNullable = false)]
        public int data = 0;
    }
}
