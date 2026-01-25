namespace NFCure
{
    public delegate void NfcDataReceive(string uid, string ndefText);

    public static class NfcMessageHandler
    {

        public static string PrepareEncryptedMessage(ProgramRSA rsa, string message, long[] destinatairePublicKey)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "";

            // Chiffrer le message avec la clé publique du destinataire
            List<long> encryptedValues = rsa.EncryptString(message, destinatairePublicKey);

            // Convertir en string pour l'envoi NFC
            return string.Join(",", encryptedValues);
        }

 
        public static string ReceiveDecryptedMessage(ProgramRSA rsa, string encryptedMessage)
        {
            if (string.IsNullOrWhiteSpace(encryptedMessage))
                return "";

            // Créer la clé privée [n, d]
            long[] clePrivee = new long[] { rsa.n, rsa.d };

            // Déchiffrer le message
            return rsa.DecryptString(encryptedMessage, clePrivee);
        }
    }
}
