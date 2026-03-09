import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTaskItemRequest, TaskItem, UpdateTaskItemRequest } from './task-item.contracts';

@Injectable({
  providedIn: 'root'
})
export class TaskItemApi {
  private readonly http = inject(HttpClient);
  private readonly taskItemsUrl = `${environment.apiBaseUrl}/task-items`;

  getByCurrentUser(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.taskItemsUrl);
  }

  create(request: CreateTaskItemRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.taskItemsUrl, request);
  }

  update(id: string, request: UpdateTaskItemRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.taskItemsUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.taskItemsUrl}/${id}`);
  }
}
