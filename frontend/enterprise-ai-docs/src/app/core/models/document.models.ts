export type ProcessingStatus = 'Pending' | 'Processing' | 'Completed';

export interface UploadResponse {
  id: string;
  status: string;
}

export interface DocumentQueueItem {
  id: string;
  documentName: string;
  submissionTime: string;
  status: ProcessingStatus;
  progress: number;
  fileType: string;
}

export interface DocumentImage {
  id: string;
  pageNumber: number;
  label: string;
  detectionScore: number;
  sourcePath: string;
  croppedPath: string;
  overlayPath: string;
}

export interface ExtractedMetadata {
  documentImageId: string;
  fieldName: string;
  fieldValue: string;
  confidence: number;
}

export interface DocumentLog {
  step: string;
  message: string;
  isError: boolean;
  createdAtUtc: string;
}

export interface DocumentDetails {
  id: string;
  fileName: string;
  status: string;
  errorMessage?: string | null;
  processingStartedAtUtc?: string | null;
  processingCompletedAtUtc?: string | null;
  images: DocumentImage[];
  metadata: ExtractedMetadata[];
  logs: DocumentLog[];
}
