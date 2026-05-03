import { Injectable } from '@angular/core';

export interface AppConfig {
  apiUrl: string;
  appName: string;
  version: string;
  production: boolean;
}

const DEFAULT_CONFIG: AppConfig = {
  apiUrl: 'http://localhost:5041',  // dev: dotnet run; docker: override to http://localhost:5100
  appName: 'CleanInk OnCall',
  version: '1.0.0',
  production: false,
};

@Injectable({ providedIn: 'root' })
export class ConfigService {
  private config: AppConfig = DEFAULT_CONFIG;

  get apiUrl(): string    { return this.config.apiUrl; }
  get appName(): string   { return this.config.appName; }
  get version(): string   { return this.config.version; }
  get production(): boolean { return this.config.production; }

  load(overrides: Partial<AppConfig>): void {
    this.config = { ...this.config, ...overrides };
  }
}
