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
