const { app, BrowserWindow, net, protocol } = require('electron');
const path = require('node:path');
const { pathToFileURL } = require('node:url');

protocol.registerSchemesAsPrivileged([{ scheme: 'clavier', privileges: {
  standard: true, secure: true, supportFetchAPI: true, stream: true
} }]);

let mainWindow;
const appRoot = path.join(__dirname, '..', 'dist', 'clavier-web', 'browser');

app.whenReady().then(() => {
  protocol.handle('clavier', async (request) => {
    const url = new URL(request.url);
    if (url.host !== 'app') return new Response('Not found', { status: 404 });
    const pathname = decodeURIComponent(url.pathname);
    const file = path.resolve(appRoot, `.${pathname === '/' ? '/index.html' : pathname}`);
    const relative = path.relative(appRoot, file);
    if (relative.startsWith('..') || path.isAbsolute(relative)) return new Response('Not found', { status: 404 });
    return net.fetch(pathToFileURL(file).toString());
  });
  mainWindow = new BrowserWindow({
    width: 1200, height: 760, minWidth: 640, minHeight: 420,
    title: 'Clavier Lenovo T14 Gen 1',
    icon: path.join(appRoot, 'assets', 'keyboard-icon.png'),
    webPreferences: { contextIsolation: true, nodeIntegration: false, sandbox: true }
  });
  mainWindow.webContents.setWindowOpenHandler(() => ({ action: 'deny' }));
  mainWindow.webContents.on('will-navigate', (event, target) => {
    if (!target.startsWith('clavier://app/')) event.preventDefault();
  });
  mainWindow.loadURL('clavier://app/');
});

app.on('window-all-closed', () => { if (process.platform !== 'darwin') app.quit(); });
