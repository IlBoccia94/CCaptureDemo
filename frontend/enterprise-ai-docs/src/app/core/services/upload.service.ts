import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UploadResponse } from '../models/document.models';

@Injectable({ providedIn: 'root' })
export class UploadService {
  private readonly endpoint = `${environment.apiBaseUrl}/documents/upload`;

  constructor(private readonly http: HttpClient) {}

  uploadDocument(file: File): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<UploadResponse>(this.endpoint, formData);
  }

  uploadDocuments(files: File[]): Observable<UploadResponse[]> {
    return forkJoin(files.map((file) => this.uploadDocument(file)));
  }
}
