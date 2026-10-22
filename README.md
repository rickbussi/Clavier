# Clavier Lenovo T14 Gen 1

Application de bureau .NET 10 / Avalonia pour afficher l’image du clavier Lenovo ThinkPad T14 Gen 1. Le même projet cible Windows et Linux.

## Prérequis

- SDK .NET 10
- Accès à NuGet lors de la première restauration des dépendances
- Sous Linux, les bibliothèques natives nécessaires à Avalonia doivent être présentes sur la distribution

## Lancer l’application

Depuis ce répertoire :

```sh
dotnet restore Clavier.slnx
dotnet run --project Clavier.App/Clavier.App.csproj
```

L’image est incluse dans `Clavier.App/Assets/clavier-lenovo-t14-gen1.jpg` et intégrée comme ressource de l’application.

## Publier et installer sous Linux x64

La publication se fait sur Windows avec le SDK .NET 10. Depuis PowerShell, à la racine du projet, exécutez :

```powershell
dotnet publish .\Clavier.App\Clavier.App.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\publish\linux-x64
```

Cette commande crée `publish/linux-x64`. Copiez tout le contenu de ce dossier vers un dossier sur votre machine Linux. Il contient le binaire `Clavier`, le script `installer-linux.sh` et l’icône `keyboard-icon.png`.

Le script source est `Clavier.App/Installer/Linux/installer-linux.sh`.

Sur Linux, ouvrez un terminal dans le dossier copié et lancez l’installation :

```bash
chmod +x installer-linux.sh
./installer-linux.sh
```

Le script installe Clavier dans `~/.local/opt/clavier`, ajoute son icône au menu des applications et crée le lanceur. L’installation ne demande pas les droits administrateur. Le lanceur du menu supprime les variables d’environnement de l’IME Linux pour contourner un problème Avalonia/IBus avec les accents composés. La publication autonome regroupe l’application et le runtime .NET dans un seul binaire ; le script et l’icône doivent rester à côté du binaire jusqu’à l’installation.
