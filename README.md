# NFCure – messages chiffrés d'un téléphone à l'autre par NFC

Application mobile qui permet d'envoyer un court message **chiffré** à une autre personne en **approchant simplement les deux téléphones**, sans réseau ni Internet : le message passe par le NFC et seul le destinataire peut le déchiffrer.

Projet d'équipe réalisé en janvier 2026, pendant le BUT Informatique.

## Fonctionnement

1. Le **destinataire** ouvre la page *Recevoir* : l'application génère sa paire de clés RSA et affiche sa **clé publique** (`n`, `e`).
2. L'**expéditeur** saisit cette clé publique et son message dans la page *Envoyer* : le message est **chiffré** avec la clé du destinataire.
3. Le téléphone de l'expéditeur **se fait passer pour une étiquette NFC** (émulation de carte, *Host Card Emulation*) qui contient le message chiffré.
4. Le destinataire approche son téléphone : il lit l'étiquette et **déchiffre** le message avec sa clé privée, qui n'a jamais quitté son appareil.

## Points techniques

- **RSA implémenté à la main** : génération de nombres premiers, PGCD, inverse modulaire, exponentiation modulaire rapide, chiffrement et déchiffrement d'un texte.
- **Émulation d'une étiquette NFC Type 4** sous Android : un service répond aux commandes APDU du lecteur (sélection de l'application, des fichiers, lecture binaire) et expose le message au format NDEF.
- Lecture NFC côté destinataire et affichage du message déchiffré.
- Interface multiplateforme en .NET MAUI.

> **Limite assumée** : les clés RSA reposent sur des entiers 64 bits. Le but était de comprendre et d'implémenter l'algorithme ; ce chiffrement n'offre pas une sécurité réelle. Une version sérieuse utiliserait `System.Security.Cryptography` avec des clés de 2048 bits ou plus, et un échange de clé publique automatisé (QR code ou NFC) plutôt qu'une saisie manuelle.

## Technologies

- C# / .NET 10, .NET MAUI
- Android : NFC, `HostApduService` (HCE), NDEF

## Mon rôle

J'ai créé le projet, conçu l'**interface** (pages d'accueil, d'envoi et de réception, icône et écran de démarrage) et **relié l'interface à l'envoi et à la réception NFC** des messages chiffrés. La partie RSA a été réalisée avec mes coéquipiers.

## Lancer le projet

Prérequis : le [SDK .NET 10](https://dotnet.microsoft.com/download) avec la charge de travail MAUI, et **deux téléphones Android équipés du NFC**.

```powershell
dotnet workload install maui-android
git clone https://github.com/MathisLABBE/NFCure.git
cd NFCure
dotnet build NFCure -f net10.0-android -t:Run
```

Ou ouvrir `NFCure.slnx` dans Visual Studio 2026 et déployer sur un téléphone branché en USB (mode développeur activé).

## Auteur

Mathis LABBE – étudiant en BUT Informatique en alternance · [GitHub](https://github.com/MathisLABBE) · [LinkedIn](https://www.linkedin.com/in/mathis-labbe-92518131b/)
