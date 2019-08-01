namespace Lobby3D.Commom.Event
{
    static public class GameEvent
    {
        public readonly static string PauseAnimal = "GameEvent.PauseAnimal";
        public readonly static string ResumeAnimal = "GameEvent.ResumeAnimal";
    }

    static public class EduEvent
    {
        public readonly static string ShowEdu = "EduEvent.ShowEDU";
        public readonly static string Edu = "EduEvent.EDU";
    }

    static public class SystemEvent
    {
        public readonly static string SaveData = "SystemEvent.SaveData";

    }

    static public class GuideEvent
    {
        public readonly static string ForceGuide = "GuideEvent.ForceGuide";
        public readonly static string UnlockFailed = "GuideEvent.TriggerWaiteTouchNpc";
        public readonly static string StopWaiteTouchNpc = "GuideEvent.StopWaiteTouchNpc";
        public readonly static string UnlockSuccess = "GuideEvent.UnlockSuccess";
        public readonly static string PlayTimelineFinish = "GuideEvent.PlayTimelineFinish";
        public readonly static string EnterFirstGame = "GuideEvent.EnterFirstGame";
        public readonly static string PlayGuideVideo = "GuideEvent.PlayGuideVideo";
    }

}