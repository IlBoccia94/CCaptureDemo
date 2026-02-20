import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { finalize } from 'rxjs';
import { ProcessingStatusService } from '../../core/services/processing-status.service';
import { UploadService } from '../../core/services/upload.service';

@Component({
  selector: 'app-upload-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './upload-page.component.html',
  styleUrl: './upload-page.component.css'
})
export class UploadPageComponent {
  queuedFiles: File[] = [];
  selectedPreviewFile: File | null = null;
  isDragging = false;
  isUploading = false;
  feedbackMessage = '';

  constructor(
    private readonly uploadService: UploadService,
    private readonly processingStatusService: ProcessingStatusService
  ) {}

  onFileDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;

    const droppedFiles = event.dataTransfer?.files;
    if (!droppedFiles) {
      return;
    }

    this.addFiles(Array.from(droppedFiles));
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(): void {
    this.isDragging = false;
  }

  onFileInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files ? Array.from(input.files) : [];
    this.addFiles(files);
    input.value = '';
  }

  selectPreview(file: File): void {
    this.selectedPreviewFile = file;
  }

  removeFile(fileToRemove: File): void {
    this.queuedFiles = this.queuedFiles.filter((file) => file !== fileToRemove);

    if (this.selectedPreviewFile === fileToRemove) {
      this.selectedPreviewFile = this.queuedFiles[0] ?? null;
    }
  }

  startProcessing(): void {
    if (this.queuedFiles.length === 0 || this.isUploading) {
      return;
    }

    this.isUploading = true;
    this.feedbackMessage = '';
    const fileBatch = [...this.queuedFiles];

    this.uploadService
      .uploadDocuments(fileBatch)
      .pipe(finalize(() => (this.isUploading = false)))
      .subscribe({
        next: (responses) => {
          const uploadedIds = responses.map((response) => response.id);
          this.processingStatusService.enqueueDocuments(fileBatch, uploadedIds);
          this.feedbackMessage = `${fileBatch.length} document(s) submitted to the processing queue.`;
          this.queuedFiles = [];
          this.selectedPreviewFile = null;
        },
        error: () => {
          this.feedbackMessage = 'Failed to submit files. Verify backend connectivity and try again.';
        }
      });
  }

  private addFiles(files: File[]): void {
    const uniqueNewFiles = files.filter(
      (candidate) => !this.queuedFiles.some((existing) => existing.name === candidate.name && existing.size === candidate.size)
    );

    this.queuedFiles = [...this.queuedFiles, ...uniqueNewFiles];
    this.selectedPreviewFile ??= this.queuedFiles[0] ?? null;
  }
}
