import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthSession } from '../services/auth-session';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html'
})
export class Navbar {
  private readonly authSession = inject(AuthSession);
  private readonly router = inject(Router);

  readonly isAuthenticated = this.authSession.isAuthenticated;
  readonly guestLinks = [
    { label: 'Login', path: '/login' },
    { label: 'Register', path: '/register' }
  ];

  logout(): void {
    this.authSession.clear();
    void this.router.navigate(['/login']);
  }
}
