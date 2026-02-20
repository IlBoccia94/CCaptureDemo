import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DocumentDetails } from '../models/document.models';

interface DocumentDetailsApiResponse {
  id: string;
  fileName: string;
  status: string;
  errorMessage?: string | null;
  processingStartedAtUtc?: string | null;
  processingCompletedAtUtc?: string | null;
  images: DocumentDetails['images'];
  metadata: DocumentDetails['metadata'];
  logs: DocumentDetails['logs'];
}

@Injectable({ providedIn: 'root' })
export class DocumentRetrievalService {
  private readonly endpoint = `${environment.apiBaseUrl}/documents`;

  constructor(private readonly http: HttpClient) {}

  getDocumentDetails(id: string): Observable<DocumentDetails> {
    return this.http
      .get<DocumentDetailsApiResponse>(`${this.endpoint}/${id}`)
      .pipe(
        map((response) => ({
          id: response.id,
          fileName: response.fileName,
          status: response.status,
          errorMessage: response.errorMessage,
          processingStartedAtUtc: response.processingStartedAtUtc,
          processingCompletedAtUtc: response.processingCompletedAtUtc,
          images: response.images,
          metadata: response.metadata,
          logs: response.logs
        }))
      );
  }
}
