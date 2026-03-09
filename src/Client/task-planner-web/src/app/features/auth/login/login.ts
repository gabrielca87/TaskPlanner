import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiErrorResponse } from '../../../shared/contracts/api.error';
import { AuthApi } from '../auth-api';
import { LoginRequest } from '../auth.contracts';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html'
})
export class Login {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authApi = inject(AuthApi);
  private readonly router = inject(Router);

  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.maxLength(1000)]]
  });

  readonly isSubmitting = signal(false);
  readonly requestError = signal('');

  submit(): void {
    this.requestError.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request: LoginRequest = this.form.getRawValue();
    this.isSubmitting.set(true);

    this.authApi.login(request).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        void this.router.navigate(['/task-items']);
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.requestError.set(this.mapError(error));
      }
    });
  }

  private mapError(error: HttpErrorResponse): string {
    const apiError = error.error as Partial<ApiErrorResponse> | null;
    if (apiError?.message && apiError.message.trim().length > 0) {
      return apiError.message;
    }

    return 'Unable to sign in right now.';
  }
}
