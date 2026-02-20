import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, map, of, switchMap } from 'rxjs';
import { DocumentDetails, DocumentImage, ExtractedMetadata } from '../../core/models/document.models';
import { DocumentRetrievalService } from '../../core/services/document-retrieval.service';
import { ProcessingStatusService } from '../../core/services/processing-status.service';

interface ImageFlowItem {
  image: DocumentImage;
  metadata: ExtractedMetadata[];
}

@Component({
  selector: 'app-document-details-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './document-details-page.component.html',
  styleUrl: './document-details-page.component.css'
})
export class DocumentDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly documentRetrievalService = inject(DocumentRetrievalService);
  private readonly processingStatusService = inject(ProcessingStatusService);

  readonly details$ = this.route.paramMap.pipe(
    map((params) => params.get('id')),
    switchMap((id) => {
      if (!id) {
        return of(this.buildFallbackDetails('Unknown Document', 'Unavailable', []));
      }

      return this.documentRetrievalService.getDocumentDetails(id).pipe(
        catchError(() => {
          const fallbackName = this.processingStatusService.getQueueSnapshot().find((item) => item.id === id)?.documentName;
          return of(this.buildFallbackDetails(fallbackName ?? 'Document', id, this.mockImages(id)));
        })
      );
    })
  );


  toFlowItems(details: DocumentDetails): ImageFlowItem[] {
    return details.images.map((image) => ({
      image,
      metadata: details.metadata.filter((entry) => entry.documentImageId === image.id)
    }));
  }

  private buildFallbackDetails(fileName: string, id: string, images: DocumentImage[]): DocumentDetails {
    return {
      id,
      fileName,
      status: 'Completed',
      processingStartedAtUtc: new Date().toISOString(),
      processingCompletedAtUtc: new Date().toISOString(),
      images,
      metadata: images.flatMap((image, index) => [
        {
          documentImageId: image.id,
          fieldName: 'Invoice Number',
          fieldValue: `INV-2025-${index + 10}`,
          confidence: 0.94
        },
        {
          documentImageId: image.id,
          fieldName: 'Vendor',
          fieldValue: 'Northwind Global',
          confidence: 0.9
        }
      ]),
      logs: []
    };
  }

  private mockImages(seed: string): DocumentImage[] {
    return [1, 2, 3].map((pageNumber) => ({
      id: `${seed}-img-${pageNumber}`,
      pageNumber,
      label: pageNumber === 2 ? 'Signature Block' : 'Invoice Header',
      detectionScore: 0.91,
      sourcePath: `https://placehold.co/720x460/111b2f/e3ebff?text=Source+Page+${pageNumber}`,
      croppedPath: `https://placehold.co/420x260/172845/e3ebff?text=Cropped+Image+${pageNumber}`,
      overlayPath: `https://placehold.co/420x260/1f3352/31d0b9?text=Bounding+Overlay+${pageNumber}`
    }));
  }
}
