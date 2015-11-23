using ICities;
using RoadNamer.Managers;
using UnityEngine;

namespace RoadNamer
{
    public class RoadNamerMod : IUserMod
    {
        private OptionsManager m_optionsManager = null;

        public string Name
        {
            get
            {
                return "Road Namer";
            }
        }

        public string Description
        {
            get
            {
                return "Allows you to name any of the roads in your city.";
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            if (m_optionsManager == null)
            {
                m_optionsManager = new GameObject("RoadNamerOptions").AddComponent<OptionsManager>();
            }

            OptionsManager.LoadOptions();
            m_optionsManager.CreateOptions(helper);
        }
    }
}
