namespace NFCure
{
    public static class RfidToolsGetter
    {
        #if ANDROID
        private static RfidTools GetTools()
        {
            var act = (MainActivity)Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            return act.RfidTools;
        }
        #endif

        public static void SetOnTagRead(NfcDataReceive cb)
        {
            #if ANDROID
            GetTools().OnTagRead = cb;
            #endif
        }

        public static void SetOffTagRead()
        {
            #if ANDROID
            GetTools().OnTagRead = null;
#endif
        }
    }
}
