import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-document-details-page',
  standalone: true,
  templateUrl: './document-details-page.component.html',
  styleUrl: './document-details-page.component.css'
})
export class DocumentDetailsPageComponent {
  documentId = this.route.snapshot.paramMap.get('id');

  constructor(private readonly route: ActivatedRoute) {}
}
