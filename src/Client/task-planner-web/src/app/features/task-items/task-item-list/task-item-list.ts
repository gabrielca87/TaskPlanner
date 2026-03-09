import { HttpErrorResponse } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthSession } from '../../../core/services/auth-session';
import { ApiErrorResponse } from '../../../shared/contracts/api.error';
import { TaskItemApi } from '../task-item-api';
import { CreateTaskItemRequest, TaskItem, UpdateTaskItemRequest } from '../task-item.contracts';

@Component({
  selector: 'app-task-item-list',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './task-item-list.html'
})
export class TaskItemList implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly taskItemApi = inject(TaskItemApi);
  private readonly authSession = inject(AuthSession);

  readonly form = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(250)]],
    description: ['', [Validators.maxLength(1000)]]
  });

  readonly tasks = signal<TaskItem[]>([]);
  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);
  readonly listError = signal('');
  readonly requestError = signal('');
  readonly editingTaskId = signal<string | null>(null);
  readonly formMode = computed(() => (this.editingTaskId() ? 'edit' : 'create'));
  readonly isEmpty = computed(() => !this.isLoading() && this.tasks().length === 0);

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.listError.set('');
    this.isLoading.set(true);

    this.taskItemApi.getByCurrentUser().subscribe({
      next: (tasks) => {
        this.tasks.set(tasks);
        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.listError.set(this.mapError(error, 'Unable to load task items right now.'));
      }
    });
  }

  startEdit(task: TaskItem): void {
    this.requestError.set('');
    this.editingTaskId.set(task.id);
    this.form.setValue({
      title: task.title,
      description: task.description ?? ''
    });
  }

  cancelEdit(): void {
    this.requestError.set('');
    this.editingTaskId.set(null);
    this.form.reset({
      title: '',
      description: ''
    });
  }

  submit(): void {
    this.requestError.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.editingTaskId()) {
      this.updateTask();
      return;
    }

    this.createTask();
  }

  deleteTask(task: TaskItem): void {
    const confirmed = window.confirm(`Delete "${task.title}"?`);
    if (!confirmed) {
      return;
    }

    this.requestError.set('');

    this.taskItemApi.delete(task.id).subscribe({
      next: () => {
        this.tasks.update((items) => items.filter((item) => item.id !== task.id));
      },
      error: (error: HttpErrorResponse) => {
        this.requestError.set(this.mapError(error, 'Unable to delete task item right now.'));
      }
    });
  }

  private createTask(): void {
    const formValue = this.form.getRawValue();
    const request: CreateTaskItemRequest = {
      userId: this.authSession.user()!.userId, // authGuard guarantees user is present
      title: formValue.title.trim(),
      description: this.normalizeDescription(formValue.description)
    };

    this.isSubmitting.set(true);

    this.taskItemApi.create(request).subscribe({
      next: (createdTask) => {
        this.tasks.update((items) => [createdTask, ...items]);
        this.isSubmitting.set(false);
        this.cancelEdit();
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.requestError.set(this.mapError(error, 'Unable to create task item right now.'));
      }
    });
  }

  private updateTask(): void {
    const id = this.editingTaskId();
    if (!id) {
      return;
    }

    const formValue = this.form.getRawValue();
    const request: UpdateTaskItemRequest = {
      id,
      title: formValue.title.trim(),
      description: this.normalizeDescription(formValue.description)
    };

    this.isSubmitting.set(true);

    this.taskItemApi.update(id, request).subscribe({
      next: (updatedTask) => {
        this.tasks.update((items) => items.map((item) => (item.id === updatedTask.id ? updatedTask : item)));
        this.isSubmitting.set(false);
        this.cancelEdit();
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.requestError.set(this.mapError(error, 'Unable to update task item right now.'));
      }
    });
  }

  private normalizeDescription(value: string): string | null {
    const trimmedValue = value.trim();
    return trimmedValue.length === 0 ? null : trimmedValue;
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): string {
    const apiError = error.error as Partial<ApiErrorResponse> | null;
    if (!apiError) {
      return fallbackMessage;
    }

    const validationMessage = this.getFirstValidationMessage(apiError);
    if (validationMessage) {
      return validationMessage;
    }

    if (apiError.message && apiError.message.trim().length > 0) {
      return apiError.message;
    }

    return fallbackMessage;
  }

  private getFirstValidationMessage(error: Partial<ApiErrorResponse>): string | null {
    if (!error.errors) {
      return null;
    }

    const firstKey = Object.keys(error.errors)[0];
    if (!firstKey) {
      return null;
    }

    const messages = error.errors[firstKey];
    if (!messages || messages.length === 0) {
      return null;
    }

    return messages[0];
  }
}
