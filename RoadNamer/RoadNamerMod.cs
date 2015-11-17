using ICities;

namespace RoadNamer
{
    public class RoadNamerMod : IUserMod
    {
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
                return "Allows you to name roads";
            }
        }
    }
}
