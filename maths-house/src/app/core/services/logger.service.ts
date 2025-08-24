import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  private enabledCache: boolean | null = null;

  private computeEnabled(): boolean {
    try {
      const params = new URLSearchParams(window.location.search);
      if (params.get('debug') === '1') return true;
      const ls = localStorage.getItem('debug');
      if (ls && (ls === '1' || ls.toLowerCase() === 'true')) return true;
    } catch {}
    return false;
  }

  get enabled(): boolean {
    if (this.enabledCache === null) this.enabledCache = this.computeEnabled();
    return this.enabledCache;
  }

  setEnabled(value: boolean) {
    this.enabledCache = value;
    try { localStorage.setItem('debug', value ? '1' : '0'); } catch {}
  }

  debug(...args: any[]) { if (this.enabled) console.debug('[DEBUG]', ...args); }
  info(...args: any[])  { if (this.enabled) console.info('[INFO ]', ...args); }
  warn(...args: any[])  { if (this.enabled) console.warn('[WARN ]', ...args); }
  error(...args: any[]) { console.error('[ERROR]', ...args); }
}
