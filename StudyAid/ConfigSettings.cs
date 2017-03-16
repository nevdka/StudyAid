namespace StudyAid
{
    public class ConfigSettings
    {
        public string ImageFileName { get; set; }
        public string TextFileName { get; set; }
        public string GoalTime { get; set; }
        public string PopUpTime { get; set; }
        public bool StudyMode { get; set; }

        public bool UsePercentages { get; set; }
        public bool UseRandomText { get; set; }

        public int GoalTimeMins { get; set; }
        public int PopUpTimeMins { get; set; }
    }
}
