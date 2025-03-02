import { Component, input } from '@angular/core';
import { AnalysisFile } from '../../../../../../types/analysis-results';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-file-output',
  templateUrl: './file-output.component.html',
  styleUrls: ['./file-output.component.scss'],
  imports: [MatIcon, MatButton],
})
export class FileOutputComponent {
  files = input.required<AnalysisFile[]>();
  //
  // constructor(private outputFilesService: OutputFilesService) {
  // }
  //
  onDownloadFileButtonClick(file: AnalysisFile) {
    //   this.outputFilesService.downloadFile(file.fileId, file.fileName).subscribe((data) => {
    //     FileOutputComponent.downloadFile(file.fileName, data);
    //   });
  }

  onDownloadAllButtonClick() {
    //   this.outputFilesService.downloadAll(this.projectId, this.runIndex).subscribe((data) => {
    //     FileOutputComponent.downloadFile('outputFiles.zip', data);
    //   });
  }

  //
  // // todo to be moved to common service
  // private static downloadFile(fileName: string, data: any) {
  //   const a: any = document.createElement('a');
  //   document.body.appendChild(a);
  //   a.style = 'display: none';
  //   const blob = new Blob([data], {type: 'octet/stream'});
  //   const url = window.URL.createObjectURL(blob);
  //   a.href = url;
  //   a.download = fileName;
  //   a.click();
  //   window.URL.revokeObjectURL(url);
  // }
}
