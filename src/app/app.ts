import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';

type Accent = { character: string; base?: string; kind?: 'grave' | 'acute' | 'circumflex' | 'diaeresis' | 'special' };
type Marker = { x: number; y: number; width?: number; color: 'blue' | 'green'; label: string };

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class AppComponent {
  @ViewChild('keyboardFrame') keyboardFrame?: ElementRef<HTMLElement>;
  @ViewChild('testInput') testInput?: ElementRef<HTMLInputElement>;

  readonly accents: Accent[] = [
    ...'ÀàÂâÇçÉéÈèÊêËëÎîÏïÔôŒœÙùÛû'.split('').map((character) => ({ character })),
  ];
  selected: string | null = null;
  value = '';
  contextMenu: { x: number; y: number; character?: string } | null = null;
  markers: Marker[] = [];

  @HostListener('window:resize') onResize(): void { this.updateMarkers(); }
  @HostListener('document:click') closeContextMenu(): void { this.contextMenu = null; }
  @HostListener('document:keydown.escape') closeOnEscape(): void { this.contextMenu = null; }

  select(character: string): void {
    this.selected = this.selected === character ? null : character;
    this.updateMarkers();
    this.testInput?.nativeElement.focus();
  }

  showCharacterMenu(event: MouseEvent, character: string): void {
    event.preventDefault(); event.stopPropagation();
    this.contextMenu = { x: event.clientX, y: event.clientY, character };
  }

  showInputMenu(event: MouseEvent): void {
    event.preventDefault();
    this.contextMenu = { x: event.clientX, y: event.clientY };
  }

  async menuAction(action: string): Promise<void> {
    const input = this.testInput?.nativeElement;
    if (this.contextMenu?.character) {
      if (action === 'copy') await navigator.clipboard?.writeText(this.contextMenu.character);
      this.contextMenu = null;
      return;
    }
    if (!input) return;
    switch (action) {
      case 'undo': document.execCommand('undo'); break;
      case 'redo': document.execCommand('redo'); break;
      case 'cut': document.execCommand('cut'); break;
      case 'copy': document.execCommand('copy'); break;
      case 'paste':
        try { document.execCommand('insertText', false, await navigator.clipboard.readText()); } catch { /* clipboard permission denied */ }
        break;
      case 'selectAll': input.select(); break;
    }
    this.contextMenu = null;
    input.focus();
  }

  private updateMarkers(): void {
    const frame = this.keyboardFrame?.nativeElement;
    if (!frame || !this.selected) { this.markers = []; return; }
    const sx = frame.clientWidth / 1500;
    const sy = frame.clientHeight / 693;
    const side = 96;
    const marker = (x: number, y: number, color: Marker['color'], label: string, width = side): Marker => ({
      x: x * sx - width * sx / 2, y: y * sy - side * sy / 2,
      width: width * sx, color, label,
    });
    const c = this.selected;
    const upper = c === c.toUpperCase();
    const result: Marker[] = [];
    const shift = (color: Marker['color'], split: boolean) => {
      const centerX = split ? color === 'blue' ? 66 : 174 : 120;
      const width = split ? side * 1.125 : side * 2.25;
      result.push(marker(centerX, 430, color, color === 'blue' ? '1' : '2', width));
    };
    if (c === 'ç' || c === 'Ç' || c === 'œ' || c === 'Œ') {
      result.push(marker(c.toLowerCase() === 'œ' ? 930 : 982, c.toLowerCase() === 'œ' ? 330 : 428, 'blue', '1', c.toLowerCase() === 'œ' ? side * .88 : side));
      result.push(marker(980, 530, 'blue', '1'));
      if (upper) shift('blue', false);
    } else {
      const lower = c.toLowerCase();
      const base = 'àâ'.includes(lower) ? 'a' : 'éèêë'.includes(lower) ? 'e' : 'îï'.includes(lower) ? 'i' : lower === 'ô' ? 'o' : 'u';
      const letter = ({ a: [239, 329], e: [407, 229], i: [907, 229], o: [1006, 229], u: [808, 229] } as Record<string, number[]>)[base]!;
      const kind = 'àèù'.includes(lower) ? 'grave' : lower === 'é' ? 'acute' : 'âêîôû'.includes(lower) ? 'circumflex' : 'diaeresis';
      const accent = kind === 'grave' ? [65, 128] : kind === 'circumflex' ? [655, 129] : [1228, 330];
      result.push(marker(letter[0], letter[1], 'green', '2'));
      result.push(marker(accent[0], accent[1], 'blue', '1'));
      const accentShift = kind === 'circumflex' || kind === 'diaeresis';
      if (accentShift) shift('blue', upper);
      if (upper) shift('green', accentShift);
    }
    this.markers = result;
  }
}
