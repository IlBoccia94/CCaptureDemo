import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, interval } from 'rxjs';
import { DocumentQueueItem, ProcessingStatus } from '../models/document.models';

@Injectable({ providedIn: 'root' })
export class ProcessingStatusService {
  private readonly queueSubject = new BehaviorSubject<DocumentQueueItem[]>([]);
  private simulationStarted = false;

  readonly queue$: Observable<DocumentQueueItem[]> = this.queueSubject.asObservable();

  constructor() {
    this.startSimulation();
  }

  enqueueDocuments(files: File[], uploadedIds: string[]): void {
    const now = new Date().toISOString();
    const nextItems = files.map((file, index) => ({
      id: uploadedIds[index] ?? crypto.randomUUID(),
      documentName: file.name,
      submissionTime: now,
      status: 'Pending' as ProcessingStatus,
      progress: 0,
      fileType: this.getFileType(file.name)
    }));

    this.queueSubject.next([...nextItems, ...this.queueSubject.value]);
  }

  getQueueSnapshot(): DocumentQueueItem[] {
    return this.queueSubject.value;
  }

  private startSimulation(): void {
    if (this.simulationStarted) {
      return;
    }

    this.simulationStarted = true;

    interval(1500).subscribe(() => {
      const updated = this.queueSubject.value.map((item) => {
        if (item.status === 'Completed') {
          return item;
        }

        const increment = Math.floor(Math.random() * 15) + 8;
        const nextProgress = Math.min(item.progress + increment, 100);

        return {
          ...item,
          progress: nextProgress,
          status: this.resolveStatus(nextProgress)
        };
      });

      this.queueSubject.next(updated);
    });
  }

  private resolveStatus(progress: number): ProcessingStatus {
    if (progress >= 100) {
      return 'Completed';
    }

    if (progress > 10) {
      return 'Processing';
    }

    return 'Pending';
  }

  private getFileType(fileName: string): string {
    const parts = fileName.split('.');
    return parts.length > 1 ? parts[parts.length - 1].toUpperCase() : 'FILE';
  }
}
