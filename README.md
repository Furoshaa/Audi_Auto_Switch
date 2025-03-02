# Audio Auto Switch Service

Un service Windows qui synchronise automatiquement le périphérique audio de communication avec le périphérique multimédia par défaut.

## Problème résolu

Quand vous changez de périphérique audio sous Windows (par exemple du casque aux haut-parleurs), Windows ne synchronise pas automatiquement le périphérique de communication. Ce service le fait pour vous.

### Cas d'utilisation
- Vous branchez votre casque : tout bascule dessus (Spotify ET Discord)
- Vous débranchez votre casque : tout revient sur les haut-parleurs
- Plus besoin d'aller dans les paramètres Windows Sound

## Installation

1. Téléchargez le fichier `.exe` depuis les releases
2. Ouvrez PowerShell en administrateur :
```powershell
cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
.\InstallUtil.exe "chemin\vers\Audi_Auto_Switch.exe"
```

## Désinstallation

En PowerShell administrateur :
```powershell
cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
.\InstallUtil.exe /u "chemin\vers\Audi_Auto_Switch.exe"
```

## Fonctionnement technique

- Utilise NAudio pour détecter les changements de périphériques
- S'exécute en tant que service Windows
- Démarre automatiquement avec Windows
- Zéro configuration requise
- Pas d'interface graphique (service en arrière-plan)

## Prérequis

- Windows 10/11
- .NET Framework 4.7.2+
- Droits administrateur pour l'installation

## Logs

En cas de problème, consultez l'Observateur d'événements Windows sous "Audio Device Sync Service"
