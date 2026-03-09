import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenStorage {
  private readonly accessTokenKey = 'taskplanner.access_token';

  set(token: string): void {
    localStorage.setItem(this.accessTokenKey, token);
  }

  get(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  clear(): void {
    localStorage.removeItem(this.accessTokenKey);
  }

}
