using System;
using System.Text;

using Android.Content;
using Android.Nfc;
using Android.OS;
using Java.Lang;

namespace NFCure.Platforms.Android
{
    public static class RfidSimple
    {
        public static string GetUid(Intent intent)
        {
            try
            {
                // 1) Le plus fiable : ExtraId
                var id = intent.GetByteArrayExtra(NfcAdapter.ExtraId);
                if (id != null && id.Length > 0)
                    return BitConverter.ToString(id).Replace("-", "");

                // 2) Sinon : ExtraTag (Tag)
                var tag = GetTagCompat(intent);
                if (tag == null) return "";

                var tagId = tag.GetId();
                return (tagId != null && tagId.Length > 0)
                    ? BitConverter.ToString(tagId).Replace("-", "")
                    : "";
            }
            catch
            {
                return "";
            }
        }

        public static string GetNdefText(Intent intent)
        {
            try
            {
                var raw = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                if (raw == null) return "";

                foreach (var p in raw)
                {
                    var msg = (NdefMessage)p;
                    var records = msg.GetRecords();
                    if (records == null) continue;

                    foreach (var r in records)
                    {
                        var payload = r.GetPayload();
                        if (payload == null || payload.Length < 2) continue;

                        int langLen = payload[0] & 0x3F;
                        int textStart = 1 + langLen;
                        if (textStart >= payload.Length) continue;

                        return Encoding.UTF8.GetString(payload, textStart, payload.Length - textStart);
                    }
                }
            }
            catch { }

            return "";
        }

        private static Tag? GetTagCompat(Intent intent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                return (Tag?)intent.GetParcelableExtra(
                    NfcAdapter.ExtraTag,
                    Class.FromType(typeof(Tag))
                );
            }

#pragma warning disable CS0618
            return (Tag?)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
#pragma warning restore CS0618
        }
    }
}
