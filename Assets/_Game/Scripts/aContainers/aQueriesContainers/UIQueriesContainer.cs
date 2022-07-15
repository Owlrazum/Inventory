using System;

namespace SNG.UI
{ 
    public static class UIQueriesContainer
    {
        public static Func<float> FuncSceneLoadingProgress;
        public static float QuerySceneLoadingProgress()
        { 
    #if UNITY_EDITOR
            if (FuncSceneLoadingProgress.GetInvocationList().Length != 1)
            {
                throw new NotSupportedException("There should be only one subscription");
            }
    #endif

            return FuncSceneLoadingProgress.Invoke();
        }

        public static Func<UIEventsUpdater> FuncGetUpdater;
        public static UIEventsUpdater QueryGetUpdater()
        {
#if UNITY_EDITOR
            if (FuncGetUpdater == null)
            {
                return null;
            }
            
            if (FuncGetUpdater.GetInvocationList().Length != 1)
            {
                throw new NotSupportedException("There should be only one subscription");
            }
    #endif

            return FuncGetUpdater.Invoke(); 
        }
    }
}