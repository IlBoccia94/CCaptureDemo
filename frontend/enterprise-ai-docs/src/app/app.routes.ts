import { Routes } from '@angular/router';
import { UploadPageComponent } from './pages/upload/upload-page.component';
import { ProcessingQueuePageComponent } from './pages/processing-queue/processing-queue-page.component';
import { ProcessedDocumentsPageComponent } from './pages/processed-documents/processed-documents-page.component';
import { DocumentDetailsPageComponent } from './pages/document-details/document-details-page.component';

export const routes: Routes = [
  { path: '', redirectTo: 'upload', pathMatch: 'full' },
  { path: 'upload', component: UploadPageComponent },
  { path: 'processing-queue', component: ProcessingQueuePageComponent },
  { path: 'processed-documents', component: ProcessedDocumentsPageComponent },
  { path: 'documents/:id', component: DocumentDetailsPageComponent },
  { path: '**', redirectTo: 'upload' }
];
