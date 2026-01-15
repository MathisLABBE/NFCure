using Android.App;
using Android.Content;
using Android.Nfc.CardEmulators;
using Android.OS;
using System.Text;

namespace NFCure.Platforms.Android;

// IMPORTANT: Exported = true pour que le système puisse l'appeler
[Service(Exported = true, Permission = "android.permission.BIND_NFC_SERVICE")]
[IntentFilter(new[] { "android.nfc.cardemulation.action.HOST_APDU_SERVICE" })]
[MetaData("android.nfc.cardemulation.host_apdu_service", Resource = "@xml/apduservice")]
public class NfcHceService : HostApduService
{
    // Status word OK
    static readonly byte[] SW_OK = { 0x90, 0x00 };
    static readonly byte[] SW_UNKNOWN = { 0x6F, 0x00 };

    // SELECT AID = 00 A4 04 00 <Lc> <AID> 00
    static readonly byte[] SELECT_AID_PREFIX = { 0x00, 0xA4, 0x04, 0x00 };

    // SELECT FILE = 00 A4 00 0C 02 <fileIdHi> <fileIdLo>
    static readonly byte[] SELECT_FILE_PREFIX = { 0x00, 0xA4, 0x00, 0x0C, 0x02 };

    // READ BINARY = 00 B0 <offsetHi> <offsetLo> <Le>
    static readonly byte[] READ_BINARY_PREFIX = { 0x00, 0xB0 };

    // Fichiers Type 4
    const ushort CC_FILE_ID = 0xE103;
    const ushort NDEF_FILE_ID = 0xE104;

    ushort _selectedFile = 0;

    // CC file minimal (déclare un NDEF file E104)
    // (format simple et compatible avec beaucoup de lecteurs)
    static readonly byte[] CC_FILE =
    {
        0x00, 0x0F,       // CCLEN = 15
        0x20,             // Mapping Version 2.0
        0x00, 0x3B,       // MLe (max read) ~59
        0x00, 0x34,       // MLc (max write) ~52
        0x04,             // NDEF File Control TLV (type)
        0x06,             // length
        0xE1, 0x04,       // File ID = E104
        0x00, 0xFF,       // Max NDEF size (255 ici)
        0x00,             // Read access
        0x00              // Write access
    };

    // Message partagé (tu le mettras depuis ton UI)
    public static string CurrentMessage { get; set; } = "Hello NFC";

    public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
    {
        try
        {
            // 1) SELECT AID (NDEF Tag Application)
            if (IsSelectAid(commandApdu))
            {
                _selectedFile = 0;
                return Concat(SW_OK);
            }

            // 2) SELECT FILE (CC / NDEF)
            if (IsSelectFile(commandApdu, out ushort fileId))
            {
                _selectedFile = fileId;
                return Concat(SW_OK);
            }

            // 3) READ BINARY
            if (IsReadBinary(commandApdu, out int offset, out int le))
            {
                var data = GetFileBytes(_selectedFile);
                if (data == null) return Concat(SW_UNKNOWN);

                var slice = Slice(data, offset, le);
                return Concat(slice, SW_OK);
            }

            return Concat(SW_UNKNOWN);
        }
        catch
        {
            return Concat(SW_UNKNOWN);
        }
    }

    public override void OnDeactivated(DeactivationReason reason)
    {
        _selectedFile = 0;
    }

    // ---- Helpers ----

    static bool IsSelectAid(byte[] apdu)
    {
        if (apdu.Length < 10) return false;
        for (int i = 0; i < SELECT_AID_PREFIX.Length; i++)
            if (apdu[i] != SELECT_AID_PREFIX[i]) return false;
        return true;
    }

    static bool IsSelectFile(byte[] apdu, out ushort fileId)
    {
        fileId = 0;
        if (apdu.Length < 7) return false;

        for (int i = 0; i < SELECT_FILE_PREFIX.Length; i++)
            if (apdu[i] != SELECT_FILE_PREFIX[i]) return false;

        fileId = (ushort)((apdu[5] << 8) | apdu[6]);
        return true;
    }

    static bool IsReadBinary(byte[] apdu, out int offset, out int le)
    {
        offset = 0; le = 0;
        if (apdu.Length < 5) return false;

        if (apdu[0] != READ_BINARY_PREFIX[0] || apdu[1] != READ_BINARY_PREFIX[1])
            return false;

        offset = (apdu[2] << 8) | apdu[3];
        le = apdu[4];
        if (le == 0) le = 256; // convention
        return true;
    }

    static byte[]? GetFileBytes(ushort fileId)
    {
        if (fileId == CC_FILE_ID) return CC_FILE;

        if (fileId == NDEF_FILE_ID)
        {
            // Construire un NDEF Text Record
            var ndef = BuildNdefTextMessage(CurrentMessage);

            // Type 4 : NDEF file = [NLEN(2 bytes)] + NDEF Message
            ushort nlen = (ushort)ndef.Length;
            var file = new byte[2 + ndef.Length];
            file[0] = (byte)((nlen >> 8) & 0xFF);
            file[1] = (byte)(nlen & 0xFF);
            Buffer.BlockCopy(ndef, 0, file, 2, ndef.Length);
            return file;
        }

        return null;
    }

    static byte[] BuildNdefTextMessage(string text)
    {
        // NDEF Message = [Record...]
        // Record Text (TNF Well-known, type "T")
        var lang = "en";
        var langBytes = Encoding.ASCII.GetBytes(lang);
        var textBytes = Encoding.UTF8.GetBytes(text);

        var payload = new byte[1 + langBytes.Length + textBytes.Length];
        payload[0] = (byte)langBytes.Length; // UTF-8 + lang length
        Buffer.BlockCopy(langBytes, 0, payload, 1, langBytes.Length);
        Buffer.BlockCopy(textBytes, 0, payload, 1 + langBytes.Length, textBytes.Length);

        // Record header (MB/ME/SR = 1, TNF=0x01)
        // 0xD1 = MB(1) ME(1) CF(0) SR(1) IL(0) TNF(001)
        byte[] record =
        {
            0xD1,       // header
            0x01,       // type length
            (byte)payload.Length, // payload length (SR => 1 byte)
            0x54        // 'T'
        };

        var ndef = new byte[record.Length + payload.Length];
        Buffer.BlockCopy(record, 0, ndef, 0, record.Length);
        Buffer.BlockCopy(payload, 0, ndef, record.Length, payload.Length);
        return ndef;
    }

    static byte[] Slice(byte[] data, int offset, int length)
    {
        if (offset < 0) offset = 0;
        if (offset >= data.Length) return Array.Empty<byte>();

        int max = Math.Min(length, data.Length - offset);
        var res = new byte[max];
        Buffer.BlockCopy(data, offset, res, 0, max);
        return res;
    }

    static byte[] Concat(params byte[][] arrays)
    {
        int len = 0;
        foreach (var a in arrays) len += a.Length;

        var res = new byte[len];
        int pos = 0;
        foreach (var a in arrays)
        {
            Buffer.BlockCopy(a, 0, res, pos, a.Length);
            pos += a.Length;
        }
        return res;
    }
}
