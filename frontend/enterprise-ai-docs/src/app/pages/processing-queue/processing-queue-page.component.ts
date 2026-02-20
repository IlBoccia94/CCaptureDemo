import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DocumentQueueItem } from '../../core/models/document.models';
import { ProcessingStatusService } from '../../core/services/processing-status.service';

@Component({
  selector: 'app-processing-queue-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './processing-queue-page.component.html',
  styleUrl: './processing-queue-page.component.css'
})
export class ProcessingQueuePageComponent {
  private readonly processingStatusService = inject(ProcessingStatusService);

  readonly queueItems$: Observable<DocumentQueueItem[]> = this.processingStatusService.queue$;
}
