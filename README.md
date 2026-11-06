# Clavier Lenovo T14 Gen 1

Application Angular utilisable dans un navigateur et dans une fenêtre Electron. Elle montre le clavier Lenovo ThinkPad T14 Gen 1 et les combinaisons à utiliser pour saisir des caractères accentués.

## Prérequis

- Node.js 24 et npm
- La publication Linux produit une archive portable `tar.gz`, constructible depuis Windows ou Linux

Les commandes se lancent depuis la racine du projet.

## Navigateur

```powershell
npm.cmd ci
npm.cmd start
```

Ouvrir l’adresse affichée par Angular, généralement `http://localhost:4200`.

Pour construire la version Web :

```powershell
npm.cmd run build
```

Le site généré se trouve dans `dist/clavier-web/browser/`.

## Application de bureau

Pour lancer la fenêtre Electron en développement de production :

```powershell
npm.cmd ci
npm.cmd run desktop
```

## Installateurs

Sous Windows :

```powershell
npm.cmd ci
npm.cmd run dist:win
```

L’installateur NSIS est généré dans `release/`.

Depuis Windows :

```powershell
npm.cmd ci
npm.cmd run dist:linux
```

La commande crée une archive Linux dans `release/`. Copiez-la sur la machine Linux, puis extrayez-la et lancez le script d’installation :

```bash
tar -xzf clavier-1.0.0.tar.gz
cd clavier-1.0.0
bash install.sh
```

Le script installe Clavier dans `~/.local/opt/clavier`, ajoute l’application au menu et crée un raccourci avec son icône sur le bureau. L’installation est propre à l’utilisateur et ne demande pas les droits administrateur.

Sous Linux, la même commande produit la même archive :

```bash
npm ci
npm run dist:linux
```

L’archive contient l’application Electron, l’icône et le script d’installation. Ce paquet n’est pas au format AppImage.

Le workflow GitHub Actions **Applications de bureau** peut être déclenché manuellement. Il construit chaque installateur sur le système cible et publie les fichiers en artefacts.
