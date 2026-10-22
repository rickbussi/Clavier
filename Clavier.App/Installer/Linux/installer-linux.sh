#!/usr/bin/env bash
set -euo pipefail

APP_NAME="Clavier"
APP_BINARY="Clavier"
SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
DATA_HOME="${XDG_DATA_HOME:-$HOME/.local/share}"
APP_DIR="$HOME/.local/opt/clavier"
ICON_DIR="$DATA_HOME/icons/hicolor/256x256/apps"
DESKTOP_DIR="$DATA_HOME/applications"

if [[ ! -f "$SCRIPT_DIR/$APP_BINARY" ]]; then
    echo "Binaire Linux introuvable : $SCRIPT_DIR/$APP_BINARY" >&2
    echo "Copiez les fichiers de publication Linux de Clavier dans ce repertoire, puis relancez ce script." >&2
    exit 1
fi
if [[ ! -f "$SCRIPT_DIR/keyboard-icon.png" ]]; then
    echo "Icone introuvable : $SCRIPT_DIR/keyboard-icon.png" >&2
    exit 1
fi

mkdir -p "$APP_DIR" "$ICON_DIR" "$DESKTOP_DIR"
# Copier tous les fichiers de publication voisins, avec leurs sous-repertoires,
# en omettant le script d'installation et l'image qui est installee separement.
find "$SCRIPT_DIR" -mindepth 1 -maxdepth 1 \
    ! -name "$(basename -- "${BASH_SOURCE[0]}")" \
    ! -name "keyboard-icon.png" \
    -exec cp -a -t "$APP_DIR" -- {} +
chmod +x "$APP_DIR/$APP_BINARY"
install -m 0644 "$SCRIPT_DIR/keyboard-icon.png" "$ICON_DIR/clavier.png"

cat > "$DESKTOP_DIR/clavier.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=Clavier
Comment=Clavier Lenovo T14 Gen 1
Exec=env -u GTK_IM_MODULE -u QT_IM_MODULE -u XMODIFIERS "$APP_DIR/$APP_BINARY"
Icon=clavier
Terminal=false
Categories=Utility;
StartupNotify=true
EOF
chmod 0644 "$DESKTOP_DIR/clavier.desktop"

if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database "$DESKTOP_DIR" >/dev/null 2>&1 || true
fi

echo "Clavier est installe. Vous le trouverez dans le menu des applications."
echo "Binaire : $APP_DIR/$APP_BINARY"
