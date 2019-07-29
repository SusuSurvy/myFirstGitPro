namespace RobotAvatar.Commom.Event
{
    static public class BattleEvent
    {
        public readonly static string LoadComplete = "BattleEvent.LoadComplete";
        public readonly static string GameWin = "BattleEvent.GameWin";
        public readonly static string GameLose = "BattleEvent.GameLose";
        public readonly static string HideButton = "BattleEvent.HideButton";
        public readonly static string PlayerBlood = "BattleEvent.PlayerBlood";
        public readonly static string EnemyBlood = "BattleEvent.EnemyBlood";
    }

    static public class EduEvent
    {
        public readonly static string ShowEdu = "EduEvent.ShowEDU";
        public readonly static string Edu = "EduEvent.EDU";
        public readonly static string EduOver = "EduEvent.EduOver";
    }

    static public class GuideEvent
    {
        public readonly static string Start = "GuideEvent.Start";
        public readonly static string Over = "GuideEvent.Over";
    }

    static public class SystemEvent
    {
        public readonly static string SaveData = "SystemEvent.SaveData";
        
    }

}