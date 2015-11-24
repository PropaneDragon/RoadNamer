using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace RoadNamer.Managers
{
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

        public static SavedOptionManager Instance()
        {
            if(instance == null)
            {
                instance = new SavedOptionManager();
            }

            return instance;
        }

        public static void SetInstance(SavedOptionManager optionManager)
        {
            if(optionManager != null)
            {
                instance = optionManager;
            }
        }

        public static void SaveOptions()
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(SavedOptionManager));
            StreamWriter writer = new StreamWriter("RoadNameOptions.xml");

            xmlSerialiser.Serialize(writer, Instance());
            writer.Close();
        }

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
    }

    [Serializable()]
    public class StoredCheckBox
    {
        [XmlElement("LinkedOption", IsNullable = false)]
        public string linkedOption = "";

        [XmlElement("OptionData", IsNullable = false)]
        public bool data = false;
    }

    [Serializable()]
    public class StoredSlider
    {
        [XmlElement("LinkedOption", IsNullable = false)]
        public string linkedOption = "";

        [XmlElement("OptionData", IsNullable = false)]
        public float data = .0f;
    }
}
