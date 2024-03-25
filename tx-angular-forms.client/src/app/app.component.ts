import { HttpClient } from '@angular/common/http';
import { Component, HostListener } from '@angular/core';

declare const TXDocumentViewer: any;

interface DocumentData {
  document?: string;
  name?: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent{

  public documentData: DocumentData = {};

  constructor(private http: HttpClient) { }

  @HostListener('window:documentViewerLoaded', ['$event'])
  onDocumentViewerLoaded() {

    TXDocumentViewer.signatures.setSubmitCallback(function (data: string) {
      var element = document.createElement('a');
      element.setAttribute('href', 'data:application/pdf;;base64,' + data);
      element.setAttribute('download', "results.pdf");
      document.body.appendChild(element);
      element.click();
    })

    var signatureSettings = {
      showSignatureBar: true,
      redirectUrlAfterSignature: 'https://localhost:7122/document/customsign',
      ownerName: 'Paul',
      signerName: 'Jacob',
      signerInitials: 'PK',
      signatureBoxes: [{ name: 'txsign', signingRequired: true, style: 0 }]
    };

    this.http.get<DocumentData>('/document/prepare').subscribe(
      (result) => {
        this.documentData = result;
        console.log(this.documentData);
        TXDocumentViewer.loadDocument(this.documentData.document, this.documentData.name, signatureSettings);
      }
    );
    
  }

  title = 'myangularbackend.client';
}
