import { computed, inject, Injectable, signal } from '@angular/core';
import { AuthResponse } from '../../features/auth/auth.contracts';
import { TokenStorage } from './token-storage';

type SessionUser = {
  userId: string;
  email: string;
  displayName: string;
};

@Injectable({
  providedIn: 'root'
})
export class AuthSession {
  private readonly userStorageKey = 'taskplanner.auth_user';
  private readonly tokenStorage = inject(TokenStorage);
  private readonly userState = signal<SessionUser | null>(this.readUserFromStorage());

  readonly user = this.userState.asReadonly();
  readonly isAuthenticated = computed(() => this.userState() !== null && this.tokenStorage.hasToken());

  open(response: AuthResponse): void {
    this.tokenStorage.set(response.accessToken);

    const user: SessionUser = {
      userId: response.userId,
      email: response.email,
      displayName: response.displayName
    };

    localStorage.setItem(this.userStorageKey, JSON.stringify(user));
    this.userState.set(user);
  }

  clear(): void {
    this.tokenStorage.clear();
    localStorage.removeItem(this.userStorageKey);
    this.userState.set(null);
  }

  getAccessToken(): string | null {
    return this.tokenStorage.get();
  }

  private readUserFromStorage(): SessionUser | null {
    const serializedUser = localStorage.getItem(this.userStorageKey);
    if (!serializedUser) {
      return null;
    }

    try {
      const parsedUser = JSON.parse(serializedUser) as SessionUser;
      if (!parsedUser.userId || !parsedUser.email || !parsedUser.displayName) {
        localStorage.removeItem(this.userStorageKey);
        return null;
      }

      return parsedUser;
    } catch {
      localStorage.removeItem(this.userStorageKey);
      return null;
    }
  }
}
