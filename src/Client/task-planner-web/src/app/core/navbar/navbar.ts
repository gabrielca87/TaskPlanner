import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

type NavLink = {
  label: string;
  path: string;
};

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html'
})
export class Navbar {
  readonly links: readonly NavLink[] = [
    { label: 'Tasks', path: '/task-items' },
    { label: 'Login', path: '/login' },
    { label: 'Register', path: '/register' }
  ];
}
