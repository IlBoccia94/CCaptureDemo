import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { map, Observable } from 'rxjs';
import { DocumentQueueItem } from '../../core/models/document.models';
import { ProcessingStatusService } from '../../core/services/processing-status.service';

@Component({
  selector: 'app-processed-documents-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './processed-documents-page.component.html',
  styleUrl: './processed-documents-page.component.css'
})
export class ProcessedDocumentsPageComponent {
  readonly completedDocuments$: Observable<DocumentQueueItem[]> = this.processingStatusService.queue$.pipe(
    map((items) => items.filter((item) => item.status === 'Completed'))
  );

  constructor(private readonly processingStatusService: ProcessingStatusService) {}
}
